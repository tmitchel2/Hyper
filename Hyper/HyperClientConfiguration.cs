using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Text;

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
            Formatters = new List<MediaTypeFormatter>();
            DefaultMediaTypeName = "json";
            DefaultEncoding = Encoding.UTF8;
        }

        /// <summary>
        /// Gets or sets the default name of the media type.
        /// </summary>
        /// <value>
        /// The default name of the media type.
        /// </value>
        public string DefaultMediaTypeName { get; set; }

        /// <summary>
        /// Gets or sets the default formatter.
        /// </summary>
        /// <value>
        /// The default formatter.
        /// </value>
        public MediaTypeFormatter DefaultFormatter { get; set; }

        /// <summary>
        /// Gets the formatters.
        /// </summary>
        /// <value>
        /// The formatters.
        /// </value>
        public IList<MediaTypeFormatter> Formatters { get; private set; }

        /// <summary>
        /// Gets or sets the default encoding.
        /// </summary>
        /// <value>
        /// The default encoding.
        /// </value>
        public Encoding DefaultEncoding { get; set; }
    }
}