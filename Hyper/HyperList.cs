using System.Collections.Generic;
using System.Linq;

namespace Hyper
{
    /// <summary>
    /// HyperList class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [HyperContract(Name = "hyperlist", MediaType = "application/vnd.hyper.hyperlist", Version = "1.0.0.0")]
    public class HyperList<T> : IHyperEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HyperList{T}" /> class.
        /// </summary>
        public HyperList()
            : this(Enumerable.Empty<T>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperList{T}" /> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public HyperList(IEnumerable<T> items)
        {
            Items = items.ToList();
        }

        /// <summary>
        /// Gets or sets the self.
        /// </summary>
        /// <value>
        /// The self.
        /// </value>
        [HyperLink(Rel = "self")]
        public HyperLink Self { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [HyperMember(Name = "items", IsOptional = false)]
        public IList<T> Items { get; set; }
    }
}