using System;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using Hyper.Http.SelfHost;

namespace Hyper.Http.Controllers
{
    /// <summary>
    /// SimpleHyperApiControllerActionSelector class.
    /// </summary>
    public class SimpleHyperApiControllerActionSelector : DelegatingApiControllerActionSelector
    {
        private readonly HyperHttpSelfHostConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleHyperApiControllerActionSelector" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public SimpleHyperApiControllerActionSelector(HyperHttpSelfHostConfiguration configuration)
            : base(new ApiControllerActionSelector())
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Selects the action for the controller.
        /// </summary>
        /// <param name="controllerContext">The context of the controller.</param>
        /// <returns>
        /// The action for the controller.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            if (controllerContext.RouteData.Values.ContainsKey("controller1"))
            {
                var method = controllerContext.Request.Method.Method.ToUpperInvariant();

                var methodInfo = controllerContext
                    .Controller
                    .GetType()
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .First(m => m.Name.ToUpperInvariant() == method);

                return new ReflectedHttpActionDescriptor(controllerContext.ControllerDescriptor, methodInfo);
            }

            var action = base.SelectAction(controllerContext);
            return action;
        }

        /// <summary>
        /// Returns a map, keyed by action string, of all <see cref="T:System.Web.Http.Controllers.HttpActionDescriptor" /> that the selector can select.  This is primarily called by <see cref="T:System.Web.Http.Description.IApiExplorer" /> to discover all the possible actions in the controller.
        /// </summary>
        /// <param name="controllerDescriptor">The controller descriptor.</param>
        /// <returns>
        /// A map of <see cref="T:System.Web.Http.Controllers.HttpActionDescriptor" /> that the selector can select, or null if the selector does not have a well-defined mapping of <see cref="T:System.Web.Http.Controllers.HttpActionDescriptor" />.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override ILookup<string, HttpActionDescriptor> GetActionMapping(HttpControllerDescriptor controllerDescriptor)
        {
            var mapping = GetActionMapping(controllerDescriptor);
            return mapping;
        }
    }
}
