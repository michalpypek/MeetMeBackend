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
		public int id { get; set; }

		public DateTime TimeCreated { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

		public QrCode QrCode { get; set; }

		public int CreatorId { get; set; }
		public List<int> GuestsIds { get; set; }
		public int GuestLimit { get; set; }
		public AgeRestriction AgeRestriction { get; set; }
		public bool MyProperty { get; set; }
		public EventType EventType { get; set; }
		public float rating { get; set; }
		public Location Location { get; set; }
	}
}