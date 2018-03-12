using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeetMe.Models
{
	public class User
	{
		[Key]
		public int id { get; set; }

		public string UserName { get; set; }
		public string PhotoURL { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }

		public float Rating { get; set; }
		public string Description { get; set; }
		public List<int> EventsCreatedIds { get; set; }
		public List<int> EventsAttendingIds { get; set; }
		public List<int> EventsAttendedIds { get; set; }
	}
}