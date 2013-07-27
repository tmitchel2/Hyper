using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Hyper.Http.Formatting
{
    /// <summary>
    /// MediaTypeFormatterExtensions class.
    /// </summary>
    public static class MediaTypeFormatterExtensions
    {
        /// <summary>
        /// To the hyper formatter.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="mediaTypeName">Name of the media type.</param>
        /// <param name="types">The types.</param>
        /// <returns></returns>
        public static MediaTypeFormatter ToHyperFormatter(this MediaTypeFormatter formatter, string mediaTypeName, IEnumerable<Type> types)
        {
            formatter.SupportedMediaTypes.Add(new MediaTypeWithQualityHeaderValue(string.Format("application/{0}", mediaTypeName)));
            formatter.SupportedMediaTypes.Add(new MediaTypeWithQualityHeaderValue(string.Format("application/vnd.httperror+{0}", mediaTypeName)));
            foreach (var type in types)
            {
                formatter.SupportedMediaTypes.Add(formatter.GetMediaType(type));
            }

            return formatter;
        }

        /// <summary>
        /// Gets the type of the media.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// MediaTypeWithQualityHeaderValue object.
        /// </returns>
        public static MediaTypeWithQualityHeaderValue GetMediaType(this MediaTypeFormatter formatter, Type type)
        {
            var mediaTypeName = formatter.GetMediaType();

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
        /// Gets the type of the media.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <returns></returns>
        public static string GetMediaType(this MediaTypeFormatter formatter)
        {
            var mediaType = formatter.SupportedMediaTypes[0];
            var mediaTypeName = mediaType.MediaType.Substring(mediaType.MediaType.IndexOf(@"/", StringComparison.Ordinal) + 1);
            return mediaTypeName;
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
    }
}