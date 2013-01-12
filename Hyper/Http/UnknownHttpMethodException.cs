using System;

namespace Hyper.Http
{
    /// <summary>
    /// UnknownHttpMethodException class.
    /// </summary>
    [Serializable]
    public class UnknownHttpMethodException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownHttpMethodException" /> class.
        /// </summary>
        /// <param name="method">The http method.</param>
        public UnknownHttpMethodException(string method)
            : base(string.Format("The method '{0}' is not valid", method))
        {
        }
    }
}