using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeetMe.Models
{
	public class Location
	{
		[Key]
		public int Id { get; set; }

		public float Latitude { get; set; }
		public float Longitude { get; set; }
		public string LocationName { get; set; }
		public string Description { get; set; }
		public string GoogleMapsURL { get; set; }
	}
}