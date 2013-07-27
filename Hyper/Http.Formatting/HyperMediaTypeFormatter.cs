using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace Hyper.Http.Formatting
{
    /// <summary>
    /// IHyperMediaTypeFormatter interface.
    /// </summary>
    public class HyperMediaTypeFormatter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HyperMediaTypeFormatter" /> class.
        /// </summary>
        /// <param name="mediaTypeName">Name of the media type.</param>
        /// <param name="formatter">The formatter.</param>
        /// <param name="types">The types.</param>
        public HyperMediaTypeFormatter(string mediaTypeName, MediaTypeFormatter formatter, IEnumerable<Type> types)
        {
            MediaTypeName = mediaTypeName;
            Formatter = formatter;
            DefaultEncoding = new UTF8Encoding(false, true);

            Formatter.SupportedMediaTypes.Add(new MediaTypeWithQualityHeaderValue(string.Format("application/{0}", MediaTypeName)));
            Formatter.SupportedMediaTypes.Add(new MediaTypeWithQualityHeaderValue(string.Format("application/vnd.httperror+{0}", MediaTypeName)));
            foreach (var type in types)
            {
                Formatter.SupportedMediaTypes.Add(GetMediaType(type));    
            }
        }

        /// <summary>
        /// Gets the name of the media type.
        /// </summary>
        /// <value>
        /// The name of the media type.
        /// </value>
        public string MediaTypeName { get; private set; }

        /// <summary>
        /// Gets the formatter.
        /// </summary>
        /// <value>
        /// The formatter.
        /// </value>
        public MediaTypeFormatter Formatter { get; private set; }

        /// <summary>
        /// Gets the default encoding.
        /// </summary>
        /// <value>
        /// The default encoding.
        /// </value>
        public Encoding DefaultEncoding { get; private set; }

        /// <summary>
        /// Gets the type of the media.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="mediaTypeName">Name of the media type.</param>
        /// <returns>
        /// MediaTypeWithQualityHeaderValue object.
        /// </returns>
        public static MediaTypeWithQualityHeaderValue GetMediaType(Type type, string mediaTypeName)
        {
            if (type == typeof(HttpError))
            {
                return new MediaTypeWithQualityHeaderValue(string.Format("application/vnd.httperror+{0}", mediaTypeName));
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return type.GetGenericArguments()
                    .Single()
                    .GetCustomAttributes(typeof(HyperContractAttribute), true)
                    .Cast<HyperContractAttribute>()
                    .Select(attribute => new MediaTypeWithQualityHeaderValue(string.Format(@"{0}list+{1}", attribute.MediaType, mediaTypeName)))
                    .Single();
            }

            return type
                .GetCustomAttributes(typeof(HyperContractAttribute), true)
                .Cast<HyperContractAttribute>()
                .Select(attribute => new MediaTypeWithQualityHeaderValue(string.Format(@"{0}+{1}", attribute.MediaType, mediaTypeName)))
                .Single();
        }

        /// <summary>
        /// Determines whether this instance [can read and write type] the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can read and write type] the specified type; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanReadAndWriteType(Type type)
        {
            if (type == typeof(HttpError))
            {
                return true;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return type.GetGenericArguments()
                    .Single()
                    .GetCustomAttributes(typeof(HyperContractAttribute), true)
                    .Cast<HyperContractAttribute>()
                    .Any();
            }

            return type
                .GetCustomAttributes(typeof(HyperContractAttribute), true)
                .Cast<HyperContractAttribute>()
                .Any();
        }

        /// <summary>
        /// Gets the type of the media.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public MediaTypeWithQualityHeaderValue GetMediaType(Type type)
        {
            return GetMediaType(type, MediaTypeName);
        }
    }
}