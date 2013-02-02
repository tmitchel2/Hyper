/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Http.ModelBinding;

namespace Hyper.Http.Controllers
{
    /// <summary>
    /// RequestExtensions class.
    /// </summary>
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
                throw new ArgumentNullException("request");
            else
                return GetProperty<HttpConfiguration>(HttpRequestMessageExtensions, request, HttpPropertyKeys.HttpConfigurationKey);
        }

        private static T GetProperty<T>(this HttpRequestMessage request, string key)
        {
            T obj;
            request.Properties.TryGetValue(key, out obj);
            return obj;
        }

        /// <summary>
        /// Gets the parsed query string as a collection of key-value pairs.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>
        /// The query string as a collection of key-value pairs.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static IEnumerable<KeyValuePair<string, string>> GetQueryNameValuePairs(this HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var requestUri = request.RequestUri;
            if (requestUri == null || string.IsNullOrEmpty(requestUri.Query))
            {
                return Enumerable.Empty<KeyValuePair<string, string>>();
            }

            IEnumerable<KeyValuePair<string, string>> jqueryNameValuePairs;
            if (!request.Properties.TryGetValue(HttpPropertyKeys.RequestQueryNameValuePairsKey, out jqueryNameValuePairs))
            {
                // TODO
                // jqueryNameValuePairs = FormDataCollectionExtensions.GetJQueryNameValuePairs(new FormDataCollection(requestUri));
                request.Properties.Add(HttpPropertyKeys.RequestQueryNameValuePairsKey, jqueryNameValuePairs);
            }

            return jqueryNameValuePairs;
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
            return HttpRequestMessageExtensions.CreateErrorResponse(request, statusCode, (Func<bool, HttpError>)(includeErrorDetail =>
            {
                if (!includeErrorDetail)
                    return new HttpError(message);
                else
                    return new HttpError(message + messageDetail);
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
                throw new ArgumentNullException("request");
            else
                return HttpRequestMessageExtensions.CreateErrorResponse(request, statusCode, (Func<bool, HttpError>)(includeErrorDetail => new HttpError(exception, includeErrorDetail)
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
                throw new ArgumentNullException("request");
            else
                return HttpRequestMessageExtensions.CreateErrorResponse(request, statusCode, (Func<bool, HttpError>)(includeErrorDetail => new HttpError(exception, includeErrorDetail)));
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
                throw new ArgumentNullException("request");
            else
                return HttpRequestMessageExtensions.CreateErrorResponse(request, statusCode, (Func<bool, HttpError>)(includeErrorDetail => new HttpError(modelState, includeErrorDetail)));
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
                throw new ArgumentNullException("request");
            else
                return HttpRequestMessageExtensions.CreateErrorResponse(request, statusCode, (Func<bool, HttpError>)(includeErrorDetail => error));
        }

        private static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, Func<bool, HttpError> errorCreator)
        {
            HttpConfiguration configuration1 = GetConfiguration(HttpRequestMessageExtensions, request);
            if (configuration1 == null)
            {
                using (HttpConfiguration configuration2 = new HttpConfiguration())
                {
                    HttpError httpError = errorCreator(configuration2.ShouldIncludeErrorDetail(request));
                    return CreateResponse<HttpError>(request, statusCode, httpError, configuration2);
                }
            }
            else
            {
                HttpError httpError = errorCreator(configuration1.ShouldIncludeErrorDetail(request));
                return CreateResponse<HttpError>(request, statusCode, httpError, configuration1);
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
            HttpConfiguration configuration = (HttpConfiguration)null;
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
                IEnumerable<MediaTypeFormatter> formatters = (IEnumerable<MediaTypeFormatter>)configuration.Formatters;
                ContentNegotiationResult negotiationResult = contentNegotiator.Negotiate(typeof(T), request, formatters);
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
                        Content = (HttpContent)new ObjectContent<T>(value, negotiationResult.Formatter, mediaType),
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
            MediaTypeFormatter writer = configuration.Formatters.FindWriter(typeof(T), mediaType);
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
            return HttpRequestMessageExtensions.CreateResponse<T>(request, statusCode, value, formatter, (MediaTypeHeaderValue)null);
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
            MediaTypeHeaderValue mediaType1 = mediaType != null ? new MediaTypeHeaderValue(mediaType) : (MediaTypeHeaderValue)null;
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
            response.Content = (HttpContent)new ObjectContent<T>(value, formatter, mediaType);
            return response;
        }

    }
}
*/