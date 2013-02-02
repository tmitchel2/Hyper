using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;

namespace Hyper.Http.Controllers
{
    /// <summary>
    /// HttpParameterBindingExtensions class.
    /// </summary>
    internal static class HttpParameterBindingExtensions
    {
        /// <summary>
        /// Wills the read URI.
        /// </summary>
        /// <param name="parameterBinding">The parameter binding.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static bool WillReadUri(this HttpParameterBinding parameterBinding)
        {
            if (parameterBinding == null)
            {
                throw new ArgumentNullException("parameterBinding");
            }
            var parameterBinding1 = parameterBinding as IValueProviderParameterBinding;
            if (parameterBinding1 != null)
            {
                var providerFactories = parameterBinding1.ValueProviderFactories;

                // && providerFactories.All(factory => factory is IUriValueProviderFactory))
                if (providerFactories.Any()) 
                {
                    return true;
                }
            }
            return false;
        }
    }
}