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
            return new HyperList<Message>(new HyperLink<HyperList<Message>>(GetRoute("Messages")), new Message[] {});
        }

        private string GetRoute(string controller, string id = null)
        {
            return new Uri(Request.RequestUri, Url.Route("DefaultApi", new { controller, id })).ToString();
        }
    }
}