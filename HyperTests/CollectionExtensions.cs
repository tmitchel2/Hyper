using System.Collections.Generic;

namespace HyperTests
{
    /// <summary>
    /// CollectionExtensions class.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="items">The items.</param>
        public static void AddRange<T>(this  ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}