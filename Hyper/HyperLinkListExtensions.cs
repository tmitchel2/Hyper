using System.Threading.Tasks;

namespace Hyper
{
    /// <summary>
    /// HyperLinkListExtensions class
    /// </summary>
    public static class HyperLinkListExtensions
    {
        /// <summary>
        /// Gets the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="id">The id.</param>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        public static Task<T> Get<T>(this HyperListLink<T> list, long id, HyperClient client) where T : IHyperEntity<T>
        {
            return list.Get(id.ToString(), client);
        }

        /// <summary>
        /// Gets the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="id">The id.</param>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        public static Task<T> Get<T>(this HyperListLink<T> list, int id, HyperClient client) where T : IHyperEntity<T>
        {
            return list.Get(id.ToString(), client);
        }
    }
}