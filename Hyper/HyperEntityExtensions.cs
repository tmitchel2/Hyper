using System.Net;
using System.Threading.Tasks;

namespace Hyper
{
    /// <summary>
    /// HyperEntityExtensions class.
    /// </summary>
    public static class HyperEntityExtensions
    {
        /// <summary>
        /// Gets the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        public static Task<T> Get<T>(this T item, HyperClient client) where T : IHyperEntity
        {
            return client.Get<T>(item.Self.Href);
        }

        /// <summary>
        /// Puts the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        public static Task<T> Put<T>(this T item, HyperClient client) where T : IHyperEntity
        {
            return client.Put(item.Self.Href, item);
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="client">The client.</param>
        /// <param name="expectedCode">The expected code.</param>
        /// <returns>Task object.</returns>
        public static Task Delete<T>(this T item, HyperClient client, HttpStatusCode expectedCode = HttpStatusCode.OK) where T : IHyperEntity
        {
            return client.Delete(item.Self.Href, expectedCode);
        }
    }
}