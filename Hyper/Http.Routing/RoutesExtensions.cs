using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Hyper.Http.Routing
{
    /// <summary>
    /// HttpRouteCollectionExtensions class.
    /// </summary>
    public static class HttpRouteCollectionExtensions
    {
        /// <summary>
        /// Maps the HTTP route lowercase.
        /// </summary>
        /// <param name="routes">The routes.</param>
        /// <param name="name">The name.</param>
        /// <param name="routeTemplate">The route template.</param>
        /// <returns></returns>
        public static IHttpRoute MapHttpRouteLowercase(this HttpRouteCollection routes, string name, string routeTemplate)
        {
            return MapHttpRouteLowercase(routes, name, routeTemplate, null, null);
        }

        /// <summary>
        /// Maps the HTTP route lowercase.
        /// </summary>
        /// <param name="routes">The routes.</param>
        /// <param name="name">The name.</param>
        /// <param name="routeTemplate">The route template.</param>
        /// <param name="defaults">The defaults.</param>
        /// <returns></returns>
        public static IHttpRoute MapHttpRouteLowercase(this HttpRouteCollection routes, string name, string routeTemplate, object defaults)
        {
            return MapHttpRouteLowercase(routes, name, routeTemplate, defaults, null);
        }

        /// <summary>
        /// Maps the HTTP route lowercase.
        /// </summary>
        /// <param name="routes">The routes.</param>
        /// <param name="name">The name.</param>
        /// <param name="routeTemplate">The route template.</param>
        /// <param name="defaults">The defaults.</param>
        /// <param name="constraints">The constraints.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static IHttpRoute MapHttpRouteLowercase(this HttpRouteCollection routes, string name, string routeTemplate, object defaults, object constraints)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }

            var route = CreateRoute(routeTemplate, GetTypeProperties(defaults), GetTypeProperties(constraints), new Dictionary<string, object>(), null);
            routes.Add(name, route);
            return route;
        }

        /// <summary>
        /// Creates the route.
        /// </summary>
        /// <param name="routeTemplate">The route template.</param>
        /// <param name="defaults">The defaults.</param>
        /// <param name="constraints">The constraints.</param>
        /// <param name="dataTokens">The data tokens.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        private static IHttpRoute CreateRoute(string routeTemplate, IDictionary<string, object> defaults, IDictionary<string, object> constraints, IDictionary<string, object> dataTokens, IDictionary<string, object> parameters)
        {
            var defaults1 = defaults != null ? new HttpRouteValueDictionary(defaults) : null;
            var constraints1 = constraints != null ? new HttpRouteValueDictionary(constraints) : null;
            var dataTokens1 = dataTokens != null ? new HttpRouteValueDictionary(dataTokens) : null;
            return new LowercaseHttpRoute(routeTemplate, defaults1, constraints1, dataTokens1);
        }

        /// <summary>
        /// Gets the type properties.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        private static IDictionary<string, object> GetTypeProperties(object instance)
        {
            var dictionary = new Dictionary<string, object>();
            if (instance != null)
            {
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(instance))
                {
                    var obj = propertyDescriptor.GetValue(instance);
                    dictionary.Add(propertyDescriptor.Name, obj);
                }
            }
            return dictionary;
        }
    }
}