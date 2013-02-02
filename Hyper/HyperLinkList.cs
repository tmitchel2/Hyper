using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hyper
{
    /// <summary>
    /// HyperLinkList class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [HyperContract(Name = "hyperlinklist", MediaType = "application/vnd.hyper.hyperlinklist", Version = "1.0.0.0")]
    public class HyperLinkList<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HyperLinkList{T}" /> class.
        /// </summary>
        public HyperLinkList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperLinkList{T}" /> class.
        /// </summary>
        /// <param name="href">The href.</param>
        public HyperLinkList(string href)
        {
            Href = href;
        }

        /// <summary>
        /// Gets or sets the href.
        /// </summary>
        /// <value>
        /// The href.
        /// </value>
        [HyperMember(Name = "href")]
        public string Href { get; set; }

        /// <summary>
        /// Posts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        public Task<T> Post(T item, HyperClient client)
        {
            return client.Post(Href, item);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        public Task<IList<T>> Get(HyperClient client)
        {
            return client.Get<IList<T>>(Href);
        }

        /// <summary>
        /// Gets the specified id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        public Task<T> Get(string id, HyperClient client)
        {
            return client.Get<T>(new Uri(new Uri(Href), id).ToString());
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Href;
        }
    }
}