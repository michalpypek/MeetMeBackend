using MeetMe.Models;
using MeetMe.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Spatial;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace MeetMe.Controllers
{
    public class EventsController : ApiController
    {
		private ApplicationDbContext db = new ApplicationDbContext();

		private UserAuthenticationManager authenticator = new UserAuthenticationManager();

		/// <summary>
		/// Create a New Event
		/// </summary>
		/// <param name="evt"></param>
		/// <returns></returns>
		[ResponseType(typeof(EventViewModel))]
		public IHttpActionResult Post(EventViewModel evt)
		{
			var usr = authenticator.AuthorizeGetUser(Request, db);

			if (usr == null)
			{
				return Unauthorized();
			}

			var evtDb = db.Events.Add(
				new Event(evt));
			evtDb.CreatorId = usr.Id;
			db.SaveChanges();
			evt.Id = evtDb.Id;
			return Ok(evt);
		}

		// GET: api/Events
		/// <summary>
		/// Get a List of all events that will happen
		/// </summary>
		/// <returns></returns>
		[ResponseType(typeof(IEnumerable<EventViewModel>))]
		public IHttpActionResult Get()
        {
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			if (shiz == null)
			{
				return Unauthorized();
			}
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			List<EventViewModel> events = GetAllEvents();

			GetFutureOrNotOlderThan24(ref events);

			return Json(events);
        }

		/// <summary>
		/// Get a List of all events that will happen, or have ended no longer than 24 hours ago
		/// </summary>
		/// <returns></returns>
		[ResponseType(typeof(IEnumerable<EventViewModel>))]
		[Route("api/Events/all")]
		public IHttpActionResult GetOld()
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			if (shiz == null)
			{
				return Unauthorized();
			}
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			List<EventViewModel> events = GetAllEvents();

			return Json(events);
		}

		// GET: api/Events/5
		/// <summary>
		/// Get a specific Event, by ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[ResponseType(typeof(EventViewModel))]
        public IHttpActionResult Get(int id)
        {
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			if (shiz == null)
			{
				return Unauthorized();
			}
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			Event evt = db.Events.Find(id);
			if (evt == null)
			{
				return NotFound();
			}

			var view = new EventViewModel(evt);

			GetGuestIds(ref view);

			return Ok(view);
		}

		/// <summary>
		/// Join an event
		/// </summary>
		/// <param name="eventId">Id of the event to join</param>
		/// <returns></returns>
		[HttpPost]
		[Route("api/Events/{eventId:int}/Join")]
		[ResponseType(typeof(EventViewModel))]
		public IHttpActionResult Join(int eventId)
		{
			var usr = authenticator.AuthorizeGetUser(Request, db);

			if(usr == null)
			{
				return Unauthorized();
			}

			Event evt = db.Events.Find(eventId);
			if (evt == null)
			{
				return NotFound();
			}

			if (evt.Guests == null)
			{
				evt.Guests = new List<Guest>();
			}

			var view = new EventViewModel(evt);
			GetGuestIds(ref view);

			if(view.GuestsIds.Contains(usr.Id))
			{
				return Json("Already Joined");
			}


			var guest = new Guest { User = usr, UserId = usr.Id };
			db.Guests.Add(guest);

			db.SaveChanges();

			if(evt.Guests.Count >= evt.GuestLimit)
			{
				return Json("Event limit reached");
			}

			if (!evt.Guests.Contains(guest))
			{
				evt.Guests.Add(guest);
			}

			db.SaveChanges();

			view = new EventViewModel(evt);
			GetGuestIds(ref view);
			return Ok(view);
		}

		/// <summary>
		/// Leave an event
		/// </summary>
		/// <param name="eventId">Id of the event to leave</param>
		/// <returns></returns>
		[HttpPost]
		[Route("api/Events/{eventId:int}/Leave")]
		[ResponseType(typeof(EventViewModel))]
		public IHttpActionResult Leave(int eventId)
		{
			var usr = authenticator.AuthorizeGetUser(Request, db);

			if (usr == null)
			{
				return Unauthorized();
			}

			Event evt = db.Events.Find(eventId);
			if (evt == null)
			{
				return NotFound();
			}

			if (evt.Guests == null)
			{
				evt.Guests = new List<Guest>();
			}

			var view = new EventViewModel(evt);
			GetGuestIds(ref view);

			int guestId = -1;

			if(!view.GuestsIds.Contains(usr.Id))
			{
				return Json("Not a guest in this event");
			}


			foreach (var item in evt.Guests)
			{
				var _guest = db.Guests.Find(item.Id);

				if (_guest != null)
				{
					if(_guest.UserId == usr.Id)
					{
						guestId = _guest.Id;
					}
				}
			}

			Guest guest = db.Guests.Find(guestId);

			if (guest == null)
			{
				return Json("Already left or something went wrong");
			}

			if (evt.Guests.Contains(guest))
			{
				evt.Guests.Remove(guest);
			}

			db.SaveChanges();

			view = new EventViewModel(evt);
			GetGuestIds(ref view);
			return Ok(view);
		}


		/// <summary>
		/// Returns whether the user has rated the event already
		/// </summary>
		/// <param name="eventId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("api/Events/{eventId:int}/WasRated")]
		public IHttpActionResult WasRated(int eventId)
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			if (shiz == null)
			{
				return Unauthorized();
			}
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			var usr = authenticator.GetUserByToken(token, db);

			Event evt = db.Events.Find(eventId);
			if (evt == null)
			{
				return NotFound();
			}

			var creator = db.ApplicationUsers.Find(evt.CreatorId);

			if (creator == null)
			{
				return NotFound();
			}

			var rating = db.Ratings.Find(creator.RatingID);

			if (rating == null)
			{
				rating = db.Ratings.Add(new Rating());
				db.SaveChanges();
				creator.RatingID = rating.Id;
				db.SaveChanges();
				return Json(false);
			}

			if(rating.UsersThatRated == null)
			{
				rating.UsersThatRated = new List<User>();
			}

			GetRatersIds(ref rating);

			bool wasRated = rating.UsersThatRatedIds.Contains(usr.Id);

			return Json(wasRated);
		}


		/// <summary>
		/// Rate the creator of an event
		/// </summary>
		/// <param name="eventId"></param>
		/// <param name="rating"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("api/Events/{eventId:int}/Rate")]
		public IHttpActionResult Rate(int eventId, float grade)
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			if (shiz == null)
			{
				return Unauthorized();
			}
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			var usr = authenticator.GetUserByToken(token, db);

			Event evt = db.Events.Find(eventId);
			if (evt == null)
			{
				return NotFound();
			}

			var creator = db.ApplicationUsers.Find(evt.CreatorId);

			if(creator == null)
			{
				return NotFound();
			}

			var rating = db.Ratings.Find(creator.RatingID);

			if (rating == null)
			{
				rating = db.Ratings.Add(new Rating());
				db.SaveChanges();
				creator.RatingID = rating.Id;
				db.SaveChanges();
			}

			if (rating.UsersThatRated == null)
			{
				rating.UsersThatRated = new List<User>();
			}

			GetRatersIds(ref rating);

			bool wasRated = rating.UsersThatRatedIds.Contains(usr.Id);

			if(wasRated)
			{
				return Json("Error: Already rated");
			}

			rating.UsersThatRated.Add(usr);

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
		/// Get events within a specified range around given location
		/// </summary>
		/// <param name="lat">Latitude</param>
		/// <param name="lon">Longitude</param>
		/// <param name="range">(meters)Range to look in</param>
		/// <returns></returns>
		[ResponseType(typeof(IEnumerable<EventViewModel>))]
		public IHttpActionResult GetEvents(double lat, double lon, double range)
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			if (shiz == null)
			{
				return Unauthorized();
			}
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			var events = GetEventsByLocation(lat, lon, range).ToList();

			GetFutureOrNotOlderThan24(ref events);

			return Json(events);
		}

		/// <summary>
		/// Get a list of events current user takes part in (as a creator or guest, future, or not older than 24h)
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route ("api/Events/My")]
		[ResponseType(typeof(IEnumerable<EventViewModel>))]
		public IHttpActionResult GetMyEvents()
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			if (shiz == null)
			{
				return Unauthorized();
			}
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			var usr = authenticator.GetUserByToken(token, db);

			List<EventViewModel> allEvents = GetAllEvents();
			List<EventViewModel> events = new List<EventViewModel>();

			foreach (var evt in allEvents)
			{
				var et = db.Events.Find(evt.Id);

				if (et.CreatorId == usr.Id || evt.GuestsIds.Contains(usr.Id))
				{
					events.Add(evt);
				}
			}

			GetFutureOrNotOlderThan24(ref events);

			return Json(events);
		}

		/// <summary>
		/// Get a list of all events current user takes or took part in (as a creator or guest)
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("api/Events/My/all")]
		[ResponseType(typeof(IEnumerable<EventViewModel>))]
		public IHttpActionResult GetAllMyEvents()
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			if (shiz == null)
			{
				return Unauthorized();
			}
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			var usr = authenticator.GetUserByToken(token, db);

			Guest guest = db.Guests.Find(usr.Id);


			if (guest == null)
			{
				guest = new Guest { Id = usr.Id, User = usr, UserId = usr.Id };
				db.Guests.Add(guest);
			}

			db.SaveChanges();


			List<EventViewModel> allEvents = GetAllEvents();
			List<EventViewModel> events = new List<EventViewModel>();

			foreach (var evt in allEvents)
			{
				var et = db.Events.Find(evt.Id);

				if (et.CreatorId == usr.Id || (et.Guests != null && et.Guests.Contains(guest)))
				{
					events.Add(evt);
				}
			}

			return Json(events);
		}

		/// <summary>
		/// Get a list of events a user with given id takes part in (as a creator or guest)
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("api/Users/{userId:int}/Events")]
		[ResponseType(typeof(IEnumerable<EventViewModel>))]
		public IHttpActionResult GetUserEvents(int userId)
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			if (shiz == null)
			{
				return Unauthorized();
			}
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			var usr = db.ApplicationUsers.Find(userId);

			if(usr == null)
			{
				return NotFound();
			}

			List<EventViewModel> list = new List<EventViewModel>();

			var allEvents = GetAllEvents();

			GetGuestIds(ref allEvents);

			foreach (var evt in allEvents)
			{
				var et = db.Events.Find(evt.Id);

				if (et.CreatorId == usr.Id || evt.GuestsIds.Contains(usr.Id))
				{
					list.Add(evt);
				}
			}

			GetFutureOrNotOlderThan24(ref list);

			return Json(list);
		}

		/// <summary>
		/// Get a list of events a user with given id has created
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("api/Users/{userId:int}/EventsCreated")]
		[ResponseType(typeof(IEnumerable<EventViewModel>))]
		public IHttpActionResult GetUserEventsCreated(int userId)
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			if (shiz == null)
			{
				return Unauthorized();
			}
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			var usr = db.ApplicationUsers.Find(userId);

			if (usr == null)
			{
				return NotFound();
			}

			var allEvents = GetAllEvents();

			List<EventViewModel> events = new List<EventViewModel>();
			foreach (var evt in allEvents)
			{
				var et = db.Events.Find(evt.Id);
				if (et.CreatorId == usr.Id)
				{
					events.Add(evt);
				}
			}

			GetFutureOrNotOlderThan24(ref events);

			return Json(events);
		}

		/// <summary>
		/// Get a list of available event types
		/// </summary>
		/// <returns></returns>
		[ResponseType(typeof(string[]))]
		[Route("api/EventTypes")]
		public IHttpActionResult GetEventTypes()
		{
			return Json(System.Enum.GetNames(typeof(EventType)));
		}

		/// <summary>
		/// Update event values
		/// </summary>
		/// <param name="id"></param>
		/// <param name="et"></param>
		/// <returns></returns>
		// PUT: api/Events/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutEvent(int id, EventViewModel et)
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			if (shiz == null)
			{
				return Unauthorized();
			}
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			var evt = db.Events.Find(id);

			if(evt == null)
			{
				return NotFound();
			}

			evt.EventName = et.EventName;
			evt.StartTime = et.StartTime;
			evt.EndTime = et.EndTime;
			//evt.GuestIds = et.GuestsIds;
			evt.GuestLimit = et.GuestLimit;
			evt.AgeRestriction = et.AgeRestriction;
			evt.EventType = et.EventType;
			evt.Latitude = et.Latitude;
			evt.Longitude = et.Longitude;
			evt.LocationName = et.LocationName;
			evt.Description = et.Description;
			evt.GoogleMapsURL = et.GoogleMapsURL;
			evt.Address = et.Address;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				return NotFound();
			}

			return Json(et);
		}


		/// <summary>
		/// Delete Event
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		// DELETE: api/Events/5
		[ResponseType(typeof(Event))]
		public IHttpActionResult Delete(int id)
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			Event evt = db.Events.Find(id);
			if (evt == null)
			{
				return NotFound();
			}

			var guests = db.Guests.Where(x => x.Event.Id == evt.Id);

			evt.Guests.Clear();
			
			db.Events.Remove(evt);
			db.SaveChanges();

			return Ok(evt);
		}

		private bool EventExists(int id)
		{
			return db.Events.Count(e => e.Id == id) > 0;
		}

		private IEnumerable<EventViewModel> GetEventsByLocation(double lat, double lon, double range)
		{

			var list = new List<EventViewModel>();

			foreach (var et in db.Events)
			{
				DateTime currentDate = DateTime.Now;
				long elapsedTicksStart = currentDate.Ticks - et.StartTime;
				long elapsedTicksEnd = currentDate.Ticks - et.EndTime;
				TimeSpan elapsedSpan = new TimeSpan(elapsedTicksEnd);

				//if (elapsedTicksStart < 0 || elapsedSpan.TotalHours <= 24)
				//{
					var evt = new EventViewModel(et);

				var dist = Distance(lat, lon, evt.Latitude, evt.Longitude) * 1000;
					if (dist <= range)
					{
						list.Add(evt);
					}
				//}
			}

			return list;
		}

		private List<EventViewModel> GetAllEvents()
		{
			var events = new List<EventViewModel>();

			foreach (var et in db.Events)
			{
				long elapsedTicksStart = DateTime.Now.Ticks - et.StartTime;
				long elapsedTicksEnd = DateTime.Now.Ticks - et.EndTime;
				TimeSpan elapsedSpan = new TimeSpan(elapsedTicksEnd);

				var mdl = new EventViewModel(et);
				events.Add(mdl);
			}

			foreach (var item in events)
			{
				var creator = db.ApplicationUsers.Find(item.CreatorId);
				if(creator != null)
				{
					var rating = db.Ratings.Find(creator.RatingID);

					if (rating == null)
					{
						rating = db.Ratings.Add(new Rating());
						db.SaveChanges();
						creator.RatingID = rating.Id;
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

					item.Rating = rate;
				}

			}

			GetGuestIds(ref events);

			return events;
		}

		private List<EventViewModel> GetFutureOrNotOlderThan24(ref List<EventViewModel> events)
		{
			long currentSeconds = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			var nList = new List<EventViewModel>();

			foreach (var evt in events)
			{
				if(evt.EndTime + 86400 >= currentSeconds || evt.StartTime > currentSeconds)
				{
					nList.Add(evt);
				}
			}

			events = nList;
			return events;
		}

		private void GetGuestIds(ref List<EventViewModel> events)
		{
			foreach (var evt in events)
			{
				var et = db.Events.Find(evt.Id);
				if (et.Guests != null)
				{
					foreach (var item in et.Guests)
					{
						var guest = db.Guests.Find(item.Id);
						evt.GuestsIds.Add(guest.UserId);
					}
				}
			}
		}

		private void GetGuestIds(ref EventViewModel evt)
		{
			var et = db.Events.Find(evt.Id);
			if (et.Guests != null)
			{
				foreach (var item in et.Guests)
				{
					var guest = db.Guests.Find(item.Id);
					evt.GuestsIds.Add(guest.UserId);
				}
			}
		}

		private void GetRatersIds(ref Rating rating)
		{
			rating.UsersThatRatedIds = new List<int>();
			if(rating.UsersThatRated != null)
			{
				foreach (var item in rating.UsersThatRated)
				{
					var user = db.ApplicationUsers.Find(item.Id);
					rating.UsersThatRatedIds.Add(user.Id);
				}
			}
		}

		private double Distance(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
		{
			double theta = lon1 - lon2;
			double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
			dist = Math.Acos(dist);
			dist = rad2deg(dist);
			dist = dist * 60 * 1.1515;
			if (unit == 'K')
			{
				dist = dist * 1.609344;
			}
			else if (unit == 'N')
			{
				dist = dist * 0.8684;
			}
			return (dist);
		}

		private double deg2rad(double deg)
		{
			return (deg * Math.PI / 180.0);
		}

		private double rad2deg(double rad)
		{
			return (rad / Math.PI * 180.0);
		}
	}
}
