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
			if (db.ApplicationUsers.Any(x => x.Token.Equals(token)))
			{
				var usr = db.ApplicationUsers.First(x => x.Token.Equals(token));

				return usr != null;
			}
			return false;
		}

		public User GetUserByToken (string token, ApplicationDbContext db)
		{
			if (IsTokenOk(token, db))
			{
				var usr = db.ApplicationUsers.First(x => x.Token.Equals(token));
				return usr;
			}
			return null;
		}

	}
}