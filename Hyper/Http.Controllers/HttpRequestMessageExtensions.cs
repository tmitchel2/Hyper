/*
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Http.Hosting;
using System.Web.Http.ModelBinding;
using System.Web.Http.Routing;

namespace Hyper.Http.Controllers
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Retrieves the <see cref="T:System.Web.Http.HttpConfiguration"/> for the given request.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="T:System.Web.Http.HttpConfiguration"/> for the given request.
        /// </returns>
        /// <param name="request">The HTTP request.</param>
        public static HttpConfiguration GetConfiguration(this HttpRequestMessage request)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            else
                return GetProperty<HttpConfiguration>(HttpRequestMessageExtensions, request, HttpPropertyKeys.HttpConfigurationKey);
        }

        /// <summary>
        /// Retrieves the <see cref="T:System.Web.Http.Dependencies.IDependencyScope"/> for the given request or null if not available.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="T:System.Web.Http.Dependencies.IDependencyScope"/> for the given request or null if not available.
        /// </returns>
        /// <param name="request">The HTTP request.</param>
        public static IDependencyScope GetDependencyScope(this HttpRequestMessage request)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            IDependencyScope dependencyScope;
            if (!DictionaryExtensions.TryGetValue<IDependencyScope>(request.Properties, HttpPropertyKeys.DependencyScope, out dependencyScope))
            {
                dependencyScope = GetConfiguration(HttpRequestMessageExtensions, request).DependencyResolver.BeginScope();
                request.Properties[HttpPropertyKeys.DependencyScope] = (object) dependencyScope;
                RegisterForDispose(HttpRequestMessageExtensions, request, (IDisposable) dependencyScope);
            }
            return dependencyScope;
        }

        /// <summary>
        /// Retrieves the <see cref="T:System.Threading.SynchronizationContext"/> for the given request or null if not available.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="T:System.Threading.SynchronizationContext"/> for the given request or null if not available.
        /// </returns>
        /// <param name="request">The HTTP request.</param>
        public static SynchronizationContext GetSynchronizationContext(this HttpRequestMessage request)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            else
                return GetProperty<SynchronizationContext>(HttpRequestMessageExtensions, request, HttpPropertyKeys.SynchronizationContextKey);
        }

        /// <summary>
        /// Gets the current X.509 certificate from the given HTTP request.
        /// </summary>
        /// 
        /// <returns>
        /// The current <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2"/>, or null if a certificate is not available.
        /// </returns>
        /// <param name="request">The HTTP request.</param>
        public static X509Certificate2 GetClientCertificate(this HttpRequestMessage request)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            X509Certificate2 x509Certificate2 = (X509Certificate2) null;
            Func<HttpRequestMessage, X509Certificate2> func;
            if (!DictionaryExtensions.TryGetValue<X509Certificate2>(request.Properties, HttpPropertyKeys.ClientCertificateKey, out x509Certificate2) && DictionaryExtensions.TryGetValue<Func<HttpRequestMessage, X509Certificate2>>(request.Properties, HttpPropertyKeys.RetrieveClientCertificateDelegateKey, out func))
            {
                x509Certificate2 = func(request);
                if (x509Certificate2 != null)
                    request.Properties.Add(HttpPropertyKeys.ClientCertificateKey, (object) x509Certificate2);
            }
            return x509Certificate2;
        }

        /// <summary>
        /// Retrieves the <see cref="T:System.Web.Http.Routing.IHttpRouteData"/> for the given request or null if not available.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="T:System.Web.Http.Routing.IHttpRouteData"/> for the given request or null if not available.
        /// </returns>
        /// <param name="request">The HTTP request.</param>
        public static IHttpRouteData GetRouteData(this HttpRequestMessage request)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            else
                return GetProperty<IHttpRouteData>(HttpRequestMessageExtensions, request, HttpPropertyKeys.HttpRouteDataKey);
        }

        private static T GetProperty<T>(this HttpRequestMessage request, string key)
        {
            T obj;
            DictionaryExtensions.TryGetValue<T>(request.Properties, key, out obj);
            return obj;
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage"/> that represents an error message.
        /// </summary>
        /// 
        /// <returns>
        /// The request must be associated with an <see cref="T:System.Web.Http.HttpConfiguration"/> instance.An <see cref="T:System.Net.Http.HttpResponseMessage"/> whose content is a serialized representation of an <see cref="T:System.Web.Http.HttpError"/> instance.
        /// </returns>
        /// <param name="request">The HTTP request.</param><param name="statusCode">The status code of the response.</param><param name="message">The error message.</param>
        public static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, string message)
        {
            return HttpRequestMessageExtensions.CreateErrorResponse(request, statusCode, new HttpError(message));
        }

        internal static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, string message, string messageDetail)
        {
            return HttpRequestMessageExtensions.CreateErrorResponse(request, statusCode, (Func<bool, HttpError>) (includeErrorDetail =>
                {
                    if (!includeErrorDetail)
                        return new HttpError(message);
                    else
                        return new HttpError(message, messageDetail);
                }));
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage"/> that represents an exception with an error message.
        /// </summary>
        /// 
        /// <returns>
        /// The request must be associated with an <see cref="T:System.Web.Http.HttpConfiguration"/> instance.An <see cref="T:System.Net.Http.HttpResponseMessage"/> whose content is a serialized representation of an <see cref="T:System.Web.Http.HttpError"/> instance.
        /// </returns>
        /// <param name="request">The HTTP request.</param><param name="statusCode">The status code of the response.</param><param name="message">The error message.</param><param name="exception">The exception.</param>
        public static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, string message, Exception exception)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            else
                return HttpRequestMessageExtensions.CreateErrorResponse(request, statusCode, (Func<bool, HttpError>) (includeErrorDetail => new HttpError(exception, includeErrorDetail)
                    {
                        Message = message
                    }));
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage"/> that represents an exception.
        /// </summary>
        /// 
        /// <returns>
        /// The request must be associated with an <see cref="T:System.Web.Http.HttpConfiguration"/> instance.An <see cref="T:System.Net.Http.HttpResponseMessage"/> whose content is a serialized representation of an  <see cref="T:System.Web.Http.HttpError"/> instance.
        /// </returns>
        /// <param name="request">The HTTP request.</param><param name="statusCode">The status code of the response.</param><param name="exception">The exception.</param>
        public static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, Exception exception)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            else
                return HttpRequestMessageExtensions.CreateErrorResponse(request, statusCode, (Func<bool, HttpError>) (includeErrorDetail => new HttpError(exception, includeErrorDetail)));
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage"/> that represents an error in the model state.
        /// </summary>
        /// 
        /// <returns>
        /// The request must be associated with an <see cref="T:System.Web.Http.HttpConfiguration"/> instance.An <see cref="T:System.Net.Http.HttpResponseMessage"/> whose content is a serialized representation of an <see cref="T:System.Web.Http.HttpError"/> instance.
        /// </returns>
        /// <param name="request">The HTTP request.</param><param name="statusCode">The status code of the response.</param><param name="modelState">The model state.</param>
        public static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, ModelStateDictionary modelState)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            else
                return HttpRequestMessageExtensions.CreateErrorResponse(request, statusCode, (Func<bool, HttpError>) (includeErrorDetail => new HttpError(modelState, includeErrorDetail)));
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage"/> that represents an error.
        /// </summary>
        /// 
        /// <returns>
        /// The request must be associated with an <see cref="T:System.Web.Http.HttpConfiguration"/> instance.An <see cref="T:System.Net.Http.HttpResponseMessage"/> whose content is a serialized representation of an <see cref="T:System.Web.Http.HttpError"/> instance.
        /// </returns>
        /// <param name="request">The HTTP request.</param><param name="statusCode">The status code of the response.</param><param name="error">The HTTP error.</param>
        public static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, HttpError error)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            else
                return HttpRequestMessageExtensions.CreateErrorResponse(request, statusCode, (Func<bool, HttpError>) (includeErrorDetail => error));
        }

        private static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, Func<bool, HttpError> errorCreator)
        {
            HttpConfiguration configuration1 = GetConfiguration(HttpRequestMessageExtensions, request);
            if (configuration1 == null)
            {
                using (HttpConfiguration configuration2 = new HttpConfiguration())
                {
                    HttpError httpError = errorCreator(configuration2.ShouldIncludeErrorDetail(request));
                    return HttpRequestMessageExtensions.CreateResponse<HttpError>(request, statusCode, httpError, configuration2);
                }
            }
            else
            {
                HttpError httpError = errorCreator(configuration1.ShouldIncludeErrorDetail(request));
                return HttpRequestMessageExtensions.CreateResponse<HttpError>(request, statusCode, httpError, configuration1);
            }
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage"/> wired up to the associated <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </summary>
        /// 
        /// <returns>
        /// An initialized <see cref="T:System.Net.Http.HttpResponseMessage"/> wired up to the associated <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </returns>
        /// <param name="request">The HTTP request message which led to this response message.</param><param name="statusCode">The HTTP response status code.</param><param name="value">The content of the HTTP response message.</param><typeparam name="T">The type of the HTTP response message.</typeparam>
        public static HttpResponseMessage CreateResponse<T>(this HttpRequestMessage request, HttpStatusCode statusCode, T value)
        {
            HttpConfiguration configuration = (HttpConfiguration) null;
            return HttpRequestMessageExtensions.CreateResponse<T>(request, statusCode, value, configuration);
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage"/> wired up to the associated <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </summary>
        /// 
        /// <returns>
        /// An initialized <see cref="T:System.Net.Http.HttpResponseMessage"/> wired up to the associated <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </returns>
        /// <param name="request">The HTTP request message which led to this response message.</param><param name="statusCode">The HTTP response status code.</param><param name="value">The content of the HTTP response message.</param><param name="configuration">The HTTP configuration which contains the dependency resolver used to resolve services.</param><typeparam name="T">The type of the HTTP response message.</typeparam>
        public static HttpResponseMessage CreateResponse<T>(this HttpRequestMessage request, HttpStatusCode statusCode, T value, HttpConfiguration configuration)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            configuration = configuration ?? GetConfiguration(HttpRequestMessageExtensions, request);
            if (configuration == null)
                throw Error.InvalidOperation(SRResources.HttpRequestMessageExtensions_NoConfiguration, new object[0]);
            IContentNegotiator contentNegotiator = ServicesExtensions.GetContentNegotiator(configuration.Services);
            if (contentNegotiator == null)
            {
                throw Error.InvalidOperation(SRResources.HttpRequestMessageExtensions_NoContentNegotiator, new object[1]
                    {
                        (object) typeof (IContentNegotiator).FullName
                    });
            }
            else
            {
                IEnumerable<MediaTypeFormatter> formatters = (IEnumerable<MediaTypeFormatter>) configuration.Formatters;
                ContentNegotiationResult negotiationResult = contentNegotiator.Negotiate(typeof (T), request, formatters);
                if (negotiationResult == null)
                {
                    return new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.NotAcceptable,
                            RequestMessage = request
                        };
                }
                else
                {
                    MediaTypeHeaderValue mediaType = negotiationResult.MediaType;
                    return new HttpResponseMessage()
                        {
                            Content = (HttpContent) new ObjectContent<T>(value, negotiationResult.Formatter, mediaType),
                            StatusCode = statusCode,
                            RequestMessage = request
                        };
                }
            }
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage"/> wired up to the associated <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </summary>
        /// 
        /// <returns>
        /// An initialized <see cref="T:System.Net.Http.HttpResponseMessage"/> wired up to the associated <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </returns>
        /// <param name="request">The HTTP request message which led to this response message.</param><param name="statusCode">The HTTP response status code.</param><param name="value">The content of the HTTP response message.</param><param name="mediaType">The media type.</param><typeparam name="T">The type of the HTTP response message.</typeparam>
        public static HttpResponseMessage CreateResponse<T>(this HttpRequestMessage request, HttpStatusCode statusCode, T value, string mediaType)
        {
            return HttpRequestMessageExtensions.CreateResponse<T>(request, statusCode, value, new MediaTypeHeaderValue(mediaType));
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage"/> wired up to the associated <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </summary>
        /// 
        /// <returns>
        /// An initialized <see cref="T:System.Net.Http.HttpResponseMessage"/> wired up to the associated <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </returns>
        /// <param name="request">The HTTP request message which led to this response message.</param><param name="statusCode">The HTTP response status code.</param><param name="value">The content of the HTTP response message.</param><param name="mediaType">The media type header value.</param><typeparam name="T">The type of the HTTP response message.</typeparam>
        public static HttpResponseMessage CreateResponse<T>(this HttpRequestMessage request, HttpStatusCode statusCode, T value, MediaTypeHeaderValue mediaType)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            if (mediaType == null)
                throw Error.ArgumentNull("mediaType");
            HttpConfiguration configuration = GetConfiguration(HttpRequestMessageExtensions, request);
            if (configuration == null)
                throw Error.InvalidOperation(SRResources.HttpRequestMessageExtensions_NoConfiguration, new object[0]);
            MediaTypeFormatter writer = configuration.Formatters.FindWriter(typeof (T), mediaType);
            if (writer != null)
                return HttpRequestMessageExtensions.CreateResponse<T>(request, statusCode, value, writer, mediaType);
            throw Error.InvalidOperation(SRResources.HttpRequestMessageExtensions_NoMatchingFormatter, new object[2]
                {
                    (object) mediaType,
                    (object) typeof (T).Name
                });
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage"/> wired up to the associated <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </summary>
        /// 
        /// <returns>
        /// An initialized <see cref="T:System.Net.Http.HttpResponseMessage"/> wired up to the associated <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </returns>
        /// <param name="request">The HTTP request message which led to this response message.</param><param name="statusCode">The HTTP response status code.</param><param name="value">The content of the HTTP response message.</param><param name="formatter">The media type formatter.</param><typeparam name="T">The type of the HTTP response message.</typeparam>
        public static HttpResponseMessage CreateResponse<T>(this HttpRequestMessage request, HttpStatusCode statusCode, T value, MediaTypeFormatter formatter)
        {
            return HttpRequestMessageExtensions.CreateResponse<T>(request, statusCode, value, formatter, (MediaTypeHeaderValue) null);
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage"/> wired up to the associated <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </summary>
        /// 
        /// <returns>
        /// An initialized <see cref="T:System.Net.Http.HttpResponseMessage"/> wired up to the associated <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </returns>
        /// <param name="request">The HTTP request message which led to this response message.</param><param name="statusCode">The HTTP response status code.</param><param name="value">The content of the HTTP response message.</param><param name="formatter">The media type formatter.</param><param name="mediaType">The media type.</param><typeparam name="T">The type of the HTTP response message.</typeparam>
        public static HttpResponseMessage CreateResponse<T>(this HttpRequestMessage request, HttpStatusCode statusCode, T value, MediaTypeFormatter formatter, string mediaType)
        {
            MediaTypeHeaderValue mediaType1 = mediaType != null ? new MediaTypeHeaderValue(mediaType) : (MediaTypeHeaderValue) null;
            return HttpRequestMessageExtensions.CreateResponse<T>(request, statusCode, value, formatter, mediaType1);
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage"/> wired up to the associated <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </summary>
        /// 
        /// <returns>
        /// An initialized <see cref="T:System.Net.Http.HttpResponseMessage"/> wired up to the associated <see cref="T:System.Net.Http.HttpRequestMessage"/>.
        /// </returns>
        /// <param name="request">The HTTP request message which led to this response message.</param><param name="statusCode">The HTTP response status code.</param><param name="value">The content of the HTTP response message.</param><param name="formatter">The media type formatter.</param><param name="mediaType">The media type header value.</param><typeparam name="T">The type of the HTTP response message.</typeparam>
        public static HttpResponseMessage CreateResponse<T>(this HttpRequestMessage request, HttpStatusCode statusCode, T value, MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            if (formatter == null)
                throw Error.ArgumentNull("formatter");
            HttpResponseMessage response = HttpRequestMessageExtensions.CreateResponse(request, statusCode);
            response.Content = (HttpContent) new ObjectContent<T>(value, formatter, mediaType);
            return response;
        }

        /// <summary>
        /// Adds the given <paramref name="resource"/> to a list of resources that will be disposed by a host once the <paramref name="request"/> is disposed.
        /// </summary>
        /// <param name="request">The HTTP request controlling the lifecycle of <paramref name="resource"/>.</param><param name="resource">The resource to dispose when <paramref name="request"/> is being disposed.</param>
        public static void RegisterForDispose(this HttpRequestMessage request, IDisposable resource)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            if (resource == null)
                return;
            List<IDisposable> list;
            if (!DictionaryExtensions.TryGetValue<List<IDisposable>>(request.Properties, HttpPropertyKeys.DisposableRequestResourcesKey, out list))
            {
                list = new List<IDisposable>();
                request.Properties[HttpPropertyKeys.DisposableRequestResourcesKey] = (object) list;
            }
            list.Add(resource);
        }

        /// <summary>
        /// Disposes of all tracked resources associated with the <paramref name="request"/> which were added via the <see cref="M:Hyper.Http.Controllers.HttpRequestMessageExtensions.RegisterForDispose(System.Net.Http.HttpRequestMessage,System.IDisposable)"/> method.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        public static void DisposeRequestResources(this HttpRequestMessage request)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            List<IDisposable> list;
            if (!DictionaryExtensions.TryGetValue<List<IDisposable>>(request.Properties, HttpPropertyKeys.DisposableRequestResourcesKey, out list))
                return;
            foreach (IDisposable disposable in list)
            {
                try
                {
                    disposable.Dispose();
                }
                catch
                {
                }
            }
            list.Clear();
        }

        /// <summary>
        /// Retrieves the <see cref="T:System.Guid"/> which has been assigned as the correlation ID associated with the given <paramref name="request"/>. The value will be created and set the first time this method is called.
        /// </summary>
        /// 
        /// <returns>
        /// The <see cref="T:System.Guid"/> object that represents the correlation ID associated with the request.
        /// </returns>
        /// <param name="request">The HTTP request.</param>
        public static Guid GetCorrelationId(this HttpRequestMessage request)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            Guid guid;
            if (!DictionaryExtensions.TryGetValue<Guid>(request.Properties, HttpPropertyKeys.RequestCorrelationKey, out guid))
            {
                guid = Guid.NewGuid();
                request.Properties.Add(HttpPropertyKeys.RequestCorrelationKey, (object) guid);
            }
            return guid;
        }

        /// <summary>
        /// Gets the parsed query string as a collection of key-value pairs.
        /// </summary>
        /// 
        /// <returns>
        /// The query string as a collection of key-value pairs.
        /// </returns>
        /// <param name="request">The HTTP request.</param>
        public static IEnumerable<KeyValuePair<string, string>> GetQueryNameValuePairs(this HttpRequestMessage request)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            Uri requestUri = request.RequestUri;
            if (requestUri == (Uri) null || string.IsNullOrEmpty(requestUri.Query))
                return Enumerable.Empty<KeyValuePair<string, string>>();
            IEnumerable<KeyValuePair<string, string>> jqueryNameValuePairs;
            if (!DictionaryExtensions.TryGetValue<IEnumerable<KeyValuePair<string, string>>>(request.Properties, HttpPropertyKeys.RequestQueryNameValuePairsKey, out jqueryNameValuePairs))
            {
                jqueryNameValuePairs = FormDataCollectionExtensions.GetJQueryNameValuePairs(new FormDataCollection(requestUri));
                request.Properties.Add(HttpPropertyKeys.RequestQueryNameValuePairsKey, (object) jqueryNameValuePairs);
            }
            return jqueryNameValuePairs;
        }

        /// <summary>
        /// Gets a <see cref="T:System.Web.Http.Routing.UrlHelper"/> instance for an HTTP request.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.Web.Http.Routing.UrlHelper"/> instance that is initialized for the specified HTTP request.
        /// </returns>
        /// <param name="request">The HTTP request.</param>
        public static UrlHelper GetUrlHelper(this HttpRequestMessage request)
        {
            if (request == null)
                throw Error.ArgumentNull("request");
            else
                return new UrlHelper(request);
        }
    }
}
*/