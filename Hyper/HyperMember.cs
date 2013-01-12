namespace Hyper
{
    /// <summary>
    /// HyperMember class.
    /// </summary>
    [HyperContract(Name = "hypermember", MediaType = "application/vnd.hyper.hypermember", Version = "1.0.0.0")]
    public class HyperMember
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [HyperMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the hyper.
        /// </summary>
        /// <value>
        /// The type of the hyper.
        /// </value>
        [HyperMember(Name = "type")]
        public HyperType HyperType { get; set; }
    }
}