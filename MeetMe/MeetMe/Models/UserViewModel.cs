using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetMe.Models
{
	public class UserViewModel
	{
		public int Id { get; set; }

		public string Token { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }

		public string Email { get; set; }

		public string PhoneNumber { get; set; }

		public string Description { get; set; }

		public string PhotoURL { get; set; }

		public float Rating { get; set; }
	}
}