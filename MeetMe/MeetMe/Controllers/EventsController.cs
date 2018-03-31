using MeetMe.Models;
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

		// GET: api/Events
		public IEnumerable<Event> Get()
        {
			return db.Events;
        }

        // GET: api/Events/5
        public IHttpActionResult Get(int id)
        {
			Event evt = db.Events.Find(id);
			if (evt == null)
			{
				return NotFound();
			}

			return Ok(evt);
		}

		// GET: api/Events/?lat?lon?range
		/// <summary>
		/// Ciekawe czy to działa
		/// </summary>
		/// <param name="lat"></param>
		/// <param name="lon"></param>
		/// <returns></returns>
		[ResponseType(typeof(IEnumerable<Event>))]
		public IHttpActionResult GetEvents(double lat, double lon, double range)
		{
			var toilets = GetEventsByLocation(lat, lon, range);

			return Json(toilets);
		}


		// POST: api/Events
		[ResponseType(typeof(Event))]
		public IHttpActionResult Post(Event evt)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			db.Events.Add(evt);
			db.SaveChanges();

			return CreatedAtRoute("DefaultApi", new { id = evt.id}, evt);
		}


		// PUT: api/Events/5
		[ResponseType(typeof(void))]
		public IHttpActionResult PutToilet(int id, Event evt)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (id != evt.id)
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
			Event evt = db.Events.Find(id);
			if (evt == null)
			{
				return NotFound();
			}

			db.Events.Remove(evt);
			db.SaveChanges();

			return Ok(evt);
		}



		private bool EventExists(int id)
		{
			return db.Events.Count(e => e.id == id) > 0;
		}


		private IEnumerable<Event> GetEventsByLocation(double lat, double lon, double range)
		{
			var startPoint = GeographyPoint.Create(lat, lon);

			var list = new List<Event>();

			foreach (var evt in db.Events)
			{
				if (evt.Location != null)
				{
					var point = GeographyPoint.Create(evt.Location.Latitude, evt.Location.Longitude);
					if(startPoint.Distance(point) <= range)
					{
						list.Add(evt);
					}
				}
			}

			return list;
		}
	}
}
