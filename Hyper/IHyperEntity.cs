namespace Hyper
{
    /// <summary>
    /// IHyperEntity interface.
    /// </summary>
    public interface IHyperEntity
    {
        /// <summary>
        /// Gets or sets the self.
        /// </summary>
        /// <value>
        /// The self.
        /// </value>
        [HyperLink(Rel = "self")]
        HyperLink Self { get; set; }
    }
}