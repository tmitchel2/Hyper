using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http.Routing;

namespace Hyper.Http.Routing
{
    /// <summary>
    /// LowercaseHttpRoute class.
    /// </summary>
    internal class LowercaseHttpRoute : HttpRoute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LowercaseHttpRoute" /> class.
        /// </summary>
        public LowercaseHttpRoute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LowercaseHttpRoute" /> class.
        /// </summary>
        /// <param name="routeTemplate">The route template.</param>
        public LowercaseHttpRoute(string routeTemplate)
            : base(routeTemplate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LowercaseHttpRoute" /> class.
        /// </summary>
        /// <param name="routeTemplate">The route template.</param>
        /// <param name="defaults">The default values for the route parameters.</param>
        public LowercaseHttpRoute(string routeTemplate, HttpRouteValueDictionary defaults)
            : base(routeTemplate, defaults)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LowercaseHttpRoute" /> class.
        /// </summary>
        /// <param name="routeTemplate">The route template.</param>
        /// <param name="defaults">The default values for the route parameters.</param>
        /// <param name="constraints">The constraints for the route parameters.</param>
        public LowercaseHttpRoute(string routeTemplate, HttpRouteValueDictionary defaults, HttpRouteValueDictionary constraints)
            : base(routeTemplate, defaults, constraints)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LowercaseHttpRoute" /> class.
        /// </summary>
        /// <param name="routeTemplate">The route template.</param>
        /// <param name="defaults">The default values for the route parameters.</param>
        /// <param name="constraints">The constraints for the route parameters.</param>
        /// <param name="dataTokens">Any additional tokens for the route parameters.</param>
        public LowercaseHttpRoute(string routeTemplate, HttpRouteValueDictionary defaults, HttpRouteValueDictionary constraints, HttpRouteValueDictionary dataTokens)
            : base(routeTemplate, defaults, constraints, dataTokens)
        {
        }

        /// <summary>
        /// Gets the virtual path.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public override IHttpVirtualPathData GetVirtualPath(System.Net.Http.HttpRequestMessage request, IDictionary<string, object> values)
        {
            var path = base.GetVirtualPath(request, values);
            if (path != null)
            {
                var virtualPath = path.VirtualPath;
                var lastIndexOf = virtualPath.LastIndexOf("?", StringComparison.Ordinal);
                if (lastIndexOf != 0)
                {
                    if (lastIndexOf > 0)
                    {
                        var leftPart = virtualPath.Substring(0, lastIndexOf).ToLowerInvariant();
                        var queryPart = virtualPath.Substring(lastIndexOf);
                        return new HttpVirtualPathData(this, leftPart + queryPart);
                    }
                    
                    var queryParams = HttpUtility.ParseQueryString(request.RequestUri.Query);
                    var acceptValue = queryParams.Get("accept");
                    return !string.IsNullOrWhiteSpace(acceptValue) ? new HttpVirtualPathData(this, path.VirtualPath.ToLowerInvariant() + "?accept=" + acceptValue) : new HttpVirtualPathData(this, path.VirtualPath.ToLowerInvariant());
                }
            }
            return path;
        }
    }
}