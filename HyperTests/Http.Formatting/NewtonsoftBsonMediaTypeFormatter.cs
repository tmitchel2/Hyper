using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Hyper.Http.Formatting;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace HyperTests.Http.Formatting
{
    /// <summary>
    /// HyperJsonMediaTypeFormatter class.
    /// </summary>
    public class NewtonsoftBsonMediaTypeFormatter : NewtonsoftJsonMediaTypeFormatter
    {
        protected override JsonReader GetJsonReader(Stream readStream)
        {
            return new BsonReader(readStream);
        }

        protected override JsonWriter GetJsonWriter(Stream writeStream)
        {
            return new BsonWriter(writeStream);
        }
    }
}