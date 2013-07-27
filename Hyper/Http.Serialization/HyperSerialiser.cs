using System.IO;
using System.Net.Http.Formatting;
using System.Text;

namespace Hyper.Http.Serialization
{
    /// <summary>
    /// HyperSerialiser class.
    /// </summary>
    public class HyperSerialiser : IHyperSerialiser
    {
        private readonly MediaTypeFormatter _formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperSerialiser" /> class.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        public HyperSerialiser(MediaTypeFormatter formatter)
        {
            _formatter = formatter;
        }

        /// <summary>
        /// Deserialises the specified data.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="data">The data.</param>
        /// <returns>Deserialised object.</returns>
        public T Deserialise<T>(string data)
        {
            var task = _formatter.ReadFromStreamAsync(typeof(T), new MemoryStream(Encoding.Default.GetBytes(data)), null, null);
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