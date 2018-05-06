using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MeetMe.Models
{
    public class Rating
    {
		[Key]
        public int Id { get; set; }

        public int NumberOfRates { get; set; }
        public float Sum { get; set; }
    }
}