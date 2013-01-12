using System.Web.Script.Serialization;
using Hal;
using JsonHalTests.Util;

namespace JsonHalTests
{
    public class JavaScriptHalSerializer : IHalSerialiser
    {
        private readonly JavaScriptSerializer _serialiser;

        public JavaScriptHalSerializer()
        {
            _serialiser = new JavaScriptSerializer();
            _serialiser.RegisterConverters(new[] { new HalJavaScriptConverter() });
        }

        public T Deserialise<T>(string data)
        {
            return _serialiser.Deserialize<T>(data);
        }
    }
}