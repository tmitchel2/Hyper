using System;
using System.Web.Http;
using Hyper;
using HyperTests.Models;

namespace HyperTests.Controllers
{
    public class UserController : ApiController
    {
        [AllowAnonymous]
        public HyperList<User> Get()
        {
            return new HyperList<User>(new HyperLink<HyperList<User>>(GetRoute("User")), new User[] { });
        }

        private string GetRoute(string controller, string id = null)
        {
            return new Uri(Request.RequestUri, Url.Route("DefaultApi", new { controller, id })).ToString();
        }
    }
}