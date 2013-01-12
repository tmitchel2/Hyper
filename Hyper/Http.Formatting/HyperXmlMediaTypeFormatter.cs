using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Hyper.Http.Formatting
{
    /// <summary>
    /// HyperXmlMediaTypeFormatter class.
    /// </summary>
    public class HyperXmlMediaTypeFormatter : MediaTypeFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HyperXmlMediaTypeFormatter" /> class.
        /// </summary>
        /// <param name="types">The types.</param>
        public HyperXmlMediaTypeFormatter(IEnumerable<Type> types)
        {
            // Fill out the mediatype and encoding we support
            foreach (var mediaType in types.Select(GetMediaType))
            {
                SupportedMediaTypes.Add(mediaType);
            }

            SupportedMediaTypes.Add(new MediaTypeWithQualityHeaderValue("application/vnd.httperror+xml"));
        }

        /// <summary>
        /// Gets the type of the media.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>MediaTypeWithQualityHeaderValue object.</returns>
        public static MediaTypeWithQualityHeaderValue GetMediaType(Type type)
        {
            if (type == typeof(HttpError))
            {
                return new MediaTypeWithQualityHeaderValue("application/vnd.httperror+xml");
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return type.GetGenericArguments()
                    .Single()
                    .GetCustomAttributes(typeof(HyperContractAttribute), true)
                    .Cast<HyperContractAttribute>()
                    .Select(attribute => new MediaTypeWithQualityHeaderValue(attribute.MediaType + @"list+xml"))
                    .SingleOrDefault();
            }

            return type
                .GetCustomAttributes(typeof(HyperContractAttribute), true)
                .Cast<HyperContractAttribute>()
                .Select(attribute => new MediaTypeWithQualityHeaderValue(attribute.MediaType + @"+xml"))
                .SingleOrDefault();
        }

        /// <summary>
        /// Queries whether this <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> can deserializean object of the specified type.
        /// </summary>
        /// <param name="type">The type to deserialize.</param>
        /// <returns>
        /// true if the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> can deserialize the type; otherwise, false.
        /// </returns>
        public override bool CanReadType(Type type)
        {
            return SupportedMediaTypes.Contains(GetMediaType(type));
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
            return SupportedMediaTypes.Contains(GetMediaType(type));
        }
    }
}