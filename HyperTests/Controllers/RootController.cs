using System;
using System.Reflection;
using System.Web.Http;
using Hyper;
using HyperTests.Models;

namespace HyperTests.Controllers
{
    /// <summary>
    /// HomeController ckass.
    /// </summary>
    public class RootController : ApiController
    {
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public Api Get()
        {
            return new Api
                {
                    Self = new HyperLink<Api>(ControllerContext.Request.RequestUri.ToString()),
                    Name = Assembly.GetExecutingAssembly().GetName().Name,
                    Types = new HyperListLink<HyperType>(GetRoute("Type")),
                    Sessions = new HyperListLink<Session>(GetRoute("Session")),
                    Users = new HyperListLink<User>(GetRoute("User")),
                    Messages = new HyperListLink<Message>(GetRoute("Message"))
                };
        }

        private string GetRoute(string controller, string id = null)
        {
            return new Uri(Request.RequestUri, Url.Route("DefaultApi", new { controller, id })).ToString();
        }
    }
}