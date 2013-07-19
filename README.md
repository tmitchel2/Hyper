Hyper
=====

A hypermedia api framework for building fully REST compliant internet scale web applications.

## Features

- Discoverability through hypertext aware media types.
- Transport protocol independence (Tcp, Websocket, etc).
- Transfer format independence (Json, Xml, Html, etc).
- Client / Server independence enabling internet scale versioning across multiple domains.
- Stateless communication (No server side session state) to enable internet level scaling.
- Caching for improved network efficiency.

## References

- http://roy.gbiv.com/untangled/2008/rest-apis-must-be-hypertext-driven

## Example
### Client
    using (var client = new HyperClient(Application.GetTypes()))
    {
      // Get Api
      var api = await client.Get<Api>(@"http://localhost:80");
      
      // Login - Create a session object using HTTP POST
      var session = await api.Sessions.Post(new Session { Username = "username", Password = "password" }, client);
      
      // Logout - Delete the session object using HTTP DELETE
      await session.Delete(client, HttpStatusCode.Unauthorized);
    }

### Server

    // Configuration
    var config = new HyperHttpSelfHostConfiguration"http://localhost:80", false);
    config.AuthenticationCookieName = "hyper-auth";
    config.WebApiUserNamePasswordValidator = new HyperUserNamePasswordValidator();
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
    config.Filters.Add(new BasicAuthenticationAttribute(config));
    config.Services.Replace(typeof(ITraceWriter), new SimpleTracer());

    // Start server
    var server = new HttpSelfHostServer(config);
    server.OpenAsync().Wait();
    Console.ReadKey();
    
### Server Model
    [HyperContract(Name = "api", MediaType = "application/vnd.hypertests.api", Version = "1.0.0.0")]
    public class Api : IHyperEntity<Api>
    {
        [HyperLink(Rel = "self")]
        public HyperLink<Api> Self { get; set; }

        [HyperMember(Name = "name")]
        public string Name { get; set; }

        [HyperMember(Name = "version")]
        public Version Version { get; set; }

        [HyperLink(Rel = "types")]
        public HyperListLink<HyperType> Types { get; set; }
        
        [HyperLink(Rel = "sessions")]
        public HyperListLink<Session> Sessions { get; set; }

        [HyperLink(Rel = "users")]
        public HyperListLink<User> Users { get; set; }

        [HyperLink(Rel = "messages")]
        public HyperListLink<Message> Messages { get; set; }
    }

### Server Controller
    public class RootController : ApiController
    {
        [AllowAnonymous]
        public Api Get()
        {
            return new Api
                {
                    Self = new HyperLink<Api>(ControllerContext.Request.RequestUri.ToString()),
                    Name = Assembly.GetExecutingAssembly().GetName().Name,
                    Version = Assembly.GetExecutingAssembly().GetName().Version,
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
    
### Hypermedia Json
Run the HyperTests project and navigate to http://localhost/?accept=application/json

    {
        "name": "HyperTests",
        "version": "0.0.1.0",
        "self": {
            "href": "http://localhost/?accept=application/json"
        },
        "types": {
            "href": "http://localhost/type?accept=application/json"
        },
        "sessions": {
            "href": "http://localhost/session?accept=application/json"
        },
        "users": {
            "href": "http://localhost/user?accept=application/json"
        },
        "messages": {
            "href": "http://localhost/message?accept=application/json"
        }
    }

## Status

Hyper is work in progress but development is active.  Questions on usage accepted.