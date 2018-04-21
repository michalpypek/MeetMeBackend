using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MeetMe.Models
{
	public class User
	{
		[Key]
		public int Id { get; set; }

		public string token { get; set; }
		public string refreshToken { get; set; }
		public long tokenExpirationDate { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
		       
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [DataType(DataType.ImageUrl)]
        public string PhotoURL { get; set; }


		public int RatingID { get; set; }

		public float UserRating { get; set;}

        public List<int> EventsCreatedIds { get; set; }
		public List<int> EventsAttendingIds { get; set; }
		public List<int> EventsAttendedIds { get; set; }
	}
}