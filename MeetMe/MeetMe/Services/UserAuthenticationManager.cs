using MeetMe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetMe.Services
{
	public class UserAuthenticationManager
	{
		public bool IsTokenOk(string token, ApplicationDbContext db)
		{
			var usr = db.ApplicationUsers.First(x => x.token.Equals(token));

			return usr != null;
		}

		public User GetUserByToken (string token, ApplicationDbContext db)
		{
			var usr = db.ApplicationUsers.First(x => x.token.Equals(token));
			return usr;
		}

	}
}