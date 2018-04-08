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
		public int id { get; set; }  
        
        [Required, StringLength(50)]
        public string UserName { get; set; }
		       
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [DataType(DataType.ImageUrl)]
        public string PhotoURL { get; set; }

        public Rating Rating { get; set; }
        public List<int> EventsCreatedIds { get; set; }
		public List<int> EventsAttendingIds { get; set; }
		public List<int> EventsAttendedIds { get; set; }
	}
    public class UserDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}