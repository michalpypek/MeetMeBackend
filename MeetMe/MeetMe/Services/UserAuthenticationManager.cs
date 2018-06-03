using MeetMe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

		public User AuthorizeGetUser(HttpRequestMessage request, ApplicationDbContext db)
		{
			string token;
			IEnumerable<string> shiz;
			request.Headers.TryGetValues("Authorization", out shiz);
			if (shiz == null)
			{
				return null;
			}
			token = shiz.FirstOrDefault();
			if (IsTokenOk(token, db))
			{
				return null;
			}

			return GetUserByToken(token,db);
		}

	}
}