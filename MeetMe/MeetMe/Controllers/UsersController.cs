using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MeetMe.Models;
using MeetMe.Services;

namespace MeetMe.Controllers
{
    public class UsersController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
		private UserAuthenticationManager authenticator = new UserAuthenticationManager();

		/// <summary>
		/// Register a user. Returns his Id
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[AllowAnonymous]
		[System.Web.Http.HttpPost]
		[Route("api/Users/Register")]
		public IHttpActionResult Register(RegisterBindingModel model)
		{
			if(db.ApplicationUsers.Any(x => x.Token == model.Token))
			{
				return BadRequest();
			}

			var rating = new Rating();
			db.Ratings.Add(rating);
			db.SaveChanges();

			var use = new Models.User
			{
				Token = model.Token,
				RatingID = rating.Id
			};
			db.ApplicationUsers.Add(use);

			db.SaveChanges();

			return Json(use.Id);
		}

		/// <summary>
		/// Get a list of all users
		/// </summary>
		/// <returns></returns>
		// GET: api/Users
		[HttpGet]
		[Route("api/Users")]
		public IEnumerable<UserViewModel> GetUsers()
        {
			var list = new List<UserViewModel>();

			foreach (var item in db.ApplicationUsers)
			{
				list.Add (new UserViewModel
				{
					Id = item.Id,
					Token = item.Token,
					FirstName = item.FirstName,
					LastName = item.LastName,
					Email = item.Email,
					PhoneNumber = item.PhoneNumber,
					Description = item.Description,
					PhotoURL = item.PhotoURL,
					FacebookURL = item.FacebookURL
				});
			}

			foreach (var usrmdl in list)
			{
				var item = db.ApplicationUsers.Find(usrmdl.Id);
				var rating = db.Ratings.Find(item.RatingID);

				if (rating == null)
				{
					rating = db.Ratings.Add(new Rating());
					db.SaveChanges();
					item.RatingID = rating.Id;
					db.SaveChanges();
				}

				float rate;

				if (rating.NumberOfRates < 3)
				{
					rate = 6f;
				}

				else
				{
					rate = rating.Sum / rating.NumberOfRates;
				}

				usrmdl.Rating = rate;
			}

            return list;
        }

		/// <summary>
		/// Get user details by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
        // GET: api/Users/5
		[HttpGet]
        [ResponseType(typeof(UserViewModel))]
        public IHttpActionResult GetUser(int id)
        {
            User item = db.ApplicationUsers.Find(id);
            if (item == null)
            {
                return NotFound();
            }

			var rating =  db.Ratings.Find(item.RatingID);
			if(rating == null)
			{
				rating = db.Ratings.Add(new Rating());
				db.SaveChanges();
				item.RatingID = rating.Id;
				db.SaveChanges();
			}

			float rate;

			if (rating.NumberOfRates < 3)
			{
				rate = 6f;
			}

			else
			{
				rate = rating.Sum / rating.NumberOfRates;
			}

			return Ok(new UserViewModel
			{
				Id = item.Id,
				Token = item.Token,
				FirstName = item.FirstName,
				LastName = item.LastName,
				Email = item.Email,
				PhoneNumber = item.PhoneNumber,
				Description = item.Description,
				PhotoURL = item.PhotoURL,
				Rating = rate,
				FacebookURL = item.FacebookURL
			});
        }

		/// <summary>
		/// Rate a user. Returns his current rating. 6 if not enough rates
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="grade"></param>
		/// <returns>Returns his current rating. 6 if not enough rates</returns>
		[HttpPost]
		[Route("~/api/Users/{userId:int}/Rate")]
		public IHttpActionResult RateUser(int userId, float grade)
		{
			var usr = authenticator.AuthorizeGetUser(Request, db);

			if (usr == null)
			{
				return Unauthorized();
			}

			var userToRate = db.ApplicationUsers.Find(userId);

			if (userToRate == null)
			{
				return NotFound();
			}

			var rating = db.Ratings.Find(userToRate.RatingID);

			if (rating == null)
			{
				rating = db.Ratings.Add(new Rating());
				db.SaveChanges();
				userToRate.RatingID = rating.Id;
				db.SaveChanges();
			}

			float rate;

			if (rating.NumberOfRates < 3)
			{
				rate = 6f;
			}

			else
			{
				rate = rating.Sum / rating.NumberOfRates;
			}

			rating.NumberOfRates += 1;
			rating.Sum += grade;

			db.SaveChanges();
			rate = rating.Sum / rating.NumberOfRates;
			return Json(rate);
		}

		/// <summary>
		/// Update user info
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
        // PUT: api/Users/5
		[HttpPut]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(UserViewModel user)
        {
			var usr = authenticator.AuthorizeGetUser(Request, db);

			if (usr == null)
			{
				return Unauthorized();
			}


			usr.FirstName = user.FirstName;
			usr.LastName = user.LastName;
			usr.PhoneNumber = user.PhoneNumber;
			usr.Email = user.Email;
			usr.Description = user.Description;
			usr.PhotoURL = user.PhotoURL;
			usr.FacebookURL = user.FacebookURL;

            db.SaveChanges();


			user.Id = usr.Id;
			user.FirstName = usr.FirstName;
			user.LastName = usr.LastName;
			user.PhoneNumber = usr.PhoneNumber;
			user.Email = usr.Email;
			user.Description = usr.Description;
			user.PhotoURL = usr.PhotoURL;
			user.FacebookURL = usr.FacebookURL;
			return Json(user);
        }

		/// <summary>
		/// Get Current Id
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("api/MyId")]
		public IHttpActionResult TestId()
		{
			var usr = authenticator.AuthorizeGetUser(Request, db);

			if (usr == null)
			{
				return Unauthorized();
			}

			return Json(usr.Id);
		}

		/// <summary>
		/// Delete User
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		// DELETE: api/Users/5
		[HttpDelete]
		[ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            User user = db.ApplicationUsers.Find(id);
            if (user == null)
            {
                return NotFound();
            }

			var guests = db.Guests.Where(x => x.User.Equals(user));

			db.Guests.RemoveRange(guests);

            db.ApplicationUsers.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

		/// <summary>
		/// Get a list of user names from given list of ids
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("api/Users")]
		[ResponseType(typeof(IEnumerable<UserOnlyName>))]
		public IHttpActionResult GetUserNames(List<int> ids)
		{
			var list = new List<UserOnlyName>();

			foreach (var id in ids)
			{
				var usr = db.ApplicationUsers.Find(id);
				if ( usr != null )
				{
					list.Add(new UserOnlyName {Id = id, FirstName = usr.FirstName, LastName = usr.LastName, PhotoURL = usr.PhotoURL, FacebookURL = usr.FacebookURL });
				}
			}

			return Json(list);
		}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.ApplicationUsers.Count(e => e.Id == id) > 0;
        }


    }
}