using System.Collections.Generic;
using Hyper.Http.Formatting;

namespace Hyper
{
    /// <summary>
    /// HyperClientConfiguration class
    /// </summary>
    public class HyperClientConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HyperClientConfiguration" /> class.
        /// </summary>
        public HyperClientConfiguration()
        {
            Formatters = new List<HyperMediaTypeFormatter>();
        }

        /// <summary>
        /// Gets the formatters.
        /// </summary>
        /// <value>
        /// The formatters.
        /// </value>
        public IList<HyperMediaTypeFormatter> Formatters { get; private set; }
    }
}