using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Hyper.Http;
using Hyper.Http.Formatting;
using Hyper.Http.Serialization;

namespace Hyper
{
    /// <summary>
    /// HyperClient class.
    /// </summary>
    public class HyperClient : IDisposable
    {
        private readonly IHyperSerialiser _serialiser;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperClient" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public HyperClient(HyperClientConfiguration configuration)
        {
            Configuration = configuration;
            _serialiser = new HyperSerialiser(Configuration.DefaultFormatter);
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public HyperClientConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public async Task<T> Get<T>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(Configuration.DefaultFormatter.GetMediaType(typeof(T)));
            var result = await _httpClient.SendAsync(request);
            return await ProcessResult<T>(result);
        }

        /// <summary>
        /// Posts the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public async Task<T> Post<T>(string url, T item)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            // Add content
            var mediaType = Configuration.DefaultFormatter.GetMediaType(typeof(T));
            request.Headers.Accept.Add(mediaType);
            request.Content = new StringContent(_serialiser.Serialise(item), Configuration.DefaultEncoding, mediaType.ToString());

            // request.Content = new ObjectContent<T>(item, new HyperJsonMediaTypeFormatter(), mediaType.ToString())

            // Add authentication
            if (item is IHasBasicAuthicationDetails)
            {
                var session = item as IHasBasicAuthicationDetails;
                var usernamePassword = string.Format("{0}:{1}", session.Username, session.Password);
                var encodedUsernamePassword = Convert.ToBase64String(Encoding.ASCII.GetBytes(usernamePassword));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedUsernamePassword);
            }

            // Send
            var result = await _httpClient.SendAsync(request);
            return await ProcessResult<T>(result);
        }

        /// <summary>
        /// Puts the specified URL.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">The URL.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public Task<T> Put<T>(string url, T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="expectedCode">The expected code.</param>
        /// <returns>Task object.</returns>
        public async Task Delete(string url, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            var result = await _httpClient.SendAsync(request);
            await ProcessErrorResult(result, expectedCode);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Processes the result.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="result">The response message.</param>
        /// <param name="expectedCode">The expected code.</param>
        /// <returns>Task object.</returns>
        private async Task<T> ProcessResult<T>(HttpResponseMessage result, HttpStatusCode expectedCode = HttpStatusCode.OK)
        {
            switch (result.StatusCode)
            {
                case HttpStatusCode.OK:
                    var data = await result.Content.ReadAsStringAsync();
                    return _serialiser.Deserialise<T>(data);
                default:
                    await ProcessErrorResult(result, expectedCode);
                    return await new TaskFactory<T>().StartNew(() => default(T));
            }
        }

        /// <summary>
        /// Processes an error result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="expectedCode">The expected code.</param>
        /// <returns>Task object.</returns>
        private async Task ProcessErrorResult(HttpResponseMessage result, HttpStatusCode expectedCode)
        {
            if (result.StatusCode == expectedCode)
            {
                return;
            }

            switch (result.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    var authError = await result.Content.ReadAsStringAsync();
                    throw new AuthenticationException(authError);

                case HttpStatusCode.InternalServerError:
                    var error = await result.Content.ReadAsStringAsync();
                    var ex = _serialiser.Deserialise<Exception>(error);
                    throw ex;
            }
        }
    }
}