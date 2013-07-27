using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Formatting;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Web.Http.Tracing;
using Hyper;
using Hyper.Http;
using Hyper.Http.Formatting;
using Hyper.Http.Routing;
using Hyper.Http.SelfHost;
using HyperTests.IdentityModel.Selectors;
using HyperTests.Models;
using NUnit.Framework;

namespace HyperTests
{
    [TestFixture]
    public abstract class FormatterTestsBase
    {
        [Test]
        public void CanSerialiseAndDeserialiseUsingFormatter()
        {
            var task = SimpleTestAsync();
            task.Wait(TimeSpan.FromSeconds(30));
        }

        public async Task SimpleTestAsync()
        {
            var url = @"http://localhost:8352";
            var server = GetServer(url);
            server.OpenAsync().Wait();

            using (var client = GetClient())
            {
                // Get the app api
                var api = await client.Get<Api>(url);

                // Login
                var session = await api.Sessions.Post(new Session { Username = "username", Password = "password" }, client);

                // Logout
                await session.Delete(client, HttpStatusCode.Unauthorized);
            }

            await server.CloseAsync();
        }

        protected abstract MediaTypeFormatter GetHyperMediaTypeFormatter(IEnumerable<Type> types);

        private HttpSelfHostServer GetServer(string url)
        {
            var config = new HyperHttpSelfHostConfiguration(url, false);

            // NOTE : Do not use username / password validation on the transport binding
            //        This is done at the server level
            config.AuthenticationCookieName = "hyper-auth";
            config.WebApiUserNamePasswordValidator = new HyperUserNamePasswordValidator();

            // Routes
            config.Routes.MapHttpRouteLowercase(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional, controller = "Root" });

            config.Formatters.Remove(config.Formatters.JsonFormatter);
            config.Formatters.Add(GetHyperMediaTypeFormatter(Application.GetTypes()));
            config.MessageHandlers.Add(new RestQueryParameterHandler());
            config.MessageHandlers.Add(new AuthenticationHandler(config));
            config.Filters.Add(new BasicAuthenticationAttribute(config));
            config.Services.Replace(typeof(ITraceWriter), new SimpleTracer());

            return new HttpSelfHostServer(config);
        }
        
        private HyperClient GetClient()
        {
            var config = new HyperClientConfiguration();
            config.Formatters.Add(GetHyperMediaTypeFormatter(Application.GetTypes()));
            config.DefaultFormatter = config.Formatters[0];
            return new HyperClient(config);
        }
    }
}