using System;
using System.Web.Http;
using Hyper;
using HyperTests.Models;

namespace HyperTests.Controllers
{
    public class MessageController : ApiController
    {
        public HyperList<Message> Get()
        {
            return new HyperList<Message> { Self = new HyperLink(GetRoute("Message")) };
        }

        private string GetRoute(string controller, string id = null)
        {
            return new Uri(Request.RequestUri, Url.Route("DefaultApi", new { controller, id })).ToString();
        }
    }
}