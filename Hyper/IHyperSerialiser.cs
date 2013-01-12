namespace Hyper
{
    /// <summary>
    /// IHyperSerialiser interface.
    /// </summary>
    public interface IHyperSerialiser
    {
        /// <summary>
        /// Deserialises the specified data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <returns>T object.</returns>
        T Deserialise<T>(string data);

        /// <summary>
        /// Serialises the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        string Serialise(object item);
    }
}