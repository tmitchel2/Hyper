using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using Hyper;
using HyperTests.Models;

namespace HyperTests.Controllers
{
    public class SessionController : ApiController
    {
        [AllowAnonymous]
        public HyperList<Session> Get()
        {
            if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                return new HyperList<Session> { Self = new HyperLink(GetRoute("Session")) };
            }

            return new HyperList<Session>
                {
                    Self = new HyperLink(GetRoute("Session")),
                    Items = new List<Session> { GetCurrentSession() }
                };
        }

        private Session GetCurrentSession()
        {
            const int sessionId = 1;
            return new Session
            {
                Self = new HyperLink(GetRoute("Session", sessionId.ToString())),
                Id = sessionId,
                Username = Thread.CurrentPrincipal.Identity.Name,
                User = new HyperLink(GetRoute("User", sessionId.ToString()))
            };
        }

        public HttpResponseMessage Get(int id)
        {
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated && id == 1)
            {
                return Request.CreateResponse(HttpStatusCode.OK, GetCurrentSession());
            }

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format("Session with id = {0} not found", id));
        }

        [AllowAnonymous]
        public Session Post(Session item)
        {
            var identity = new GenericIdentity(item.Username, "Basic");
            var principal = new GenericPrincipal(identity, new[] { "all" });
            Thread.CurrentPrincipal = principal;

            return new Session
                {
                    Id = 1,
                    Self = new HyperLink(GetRoute("Session", "1")),
                    User = new HyperLink(GetRoute("User", "1"))
                };
        }

        public HttpResponseMessage Delete(int id)
        {
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated && id == 1)
            {
                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(""), new string[0]);
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            return Request.CreateErrorResponse(HttpStatusCode.NotFound, string.Format("Session with id = {0} not found", id));
        }
        
        private string GetRoute(string controller, string id = null)
        {
            return new Uri(Request.RequestUri, Url.Route("DefaultApi", new { controller, id })).ToString();
        }
    }
}