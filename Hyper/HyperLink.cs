namespace Hyper
{
    /// <summary>
    /// HyperLink class.
    /// </summary>
    [HyperContract(Name = "hyperlink", MediaType = "application/vnd.hyper.hyperlink", Version = "1.0.0.0")]
    public class HyperLink
    {
        public static readonly HyperLink Empty = new HyperLink();

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperLink" /> class.
        /// </summary>
        public HyperLink()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperLink" /> class.
        /// </summary>
        /// <param name="href">The href.</param>
        public HyperLink(string href)
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
        /// Gets or sets the hreflang.
        /// </summary>
        /// <value>
        /// The hreflang.
        /// </value>
        [HyperMember(Name = "hreflang", IsOptional = true)]
        public string Hreflang { get; set; }

        /// <summary>
        /// Gets or sets the rel.
        /// </summary>
        /// <value>
        /// The rel.
        /// </value>
        [HyperMember(Name = "rel", IsOptional = true)]
        public string Rel { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [HyperMember(Name = "name", IsOptional = true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [HyperMember(Name = "title", IsOptional = true)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is templated.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is templated; otherwise, <c>false</c>.
        /// </value>
        [HyperMember(Name = "templated", IsOptional = true)]
        public bool IsTemplated { get; set; }

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