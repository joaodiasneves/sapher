namespace Sapher.Utils
{
    using System.Collections.Generic;

    internal static class Pair
    {
        internal static KeyValuePair<K, V> Of<K, V>(K key, V value)
        {
            return new KeyValuePair<K, V>(key, value);
        }
    }
}