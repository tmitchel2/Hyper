using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Hyper.Http
{
    /// <summary>
    /// AuthenticationHandler class.
    /// </summary>
    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly IAuthenticationConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationHandler" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public AuthenticationHandler(IAuthenticationConfiguration configuration)
        {
            _configuration = configuration;
        }

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
            // Get usernamepassword from http basic auth
            var usernamePassword = string.Empty;
            var authHeader = request.Headers.Authorization;
            if (authHeader != null)
            {
                var encodedUsernamePassword = authHeader.Parameter.Trim();
                usernamePassword = Encoding.ASCII.GetString(Convert.FromBase64String(encodedUsernamePassword));
            }
            
            // Get username passord from cookie
            var cookies = new List<CookieHeaderValue>();
            if (_configuration.UseAuthenticationCookie)
            {
                cookies = request.Headers.GetCookies(_configuration.AuthenticationCookieName).ToList();
                if (cookies.Count > 0)
                {
                    usernamePassword = cookies[0][_configuration.AuthenticationCookieName].Value;
                }
            }

            // Set current principal
            if (!string.IsNullOrWhiteSpace(usernamePassword))
            {
                var parts = usernamePassword.Split(":".ToCharArray());
                var username = parts[0];
                var password = parts[1];

                try
                {
                    _configuration.WebApiUserNamePasswordValidator.Validate(username, password);

                    var identity = new GenericIdentity(username, "Basic");
                    var principal = new GenericPrincipal(identity, new[] { "all" });
                    Thread.CurrentPrincipal = principal;
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.User = principal;
                    }
                }
                catch (SecurityTokenException)
                {
                }
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (_configuration.UseAuthenticationCookie)
            {
                if (cookies.Count > 0 && !Thread.CurrentPrincipal.Identity.IsAuthenticated)
                {
                    foreach (var cookie in cookies)
                    {
                        cookie.Domain = @".localhost";
                        cookie.Path = @"/";
                        cookie.Expires = DateTimeOffset.Now.AddDays(-7);
                    }

                    response.Headers.AddCookies(cookies);
                }
                else if (cookies.Count == 0 && Thread.CurrentPrincipal.Identity.IsAuthenticated)
                {
                    // Add new cookie
                    response.Headers.AddCookies(
                        new[]
                            {
                                new CookieHeaderValue(_configuration.AuthenticationCookieName, usernamePassword)
                                    {
                                        Domain = @".localhost", Path = @"/", Expires = DateTimeOffset.Now.AddDays(7) 
                                    }
                            });
                }
            }

            return response;
        }
    }
}