using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hyper
{
    /// <summary>
    /// HyperListLink class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [HyperContract(Name = "hyperlinklist", MediaType = "application/vnd.hyper.hyperlinklist", Version = "1.0.0.0")]
    public class HyperListLink<T> where T : IHyperEntity<T>
    {
        public static readonly HyperListLink<T> Empty = new HyperListLink<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperListLink{T}" /> class.
        /// </summary>
        public HyperListLink()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperListLink{T}" /> class.
        /// </summary>
        /// <param name="href">The href.</param>
        public HyperListLink(string href)
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
        /// Gets or sets the embedded.
        /// </summary>
        /// <value>
        /// The embedded.
        /// </value>
        [HyperMember(Name = "embedded", IsOptional = true)]
        public IList<T> Embedded { get; set; }

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
        public Task<HyperList<T>> Get(HyperClient client)
        {
            return client.Get<HyperList<T>>(Href);
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