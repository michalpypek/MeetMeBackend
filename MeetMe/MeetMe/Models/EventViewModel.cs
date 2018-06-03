using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetMe.Models
{
	public class EventViewModel
	{
		public int? Id { get; set; }
		public string EventName { get; set; }
		public long TimeCreated { get; set; }
		public long StartTime { get; set; }
		public long EndTime { get; set; }

		public QrCode QrCode { get; set; }

		public int CreatorId { get; set; }
		public List<int> GuestsIds { get; set; }
		public int GuestLimit { get; set; }
		public AgeRestriction AgeRestriction { get; set; }
		public EventType EventType { get; set; }

		public float Latitude { get; set; }
		public float Longitude { get; set; }
		public string LocationName { get; set; }
		public string Description { get; set; }
		public string GoogleMapsURL { get; set; }
		public string Address { get; set; }

		public float Rating { get; set; }

		public EventViewModel() { }

		public EventViewModel (Event et)
		{
			this.Id = et.Id;
			this.EventName = et.EventName;
			this.TimeCreated = et.TimeCreated;
			this.StartTime = et.StartTime;
			this.EndTime = et.EndTime;
			this.QrCode = et.QrCode;
			this.CreatorId = et.CreatorId;
			this.GuestsIds = new List<int>();
			this.GuestLimit = et.GuestLimit;
			this.AgeRestriction = et.AgeRestriction;
			this.EventType = et.EventType;
			this.Latitude = et.Latitude;
			this.Longitude = et.Longitude;
			this.LocationName = et.LocationName;
			this.Description = et.Description;
			this.GoogleMapsURL = et.GoogleMapsURL;
			this.Address = et.Address;
		}

	}
}