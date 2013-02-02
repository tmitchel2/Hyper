using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Tracing;
using Hyper.Http.SelfHost;

namespace Hyper.Http
{
    /// <summary>
    /// AuthenticationHandler class.
    /// </summary>
    public class AuthenticationHandler : DelegatingHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationHandler" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public AuthenticationHandler(HyperHttpSelfHostConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public HyperHttpSelfHostConfiguration Configuration { get; private set; }

        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.
        /// </returns>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Authenticate
            string username;
            string password;
            Guid sessionId;
            if (TryGetUsernamePassword(request, out username, out password, out sessionId))
            {
                Authenticate(request, username, password);
                SetSessionId(request, sessionId);
            }

            // Process
            var response = await base.SendAsync(request, cancellationToken);

            // Response
            UpdateCookies(request, response, username, password, sessionId);
            return response;
        }

        /// <summary>
        /// Froms the encoded username password.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        private static void FromEncodedUsernamePassword(string value, out string username, out string password)
        {
            var usernamePassword = Encoding.ASCII.GetString(Convert.FromBase64String(value));
            var parts = usernamePassword.Split(":".ToCharArray());
            username = parts[0];
            password = parts[1];
        }

        /// <summary>
        /// To the encoded username password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>Encoded username and password.</returns>
        private static string ToEncodedUsernamePassword(string username, string password)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
        }

        /// <summary>
        /// Sets the session id.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="sessionId">The session id.</param>
        private void SetSessionId(HttpRequestMessage request, Guid sessionId)
        {
            request.Properties[Configuration.SessionCookieName] = sessionId;
        }

        /// <summary>
        /// Gets the username password.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="sessionId">The session id.</param>
        /// <returns></returns>
        private bool TryGetUsernamePassword(HttpRequestMessage request, out string username, out string password, out Guid sessionId)
        {
            // Get usernamepassword from http basic auth
            username = null;
            password = null;
           
            if (request.Headers.Authorization != null)
            {
                FromEncodedUsernamePassword(request.Headers.Authorization.Parameter.Trim(), out username, out password);
            }

            // Get username passord from cookie
            if (username == null && 
                password == null && 
                request.Headers.GetCookies(Configuration.AuthenticationCookieName).Any())
            {
                var authCookie =
                    request.Headers.GetCookies(Configuration.AuthenticationCookieName)
                    .Single()[Configuration.AuthenticationCookieName];

                FromEncodedUsernamePassword(authCookie.Value, out username, out password);
            }

            if (request.Headers.GetCookies(Configuration.SessionCookieName).Any())
            {
                var sessionIdCookie =
                    request.Headers.GetCookies(Configuration.SessionCookieName)
                    .Single()[Configuration.SessionCookieName];

                // Extract existing session id
                sessionId = Guid.Parse(sessionIdCookie.Value);
            }
            else
            {
                // Create new session id
                sessionId = Guid.NewGuid();
            }

            return username != null && password != null;
        }

        /// <summary>
        /// Authenticates the specified username password.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        private void Authenticate(HttpRequestMessage request, string username, string password)
        {
            try
            {
                Configuration.WebApiUserNamePasswordValidator.Validate(username, password);
            }
            catch (SecurityTokenException)
            {
                Configuration.Services.GetTraceWriter().Warn(request, "AuthenticationHandler", "User {0} failed authentication", username);
            }

            var identity = new GenericIdentity(username, "Basic");
            var principal = new GenericPrincipal(identity, new[] { "all" });
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        /// <summary>
        /// Updates the cookies.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <param name="username">The username password.</param>
        /// <param name="password">The password.</param>
        /// <param name="sessionId">The session id.</param>
        private void UpdateCookies(HttpRequestMessage request, HttpResponseMessage response, string username, string password, Guid sessionId)
        {
            if (request.Headers.GetCookies(Configuration.AuthenticationCookieName).Any() && 
                !Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                // NOTE can't use domain on localhost and chrome
                // cookie.Domain = @".localhost.com";
                var authCookie = request.Headers.GetCookies(Configuration.AuthenticationCookieName).First();
                authCookie.Path = @"/";
                authCookie.Expires = DateTime.Now;
                response.Headers.AddCookies(new[] { authCookie });
            }
            else if (!request.Headers.GetCookies(Configuration.AuthenticationCookieName).Any() && 
                Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                var authCookie = new CookieHeaderValue(
                    Configuration.AuthenticationCookieName, ToEncodedUsernamePassword(username, password))
                    {
                        // NOTE can't use domain on localhost and chrome
                        // Domain = @".localhost.com", 
                        Path = @"/",
                        Expires = DateTime.Now.AddDays(7)
                    };
                
                // Add new cookie
                response.Headers.AddCookies(new[] { authCookie });
            }

            if (request.Headers.GetCookies(Configuration.SessionCookieName).Any() &&
                !Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                // NOTE can't use domain on localhost and chrome
                // cookie.Domain = @".localhost.com";
                var sessionCookie = request.Headers.GetCookies(Configuration.SessionCookieName).First();
                sessionCookie.Path = @"/";
                sessionCookie.Expires = DateTime.Now;
                response.Headers.AddCookies(new[] { sessionCookie });
            }
            else if (!request.Headers.GetCookies(Configuration.SessionCookieName).Any() &&
                Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                var sessionCookie = new CookieHeaderValue(Configuration.SessionCookieName, sessionId.ToString())
                {
                    // NOTE can't use domain on localhost and chrome
                    // Domain = @".localhost.com", 
                    Path = @"/",
                    Expires = DateTime.Now.AddDays(7)
                };

                // Add new cookie
                response.Headers.AddCookies(new[] { sessionCookie });
            }
        }
    }
}