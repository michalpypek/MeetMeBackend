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


		// GET: api/Events
		[ResponseType(typeof(IEnumerable<EventViewModel>))]
		public IEnumerable<EventViewModel> Get()
        {
			List<EventViewModel> events = new List<EventViewModel>();
			foreach (var et in db.Events)
			{
				events.Add(
					new EventViewModel
					{
						Id = et.Id,
						TimeCreated = et.TimeCreated,
						StartTime = et.StartTime,
						EndTime = et.EndTime,
						QrCode = et.QrCode,
						CreatorId = et.CreatorId,
						GuestsIds = et.GuestsIds,
						GuestLimit = et.GuestLimit,
						AgeRestriction = et.AgeRestriction,
						EventType = et.EventType,
						Rating = et.Rating,
						Latitude = et.Latitude,
						Longitude = et.Longitude,
						Description = et.Description,
						GoogleMapsURL = et.GoogleMapsURL
					});
			}
			return events;
        }

        // GET: api/Events/5
		[ResponseType(typeof(Event))]
        public IHttpActionResult Get(int id)
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

			return Ok(evt);
		}

		[HttpPost]
		[ResponseType(typeof(Event))]
		public IHttpActionResult Join(int id)
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			token = shiz.FirstOrDefault();
			if(!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			Event evt = db.Events.Find(id);
			if (evt == null)
			{
				return NotFound();
			}

			evt.GuestsIds.Add(authenticator.GetUserByToken(token,db).Id);
			return Ok(evt);
		}

		// GET: api/Events/?lat?lon?range
		/// <summary>
		/// Ciekawe czy to działa
		/// </summary>
		/// <param name="lat"></param>
		/// <param name="lon"></param>
		/// <returns></returns>
		[ResponseType(typeof(IEnumerable<EventViewModel>))]
		public IHttpActionResult GetEvents(double lat, double lon, double range)
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			var events = GetEventsByLocation(lat, lon, range);

			return Json(events);
		}


		// POST: api/Events
		[ResponseType(typeof(EventViewModel))]
		public IHttpActionResult Post(EventViewModel evt)
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			db.Events.Add(
				new Event
				{
					TimeCreated = evt.TimeCreated,
					StartTime = evt.StartTime,
					EndTime = evt.EndTime,	   
					QrCode = evt.QrCode,
					CreatorId = evt.CreatorId,
					GuestsIds = evt.GuestsIds,
					GuestLimit = evt.GuestLimit,
					AgeRestriction = evt.AgeRestriction,
					EventType = evt.EventType,
					Rating =  0,
					Latitude = evt.Latitude,
					Longitude = evt.Longitude,
					Description = evt.Description,
					GoogleMapsURL = evt.GoogleMapsURL
				});
			db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = evt.Id}, evt);
		}


		// GET: api/Events
		[Route ("api/Events/My")]
		[ResponseType(typeof(IEnumerable<EventViewModel>))]
		public IEnumerable<EventViewModel> GetMyEvents()
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return null;
			}

			List<EventViewModel> events = new List<EventViewModel>();
			foreach (var et in db.Events)
			{
				events.Add(
					new EventViewModel
					{
						Id = et.Id,
						TimeCreated = et.TimeCreated,
						StartTime = et.StartTime,
						EndTime = et.EndTime,
						QrCode = et.QrCode,
						CreatorId = et.CreatorId,
						GuestsIds = et.GuestsIds,
						GuestLimit = et.GuestLimit,
						AgeRestriction = et.AgeRestriction,
						EventType = et.EventType,
						Rating = et.Rating,
						Latitude = et.Latitude,
						Longitude = et.Longitude,
						Description = et.Description,
						GoogleMapsURL = et.GoogleMapsURL
					});
			}
			return events;
		}

		[ResponseType(typeof(string[]))]
		[Route("api/EventTypes")]
		public IHttpActionResult GetEventTypes()
		{
			return Json(System.Enum.GetNames(typeof(EventType)));
		}

		// PUT: api/Events/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutEvent(int id, EventViewModel et)
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var evt = db.Events.First(x => x.Id == id);

			if (id != evt.Id)
			{
				return BadRequest();
			}

			db.Entry(evt).State = EntityState.Modified;

			try
			{
				db.SaveChanges();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!EventExists(id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}

			return StatusCode(HttpStatusCode.NoContent);
		}


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

			db.Events.Remove(evt);
			db.SaveChanges();

			return Ok(evt);
		}

		[HttpPut]
		public IHttpActionResult AddUserToEvent(int eventId, List<int> userIds)
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			token = shiz.FirstOrDefault();
			if (!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			Event evt = db.Events.Find(eventId);
			if (evt == null)
			{
				return NotFound();
			}

			evt.GuestsIds.AddRange(userIds);
			db.SaveChanges();

			return Ok(evt);
		}


		private bool EventExists(int id)
		{
			return db.Events.Count(e => e.Id == id) > 0;
		}


		private IEnumerable<EventViewModel> GetEventsByLocation(double lat, double lon, double range)
		{
			var startPoint = GeographyPoint.Create(lat, lon);

			var list = new List<EventViewModel>();

			foreach (var et in db.Events)
			{
				var evt = new EventViewModel
				{
					Id = et.Id,
					TimeCreated = et.TimeCreated,
					StartTime = et.StartTime,
					EndTime = et.EndTime,
					QrCode = et.QrCode,
					CreatorId = et.CreatorId,
					GuestsIds = et.GuestsIds,
					GuestLimit = et.GuestLimit,
					AgeRestriction = et.AgeRestriction,
					EventType = et.EventType,
					Rating = et.Rating,
					Latitude = et.Latitude,
					Longitude = et.Longitude,
					Description = et.Description,
					GoogleMapsURL = et.GoogleMapsURL
				};

				var point = GeographyPoint.Create(evt.Latitude, evt.Longitude);
				if(startPoint.Distance(point) <= range)
				{
					list.Add(evt);
				}
			}

			return list;
		}
	}
}
