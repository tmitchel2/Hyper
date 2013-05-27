using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Tracing;
using Hyper.Http.SelfHost;

namespace Hyper.Http
{
    /// <summary>
    /// BasicAuthenticationAttribute class.
    /// </summary>
    public class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthenticationAttribute" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public BasicAuthenticationAttribute(HyperHttpSelfHostConfiguration configuration)
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
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Configuration.Services.GetTraceWriter().Info(actionContext.Request, "BasicAuthenticationAttribute", "Starting authentication");
            
            if (!SkipAuthorization(actionContext))
            {
                // If no authorization header is supplied or the current user is not authenticated then fail
                // TODO : actionContext.Request.Headers.Authorization == null ||
                if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
                {
                    Configuration.Services.GetTraceWriter().Info(actionContext.Request, "BasicAuthenticationAttribute", "User is NOT authenticated");
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    actionContext.Response.Headers.WwwAuthenticate.Add(
                        new AuthenticationHeaderValue("Basic", "realm=\"nakack\""));
                    return;
                }
            }
            else
            {
                Configuration.Services.GetTraceWriter().Info(actionContext.Request, "BasicAuthenticationAttribute", "Skipping authentication because of [AllowAnonymousAttribute]");
            }

            Configuration.Services.GetTraceWriter().Info(actionContext.Request, "BasicAuthenticationAttribute", "User is authenticated");
                    base.OnActionExecuting(actionContext);
        }

        /// <summary>
        /// Occurs after the action method is invoked.
        /// </summary>
        /// <param name="actionExecutedContext">The action executed context.</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            if (actionExecutedContext.Response == null && 
                actionExecutedContext.Response.Headers.WwwAuthenticate == null)
            {
                actionExecutedContext.Response.Headers.WwwAuthenticate.Add(
                    new AuthenticationHeaderValue("Basic", "realm=\"nakack\""));
            }
        }

        /// <summary>
        /// Skips the authorization.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns></returns>
        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            return actionContext
                .ActionDescriptor
                .GetCustomAttributes<AllowAnonymousAttribute>()
                .Any();
        }
    }
}