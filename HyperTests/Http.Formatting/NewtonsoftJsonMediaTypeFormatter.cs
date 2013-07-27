using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Hyper.Http.Formatting;
using Newtonsoft.Json;

namespace HyperTests.Http.Formatting
{
    /// <summary>
    /// HyperJsonMediaTypeFormatter class.
    /// </summary>
    public class NewtonsoftJsonMediaTypeFormatter : MediaTypeFormatter
    {
        public override bool CanReadType(Type type)
        {
            return HyperMediaTypeFormatter.CanReadAndWriteType(type);
        }

        public override bool CanWriteType(Type type)
        {
            return HyperMediaTypeFormatter.CanReadAndWriteType(type);
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
        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, System.Net.Http.HttpContent content, IFormatterLogger formatterLogger)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                var reader = GetJsonReader(readStream);
                using (reader)
                {
                    var jsonSerializerSettings = new JsonSerializerSettings();
                    jsonSerializerSettings.Converters.Add(new HyperNewtonsoftJsonConverter());
                    var jsonSerializer = JsonSerializer.Create(jsonSerializerSettings);
                    var output = jsonSerializer.Deserialize(reader, type);
                    if (formatterLogger != null)
                    {
                        jsonSerializer.Error += (sender, e) =>
                            {
                                var exception = e.ErrorContext.Error;
                                formatterLogger.LogError(e.ErrorContext.Path, exception.Message);
                                e.ErrorContext.Handled = true;
                            };
                    }
                    tcs.SetResult(output);
                }
            }
            catch (Exception e)
            {
                if (formatterLogger == null) throw;
                formatterLogger.LogError(String.Empty, e.Message);
                tcs.SetResult(GetDefaultValueForType(type));
            }

            return tcs.Task;
        }

        protected virtual JsonReader GetJsonReader(Stream readStream)
        {
            return new JsonTextReader(new StreamReader(readStream));
        }

        protected virtual JsonWriter GetJsonWriter(Stream writeStream)
        {
            return new JsonTextWriter(new StreamWriter(writeStream)) { CloseOutput = false };
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
            var tcs = new TaskCompletionSource<object>();
            using (var writer = GetJsonWriter(writeStream))
            {
                var jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Converters.Add(new HyperNewtonsoftJsonConverter());
                var jsonSerializer = JsonSerializer.Create(jsonSerializerSettings);
                jsonSerializer.Error += (sender, e) =>
                    {
                        Console.WriteLine(e);
                    };

                jsonSerializer.Serialize(writer, value);
                writer.Flush();
                tcs.SetResult(null);
            }

            return tcs.Task;
        }
    }
}