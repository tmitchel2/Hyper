using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Hyper.Http
{
    /// <summary>
    /// RestQueryParameterHandler class.
    /// </summary>
    public class RestQueryParameterHandler : DelegatingHandler
    {
        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.
        /// </returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var queryParams = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var acceptValue = queryParams.Get("accept");
            if (acceptValue != null)
            {
                request.Headers.Accept.Clear();
                request.Headers.Accept.ParseAdd(acceptValue);
            }

            var fieldsValue = queryParams.Get("fields");
            if (fieldsValue != null)
            {
            }

            var methodValue = queryParams.Get("method");
            if (methodValue != null)
            {
                try
                {
                    request.Method = ToMethod(methodValue);
                }
                catch (UnknownHttpMethodException ex)
                {
                    return request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// To the method.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static HttpMethod ToMethod(string value)
        {
            if (value == HttpMethod.Delete.Method)
            {
                return HttpMethod.Delete;
            }
            
            if (value == HttpMethod.Get.Method)
            {
                return HttpMethod.Get;
            }

            if (value == HttpMethod.Head.Method)
            {
                return HttpMethod.Head;
            }

            if (value == HttpMethod.Options.Method)
            {
                return HttpMethod.Options;
            }

            if (value == HttpMethod.Post.Method)
            {
                return HttpMethod.Post;
            }

            if (value == HttpMethod.Put.Method)
            {
                return HttpMethod.Put;
            }

            if (value == HttpMethod.Trace.Method)
            {
                return HttpMethod.Trace;
            }

            throw new UnknownHttpMethodException(value);
        }
    }
}