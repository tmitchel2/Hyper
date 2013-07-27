using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Hyper.Http.Serialization;

namespace Hyper.Http.Formatting
{
    /// <summary>
    /// HyperJsonMediaTypeFormatter class.
    /// </summary>
    public class JsonMediaTypeFormatter : MediaTypeFormatter
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
            return MediaTypeFormatterExtensions.CanReadAndWriteType(type);
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
            return MediaTypeFormatterExtensions.CanReadAndWriteType(type);
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
            var encoding = new UTF8Encoding(false, true);
            var serialiser = new JavaScriptSerializer();
            var jsonConverter = new HyperJsonConverter(new[] { type });
            serialiser.RegisterConverters(new[] { jsonConverter });
            using (var streamReader = new StreamReader(readStream, encoding))
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
                        var encoding = new UTF8Encoding(false, true);
                        var serialiser = new JavaScriptSerializer();
                        var jsonConverter = new HyperJsonConverter(new[] { type });
                        serialiser.RegisterConverters(new[] { jsonConverter });
                        using (var streamWriter = new StreamWriter(writeStream, encoding, 512, true))
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