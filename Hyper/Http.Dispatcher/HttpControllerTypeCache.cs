using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace Hyper.Http.Dispatcher
{
    /// <summary>
    /// HttpControllerTypeCache class.
    /// </summary>
    internal sealed class HttpControllerTypeCache
    {
        private readonly HttpConfiguration _configuration;
        private readonly Lazy<Dictionary<string, ILookup<string, Type>>> _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpControllerTypeCache" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public HttpControllerTypeCache(HttpConfiguration configuration)
        {
            _configuration = configuration;
            _cache = new Lazy<Dictionary<string, ILookup<string, Type>>>(InitializeCache);
        }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <value>
        /// The cache.
        /// </value>
        internal Dictionary<string, ILookup<string, Type>> Cache
        {
            get
            {
                return _cache.Value;
            }
        }

        /// <summary>
        /// Gets the controller types.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns></returns>
        public ICollection<Type> GetControllerTypes(string controllerName)
        {
            var hashSet = new HashSet<Type>();
            ILookup<string, Type> lookup;
            if (_cache.Value.TryGetValue(controllerName, out lookup))
            {
                foreach (IGrouping<string, Type> current in lookup)
                {
                    hashSet.UnionWith(current);
                }
            }
            return hashSet;
        }

        /// <summary>
        /// Initializes the cache.
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, ILookup<string, Type>> InitializeCache()
        {
            var assembliesResolver = _configuration.Services.GetAssembliesResolver();
            var httpControllerTypeResolver = _configuration.Services.GetHttpControllerTypeResolver();
            var controllerTypes = httpControllerTypeResolver.GetControllerTypes(assembliesResolver);
            var source = controllerTypes.GroupBy(t => t.Name.Substring(0, t.Name.Length - DefaultHttpControllerSelector.ControllerSuffix.Length), StringComparer.OrdinalIgnoreCase);
            return source.ToDictionary(g => g.Key, g => g.ToLookup(t => t.Namespace ?? string.Empty, StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase);
        }
    }
}