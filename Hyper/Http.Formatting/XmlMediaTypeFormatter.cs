using System;
using System.Net.Http.Formatting;

namespace Hyper.Http.Formatting
{
    /// <summary>
    /// XmlMediaTypeFormatter class.
    /// </summary>
    public class XmlMediaTypeFormatter : MediaTypeFormatter
    {
        /// <summary>
        /// Queries whether this <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> can deserializean object of the specified type.
        /// </summary>
        /// <param name="type">The type to deserialize.</param>
        /// <returns>
        /// true if the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> can deserialize the type; otherwise, false.
        /// </returns>
        public override bool CanReadType(Type type)
        {
            return HyperMediaTypeFormatter.CanReadAndWriteType(type);
        }

        /// <summary>
        /// Queries whether this <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> can serializean object of the specified type.
        /// </summary>
        /// <param name="type">The type to serialize.</param>
        /// <returns>
        /// true if the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> can serialize the type; otherwise, false.
        /// </returns>
        public override bool CanWriteType(Type type)
        {
            return HyperMediaTypeFormatter.CanReadAndWriteType(type);
        }
    }
}