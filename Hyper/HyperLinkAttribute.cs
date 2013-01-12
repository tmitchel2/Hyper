using System;

namespace Hyper
{
    /// <summary>
    /// HyperLinkAttribute class.
    /// </summary>
    public class HyperLinkAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the href.
        /// </summary>
        /// <value>
        /// The href.
        /// </value>
        [HyperMember(Name = "href")]
        public string Href { get; set; }

        /// <summary>
        /// Gets or sets the hreflang.
        /// </summary>
        /// <value>
        /// The hreflang.
        /// </value>
        [HyperMember(Name = "hreflang")]
        public string Hreflang { get; set; }

        /// <summary>
        /// Gets or sets the rel.
        /// </summary>
        /// <value>
        /// The rel.
        /// </value>
        [HyperMember(Name = "rel")]
        public string Rel { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [HyperMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is templated.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is templated; otherwise, <c>false</c>.
        /// </value>
        [HyperMember(Name = "templated")]
        public bool IsTemplated { get; set; }
    }
}