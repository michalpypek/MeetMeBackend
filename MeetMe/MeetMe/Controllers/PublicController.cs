using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace MeetMe.Controllers
{
    public class PublicController : ApiController
    {
		[HttpGet]
		[Route("api/Ping")]
		public IHttpActionResult Ping()
		{
			return Ok();
		}
    }
}
