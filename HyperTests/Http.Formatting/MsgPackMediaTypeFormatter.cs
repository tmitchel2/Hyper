using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Hyper.Http.Formatting;
using MsgPack;
using MsgPack.Serialization;

namespace HyperTests.Http.Formatting
{
    public class MsgPackMediaTypeFormatter : MediaTypeFormatter
    {
        public override bool CanReadType(Type type)
        {
            return HyperMediaTypeFormatter.CanReadAndWriteType(type);
        }

        public override bool CanWriteType(Type type)
        {
            return HyperMediaTypeFormatter.CanReadAndWriteType(type);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            var tcs = new TaskCompletionSource<object>();
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                value = ((IEnumerable<object>)value).ToList();
            }

            var serializer = MessagePackSerializer.Create<dynamic>();
            serializer.Pack(stream, value);
            tcs.SetResult(null);
            return tcs.Task;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var tcs = new TaskCompletionSource<object>();
            try
            {
                var serializer = MessagePackSerializer.Create(type);
                object result;
                using (var unpacker = Unpacker.Create(stream))
                {
                    unpacker.Read();
                    result = serializer.UnpackFrom(unpacker);
                }

                tcs.SetResult(result);
            }
            catch (Exception e)
            {
                if (formatterLogger == null) throw;
                formatterLogger.LogError(String.Empty, e.Message);
                tcs.SetResult(GetDefaultValueForType(type));
            }

            return tcs.Task;
        }
    }
}