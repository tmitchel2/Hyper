Hyper
=====

A hypermedia api framework for building fully REST compliant internet scale web applications.

## Features

- Discoverability through hypertext aware media types.
- Transfer protocol independence (Http, Ftp, etc).
- Transfer format independence (Json, Xml, Html, etc).
- Client / Server independence enabling internet scale versioning across multiple domains.
- Stateless communication (No server side session state) to enable internet level scaling.
- Caching for improved network efficiency.

## References

- http://roy.gbiv.com/untangled/2008/rest-apis-must-be-hypertext-driven

## Example
### Client
    using (var client = new HyperClient())
    {
      // Get Api
      var api = await client.Get<Api>(@"http://api.example.com");
      
      // Login - Create a session object using HTTP POST
      var session = await api.Sessions.Post(new Session { Username = "username", Password = "password" }, client);
      
      // Logout - Delete the session object using HTTP DELETE
      await session.Delete(client);
    }

### Server
    var config = new HyperHttpSelfHostConfiguration(@"http://api.example.com", false);
    config.UseAuthenticationCookie = true;
    config.AuthenticationCookieName = "hyper-auth";
    config.WebApiUserNamePasswordValidator = new HyperUserNamePasswordValidator();

    // Init routes
    config.Routes.MapHttpRouteLowercase(
        name: "DefaultApi",
        routeTemplate: "{controller}/{id}",
        defaults: new { id = RouteParameter.Optional, controller = "Root" });

    // Init media type formatters
    var types = GetTypes();
    config.Formatters.Remove(config.Formatters.JsonFormatter);
    config.Formatters.Add(new HyperJsonMediaTypeFormatter(types));
    config.Formatters.Add(new HyperXmlMediaTypeFormatter(types));
    
    // Init message handlers and filters
    config.MessageHandlers.Add(new RestQueryParameterHandler());    
    config.MessageHandlers.Add(new AuthenticationHandler(config));
    config.Filters.Add(new BasicAuthenticationAttribute());
    
    // Start server
    var server = new HttpSelfHostServer(config);
    server.OpenAsync().Wait();
    Console.ReadKey();

### Server Model
    [HalContract]
    public class Api : IHalEntity
    {
        [HalLink(Rel = "self")]
        public HalLink Self { get; set; }

        [HalMember(Name = "name")]
        public string Name { get; set; }

        [HalMember(Name = "version")]
        public Version Version { get; set; }

        [HalLink(Rel = "types")]
        public HalLinkList<HalType> Types { get; set; }
        
        [HalLink(Rel = "sessions")]
        public HalLinkList<Session> Sessions { get; set; }

        [HalLink(Rel = "users")]
        public HalLinkList<User> Users { get; set; }
    }
    
### Server Controller
    public class RootController : ApiController
    {
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
