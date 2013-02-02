using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Hyper.Http
{
    /// <summary>
    /// DictionaryExtensions class.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool TryGetValue<T>(this IDictionary<string, object> collection, string key, out T value)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            object obj;
            if (collection.TryGetValue(key, out obj) && obj is T)
            {
                value = (T)obj;
                return true;
            }

            value = default(T);
            return false;
        }

        /// <summary>
        /// Finds the keys with prefix.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        internal static IEnumerable<KeyValuePair<string, TValue>> FindKeysWithPrefix<TValue>(this IDictionary<string, TValue> dictionary, string prefix)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            if (prefix == null)
            {
                throw new ArgumentNullException("prefix");
            }

            TValue exactMatchValue;
            if (dictionary.TryGetValue(prefix, out exactMatchValue))
            {
                yield return new KeyValuePair<string, TValue>(prefix, exactMatchValue);
            }

            foreach (KeyValuePair<string, TValue> keyValuePair in dictionary)
            {
                var key = keyValuePair.Key;
                if (key.Length > prefix.Length && key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    if (prefix.Length == 0)
                    {
                        yield return keyValuePair;
                    }
                    else
                    {
                        char charAfterPrefix = key[prefix.Length];
                        switch (charAfterPrefix)
                        {
                            case '.':
                            case '[':
                                yield return keyValuePair;
                                continue;
                            default:
                                continue;
                        }
                    }
                }
            }
        }
    }
}