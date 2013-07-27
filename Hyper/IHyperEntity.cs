namespace Hyper
{
    /// <summary>
    /// IHyperEntity interface.
    /// </summary>
    /// <typeparam name="T">Type T.</typeparam>
    public interface IHyperEntity<T>
    {
        /// <summary>
        /// Gets or sets the self.
        /// </summary>
        /// <value>
        /// The self.
        /// </value>
        [HyperLink(Rel = "self")]
        HyperLink<T> Self { get; set; }
    }
}