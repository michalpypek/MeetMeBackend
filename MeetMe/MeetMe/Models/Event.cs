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

        public long TimeCreated { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }

        public QrCode QrCode { get; set; }

        public int CreatorId { get; set; }
        public List<int> GuestsIds { get; set; }
        public int GuestLimit { get; set; }
        public AgeRestriction AgeRestriction { get; set; }
        public EventType EventType { get; set; }
        public float Rating { get; set; }

		public float Latitude { get; set; }
		public float Longitude { get; set; }
		public string LocationName { get; set; }
		public string Description { get; set; }
		public string GoogleMapsURL { get; set; }
	}
}