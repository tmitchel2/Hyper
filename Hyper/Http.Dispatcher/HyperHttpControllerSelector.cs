using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace Hyper.Http.Dispatcher
{
    /// <summary>
    /// HyperHttpControllerSelector class.
    /// </summary>
    public class HyperHttpControllerSelector : IHttpControllerSelector
    {
        private readonly HttpConfiguration _configuration;
        private readonly HttpControllerTypeCache _controllerTypeCache;
        private readonly Lazy<ConcurrentDictionary<string, HttpControllerDescriptor>> _controllerInfoCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperHttpControllerSelector" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public HyperHttpControllerSelector(HttpConfiguration configuration)
        {
            _configuration = configuration;
            _controllerTypeCache = new HttpControllerTypeCache(_configuration);
            _controllerInfoCache = new Lazy<ConcurrentDictionary<string, HttpControllerDescriptor>>(InitializeControllerInfoCache);
        }

        /// <summary>
        /// Selects a <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> for the given <see cref="T:System.Net.Http.HttpRequestMessage" />.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <returns>
        /// An <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> instance.
        /// </returns>
        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var controllerName = GetControllerName(request);

            HttpControllerDescriptor controllerDescriptor;
            if (_controllerInfoCache.Value.TryGetValue(controllerName, out controllerDescriptor))
            {
                return controllerDescriptor;
            }

            return null;
        }

        /// <summary>
        /// Returns a map, keyed by controller string, of all <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> that the selector can select.  This is primarily called by <see cref="T:System.Web.Http.Description.IApiExplorer" /> to discover all the possible controllers in the system.
        /// </summary>
        /// <returns>
        /// A map of all <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" /> that the selector can select, or null if the selector does not have a well-defined mapping of <see cref="T:System.Web.Http.Controllers.HttpControllerDescriptor" />.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the name of the controller.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The controller name.</returns>
        public virtual string GetControllerName(HttpRequestMessage request)
        {
            var routeData = (IHttpRouteData)request.Properties[HttpPropertyKeys.HttpRouteDataKey];
            if (routeData == null)
            {
                return null;
            }

            return (string)routeData.Values["controller"];
        }

        /// <summary>
        /// Initializes the controller info cache.
        /// </summary>
        /// <returns></returns>
        private ConcurrentDictionary<string, HttpControllerDescriptor> InitializeControllerInfoCache()
        {
            var concurrentDictionary = new ConcurrentDictionary<string, HttpControllerDescriptor>(StringComparer.OrdinalIgnoreCase);
            var hashSet = new HashSet<string>();
            foreach (KeyValuePair<string, ILookup<string, Type>> keyValuePair in _controllerTypeCache.Cache)
            {
                string key = keyValuePair.Key;
                foreach (IEnumerable<Type> enumerable in keyValuePair.Value)
                {
                    foreach (Type controllerType in enumerable)
                    {
                        if (concurrentDictionary.Keys.Contains(key))
                        {
                            hashSet.Add(key);
                            break;
                        }
                        concurrentDictionary.TryAdd(
                            key, new HttpControllerDescriptor(_configuration, key, controllerType));
                    }
                }
            }
            foreach (string key in hashSet)
            {
                HttpControllerDescriptor controllerDescriptor;
                concurrentDictionary.TryRemove(key, out controllerDescriptor);
            }
            return concurrentDictionary;
        }
    }
}