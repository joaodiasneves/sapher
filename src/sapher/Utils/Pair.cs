namespace Sapher.Utils
{
    using System.Collections.Generic;

    public static class Pair
    {
        public static KeyValuePair<K, V> Of<K, V>(K key, V value)
        {
            return new KeyValuePair<K, V>(key, value);
        }
    }
}