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

			var evtDb = db.Events.Add(
				new Event
				{
					TimeCreated = evt.TimeCreated,
					StartTime = evt.StartTime,
					EndTime = evt.EndTime,
					QrCode = evt.QrCode,
					CreatorId = usr.Id,
					GuestsIds = new List<int>(),
					GuestLimit = evt.GuestLimit,
					AgeRestriction = evt.AgeRestriction,
					EventType = evt.EventType,
					Latitude = evt.Latitude,
					Longitude = evt.Longitude,
					Description = evt.Description,
					GoogleMapsURL = evt.GoogleMapsURL
				});
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

			List<EventViewModel> events = new List<EventViewModel>();

			foreach (var et in db.Events)
			{
				DateTime currentDate = DateTime.Now;
				long elapsedTicksStart = currentDate.Ticks - et.StartTime;
				long elapsedTicksEnd = currentDate.Ticks - et.EndTime;
				TimeSpan elapsedSpan = new TimeSpan(elapsedTicksEnd);

				if (elapsedTicksStart < 0)
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
							Latitude = et.Latitude,
							Longitude = et.Longitude,
							Description = et.Description,
							GoogleMapsURL = et.GoogleMapsURL
						});
				}
			}
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

			List<EventViewModel> events = new List<EventViewModel>();

			foreach (var et in db.Events)
			{
				DateTime currentDate = DateTime.Now;
				long elapsedTicksStart = currentDate.Ticks - et.StartTime;
				long elapsedTicksEnd = currentDate.Ticks - et.EndTime;
				TimeSpan elapsedSpan = new TimeSpan(elapsedTicksEnd);

				if (elapsedTicksStart < 0 || elapsedSpan.TotalHours <= 24)
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
							Latitude = et.Latitude,
							Longitude = et.Longitude,
							Description = et.Description,
							GoogleMapsURL = et.GoogleMapsURL
						});
				}
			}
			return Json(events);
		}

		// GET: api/Events/5
		/// <summary>
		/// Get a specific Event, by ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[ResponseType(typeof(Event))]
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

			return Ok(evt);
		}

		/// <summary>
		/// Join an event
		/// </summary>
		/// <param name="eventId">Id of the event to join</param>
		/// <returns></returns>
		[HttpPost]
		[Route("api/Events/{eventId:int}/Join")]
		[ResponseType(typeof(Event))]
		public IHttpActionResult Join(int eventId)
		{
			string token;
			IEnumerable<string> shiz;
			Request.Headers.TryGetValues("Authorization", out shiz);
			if (shiz == null)
			{
				return Unauthorized();
			}
			token = shiz.FirstOrDefault();
			if(!authenticator.IsTokenOk(token, db))
			{
				return Unauthorized();
			}

			var usr = authenticator.GetUserByToken(token, db);

			Event evt = db.Events.Find(eventId);
			if (evt == null)
			{
				return NotFound();
			}

			if (evt.GuestsIds == null)
			{
				evt.GuestsIds = new List<int>();
			}

			if(evt.GuestsIds.Count >= evt.GuestLimit)
			{
				return Json("Event limit reached");
			}

			if (!evt.GuestsIds.Contains(usr.Id))
			{
				evt.GuestsIds.Add(usr.Id);
			}

			db.SaveChanges();
			return Ok(evt);
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

			var events = GetEventsByLocation(lat, lon, range);

			return Json(events);
		}

		/// <summary>
		/// Get a list of events current user takes part in (as a creator or guest)
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

			List<EventViewModel> events = new List<EventViewModel>();
			foreach (var et in db.Events)
			{
				if (et.CreatorId == usr.Id || (et.GuestsIds != null && et.GuestsIds.Contains(usr.Id)))
				{
					DateTime currentDate = DateTime.Now;
					long elapsedTicksStart = currentDate.Ticks - et.StartTime;
					long elapsedTicksEnd = currentDate.Ticks - et.EndTime;
					TimeSpan elapsedSpan = new TimeSpan(elapsedTicksEnd);

					if (elapsedTicksStart < 0 || elapsedSpan.TotalHours <= 24)
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
							Latitude = et.Latitude,
							Longitude = et.Longitude,
							Description = et.Description,
							GoogleMapsURL = et.GoogleMapsURL
						});
					}
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

			foreach (var et in db.Events)
			{
				if (et.CreatorId == usr.Id  || (et.GuestsIds != null && et.GuestsIds.Contains(usr.Id)))
				{
					DateTime currentDate = DateTime.Now;
					long elapsedTicksStart = currentDate.Ticks - et.StartTime;
					long elapsedTicksEnd = currentDate.Ticks - et.EndTime;
					TimeSpan elapsedSpan = new TimeSpan(elapsedTicksEnd);

					if (elapsedTicksStart < 0 || elapsedSpan.TotalHours <= 24)
					{
						list.Add(
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
							Latitude = et.Latitude,
							Longitude = et.Longitude,
							Description = et.Description,
							GoogleMapsURL = et.GoogleMapsURL
						});
					}
				}
			}
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

			List<EventViewModel> events = new List<EventViewModel>();
			foreach (var et in db.Events)
			{
				if (et.CreatorId == usr.Id)
				{
					DateTime currentDate = DateTime.Now;
					long elapsedTicksStart = currentDate.Ticks - et.StartTime;
					long elapsedTicksEnd = currentDate.Ticks - et.EndTime;
					TimeSpan elapsedSpan = new TimeSpan(elapsedTicksEnd);

					if (elapsedTicksStart < 0 || elapsedSpan.TotalHours <= 24)
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
							Latitude = et.Latitude,
							Longitude = et.Longitude,
							Description = et.Description,
							GoogleMapsURL = et.GoogleMapsURL
						});
					}
				}
			}
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

			evt.StartTime = et.StartTime;
			evt.EndTime = et.EndTime;
			evt.GuestsIds = et.GuestsIds;
			evt.GuestLimit = et.GuestLimit;
			evt.AgeRestriction = et.AgeRestriction;
			evt.EventType = et.EventType;
			evt.Latitude = et.Latitude;
			evt.Longitude = et.Longitude;
			evt.LocationName = et.LocationName;
			evt.Description = et.Description;
			evt.GoogleMapsURL = et.GoogleMapsURL;

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

			db.Events.Remove(evt);
			db.SaveChanges();

			return Ok(evt);
		}

		/// <summary>
		/// Add a list of users to event
		/// </summary>
		/// <param name="eventId"></param>
		/// <param name="userIds"></param>
		/// <returns></returns>
		[HttpPut]
		public IHttpActionResult AddUserToEvent(int eventId, List<int> userIds)
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

			Event evt = db.Events.Find(eventId);
			if (evt == null)
			{
				return NotFound();
			}

			if(evt.GuestsIds == null)
			{
				evt.GuestsIds = new List<int>();
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
				DateTime currentDate = DateTime.Now;
				long elapsedTicksStart = currentDate.Ticks - et.StartTime;
				long elapsedTicksEnd = currentDate.Ticks - et.EndTime;
				TimeSpan elapsedSpan = new TimeSpan(elapsedTicksEnd);

				if (elapsedTicksStart < 0 || elapsedSpan.TotalHours <= 24)
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
						Latitude = et.Latitude,
						Longitude = et.Longitude,
						Description = et.Description,
						GoogleMapsURL = et.GoogleMapsURL
					};

					var point = GeographyPoint.Create(evt.Latitude, evt.Longitude);
					if (startPoint.Distance(point) <= range)
					{
						list.Add(evt);
					}
				}
			}

			return list;
		}
	}
}
