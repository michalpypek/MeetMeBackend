using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetMe.Models
{
	public class UserOnlyName
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string FacebookURL { get; set;}
		public string PhotoURL { get; set; }
	}
}