using System.Linq;
using System.Web.Http.Controllers;

namespace Hyper.Http.Controllers
{
    /// <summary>
    /// DelegatingApiControllerActionSelector class.
    /// </summary>
    public class DelegatingApiControllerActionSelector : IHttpActionSelector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatingApiControllerActionSelector" /> class.
        /// </summary>
        /// <param name="innerActionSelector">The inner action selector.</param>
        public DelegatingApiControllerActionSelector(IHttpActionSelector innerActionSelector)
        {
            InnerActionSelector = innerActionSelector;
        }

        /// <summary>
        /// Gets the inner action selector.
        /// </summary>
        /// <value>
        /// The inner action selector.
        /// </value>
        public IHttpActionSelector InnerActionSelector { get; private set; }

        /// <summary>
        /// Selects the action for the controller.
        /// </summary>
        /// <param name="controllerContext">The context of the controller.</param>
        /// <returns>
        /// The action for the controller.
        /// </returns>
        public virtual HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            return InnerActionSelector.SelectAction(controllerContext);
        }

        /// <summary>
        /// Returns a map, keyed by action string, of all <see cref="T:System.Web.Http.Controllers.HttpActionDescriptor" /> that the selector can select.  This is primarily called by <see cref="T:System.Web.Http.Description.IApiExplorer" /> to discover all the possible actions in the controller.
        /// </summary>
        /// <param name="controllerDescriptor">The controller descriptor.</param>
        /// <returns>
        /// A map of <see cref="T:System.Web.Http.Controllers.HttpActionDescriptor" /> that the selector can select, or null if the selector does not have a well-defined mapping of <see cref="T:System.Web.Http.Controllers.HttpActionDescriptor" />.
        /// </returns>
        public virtual ILookup<string, HttpActionDescriptor> GetActionMapping(HttpControllerDescriptor controllerDescriptor)
        {
            return InnerActionSelector.GetActionMapping(controllerDescriptor);
        }
    }
}