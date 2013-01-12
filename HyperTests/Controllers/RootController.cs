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
                    Self = new HyperLink(ControllerContext.Request.RequestUri.ToString()),
                    Name = Assembly.GetExecutingAssembly().GetName().Name,
                    Version = Assembly.GetExecutingAssembly().GetName().Version,
                    Types = new HyperLinkList<HyperType>(GetRoute("Type")),
                    Sessions = new HyperLinkList<Session>(GetRoute("Session")),
                    Users = new HyperLinkList<User>(GetRoute("User")),
                    Messages = new HyperLinkList<Message>(GetRoute("Message"))
                };
        }

        private string GetRoute(string controller, string id = null)
        {
            return new Uri(Request.RequestUri, Url.Route("DefaultApi", new { controller, id })).ToString();
        }
    }
}