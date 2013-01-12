using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hyper.Http.Formatting;

namespace Hyper.Http.Serialization
{
    /// <summary>
    /// HyperSerialiser class.
    /// </summary>
    public class HyperSerialiser : IHyperSerialiser
    {
        private readonly HyperJsonMediaTypeFormatter _formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperSerialiser" /> class.
        /// </summary>
        /// <param name="types">The types.</param>
        public HyperSerialiser(IEnumerable<Type> types)
        {
            _formatter = new HyperJsonMediaTypeFormatter(types);
        }

        /// <summary>
        /// Deserialises the specified data.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="data">The data.</param>
        /// <returns>Deserialised object.</returns>
        public T Deserialise<T>(string data)
        {
            var task = _formatter.ReadFromStreamAsync(typeof(T), new MemoryStream(ASCIIEncoding.Default.GetBytes(data)), null, null);
            task.Wait();
            return (T)task.Result;
        }

        /// <summary>
        /// Serialises the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Serialised object.</returns>
        public string Serialise(object item)
        {
            var stream = new MemoryStream();
            var task = _formatter.WriteToStreamAsync(item.GetType(), item, stream, null, null);
            task.Wait();
            stream.Position = 0;
            return new StreamReader(stream).ReadToEnd();
        }
    }
}