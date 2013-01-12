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

## Client Usage

    // Initialise client
    using (var client = new HyperClient())
    {
      // Get Api
      var api = await client.Get<Api>(@"https://api.example.com");
      
      // Login - Create a session object using HTTP POST
      var session = await api.Sessions.Post(new Session { Username = "username", Password = "password" }, client);
      
      // Logout - Delete the session object using HTTP DELETE
      await session.Delete(client);
    }
