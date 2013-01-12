using System;
using System.Diagnostics;
using System.ServiceModel;
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

namespace HyperTests
{
    public static class Application
    {
        public static void Main(string[] args)
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new ConsoleTraceListener());
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            // LOGIN 
            // http://username:password@localhost/?accept=application/json

            var config = GetConfiguration("http://localhost:80", false);
            // var config = ConfigurationHelper.GetConfiguration("https://localhost:443", true);
            var server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();
            Console.ReadKey();
        }

        public static HttpSelfHostConfiguration GetConfiguration(string url, bool requiresSSL)
        {
            var config = new HyperHttpSelfHostConfiguration(url, requiresSSL);
            if (requiresSSL)
            {
                config.ClientCredentialType = HttpClientCredentialType.Certificate;
            }

            // NOTE : Do not use username / password validation on the transport binding
            //        This is done at the server level
            config.UseAuthenticationCookie = true;
            config.AuthenticationCookieName = "hyper-auth";
            config.WebApiUserNamePasswordValidator = new HyperUserNamePasswordValidator();

            // Routes
            config.Routes.MapHttpRouteLowercase(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional, controller = "Root" });

            var types = GetTypes();
            config.Formatters.Remove(config.Formatters.JsonFormatter);
            config.Formatters.Add(new HyperJsonMediaTypeFormatter(types));
            config.Formatters.Add(new HyperXmlMediaTypeFormatter(types));
            config.MessageHandlers.Add(new RestQueryParameterHandler());
            config.MessageHandlers.Add(new AuthenticationHandler(config));
            config.Filters.Add(new BasicAuthenticationAttribute());
            config.Services.Replace(typeof(ITraceWriter), new SimpleTracer());
            return config;
        }

        public static Type[] GetTypes()
        {
            var types = new[]
                {
                    typeof(Api), typeof(HyperList<Api>), typeof(Session), typeof(HyperList<Session>), typeof(User),
                    typeof(HyperList<User>), typeof(Message), typeof(HyperList<Message>), typeof(HyperType),
                    typeof(HyperList<HyperType>), typeof(HyperList<>)
                };
            return types;
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Trace.TraceError(args.ExceptionObject.ToString());
        }
    }
}
