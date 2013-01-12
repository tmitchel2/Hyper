using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;
using Hyper.Http.Serialization;

namespace Hyper.Http.Formatting
{
    /// <summary>
    /// HyperJsonMediaTypeFormatter class.
    /// </summary>
    public class HyperJsonMediaTypeFormatter : MediaTypeFormatter
    {
        private static readonly Encoding DefaultEncodingVal = new UTF8Encoding(false, true);
        private readonly HyperJsonConverter _jsonConverter;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="HyperJsonMediaTypeFormatter" /> class.
        /// </summary>
        /// <param name="types">The types.</param>
        public HyperJsonMediaTypeFormatter(IEnumerable<Type> types)
        {
            var typeList = types.ToList();

            // Create default converter
            _jsonConverter = new HyperJsonConverter(typeList);

            // Add vendor specific json media types
            foreach (var mediaType in typeList.Select(GetMediaType))
            {
                SupportedMediaTypes.Add(mediaType);
            }

            // Add global json media type
            SupportedMediaTypes.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            SupportedMediaTypes.Add(new MediaTypeWithQualityHeaderValue("application/vnd.httperror+json"));

            // SupportedEncodings.Add(new UTF8Encoding(false, true));
            // SupportedEncodings.Add(new UnicodeEncoding(false, true, true));
        }

        /// <summary>
        /// Gets the default encoding.
        /// </summary>
        /// <value>
        /// The default encoding.
        /// </value>
        public static Encoding DefaultEncoding
        {
            get
            {
                return DefaultEncodingVal;
            }
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
                return new MediaTypeWithQualityHeaderValue("application/vnd.httperror+json");
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return type.GetGenericArguments()
                    .Single()
                    .GetCustomAttributes(typeof(HyperContractAttribute), true)
                    .Cast<HyperContractAttribute>()
                    .Select(attribute => new MediaTypeWithQualityHeaderValue(attribute.MediaType + @"list+json"))
                    .SingleOrDefault();
            }

            return type
                .GetCustomAttributes(typeof(HyperContractAttribute), true)
                .Cast<HyperContractAttribute>()
                .Select(attribute => new MediaTypeWithQualityHeaderValue(attribute.MediaType + @"+json"))
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

        /// <summary>
        /// Asynchronously deserializes an object of the specified type.
        /// </summary>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="readStream">The <see cref="T:System.IO.Stream" /> to read.</param>
        /// <param name="content">The <see cref="T:System.Net.Http.HttpContent" />, if available. It may be null.</param>
        /// <param name="formatterLogger">The <see cref="T:System.Net.Http.Formatting.IFormatterLogger" /> to log events to.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" /> whose result will be an object of the given type.
        /// </returns>
        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, System.Net.Http.HttpContent content, IFormatterLogger formatterLogger)
        {
            var serialiser = new JavaScriptSerializer();
            serialiser.RegisterConverters(new[] { _jsonConverter });
            using (var streamReader = new StreamReader(readStream, DefaultEncoding))
            {
                var data = await streamReader.ReadToEndAsync();
                return serialiser.Deserialize(data, type);
            }
        }

        /// <summary>
        /// Asynchronously writes an object of the specified type.
        /// </summary>
        /// <param name="type">The type of the object to write.</param>
        /// <param name="value">The object value to write.  It may be null.</param>
        /// <param name="writeStream">The <see cref="T:System.IO.Stream" /> to which to write.</param>
        /// <param name="content">The <see cref="T:System.Net.Http.HttpContent" /> if available. It may be null.</param>
        /// <param name="transportContext">The <see cref="T:System.Net.TransportContext" /> if available. It may be null.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" /> that will perform the write.
        /// </returns>
        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, System.Net.Http.HttpContent content, TransportContext transportContext)
        {
            var task = Task.Factory.StartNew(
                () =>
                    {
                        var serialiser = new JavaScriptSerializer();
                        serialiser.RegisterConverters(new[] { _jsonConverter });
                        using (
                            var streamWriter = new StreamWriter(writeStream, DefaultEncoding, 512, true))
                        {
                            var data = serialiser.Serialize(value);
                            streamWriter.Write(data);
                            streamWriter.Flush();
                        }
                    });
            return task;
        }
    }
}