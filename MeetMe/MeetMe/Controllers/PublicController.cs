using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MeetMe.Controllers
{
	public class PublicController : ApiController
	{
		[HttpGet]
		[Route("api/Public/Ping")]
		public IHttpActionResult Ping()
		{
			return Ok();
		}

	}

}
