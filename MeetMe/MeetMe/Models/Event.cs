using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeetMe.Models
{
	public class Event
	{
		[Key]
		public int? Id { get; set; }

		public string EventName { get; set; }

        public long TimeCreated { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }

        public QrCode QrCode { get; set; }

        public int CreatorId { get; set; }

		public List<int> GuestsIds { get; set; }
		//public List<User> Guests { get; set; }
		public virtual ICollection<Guest> Guests { get; set; }
		public int GuestLimit { get; set; }
        public AgeRestriction AgeRestriction { get; set; }
        public EventType EventType { get; set; }

		public float Latitude { get; set; }
		public float Longitude { get; set; }
		public string LocationName { get; set; }
		public string Description { get; set; }
		public string GoogleMapsURL { get; set; }
		public string Address { get; set; }


		public Event()
		{

		}

		public Event(EventViewModel evt)
		{
			EventName = evt.EventName;
			TimeCreated = evt.TimeCreated;
			StartTime = evt.StartTime;
			EndTime = evt.EndTime;
			QrCode = evt.QrCode;
			GuestsIds = new List<int>();
			Guests = new List<Guest>();
			GuestLimit = evt.GuestLimit;
			AgeRestriction = evt.AgeRestriction;
			EventType = evt.EventType;
			Latitude = evt.Latitude;
			Longitude = evt.Longitude;
			LocationName = evt.LocationName;
			Description = evt.Description;
			GoogleMapsURL = evt.GoogleMapsURL;
			Address = evt.Address;
		}
	}
}