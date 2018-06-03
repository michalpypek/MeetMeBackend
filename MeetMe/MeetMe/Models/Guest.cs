using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetMe.Models
{
	public class Guest
	{
		public int Id { get; set; }
		public User User { get; set; }
		public int UserId { get; set; }

		public virtual Event Event { get; set; }
	}
}