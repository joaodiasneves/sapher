namespace Sapher.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    public class SapherConfigurationException : Exception
    {
        public SapherConfigurationException()
        {
        }

        public SapherConfigurationException(string message, params KeyValuePair<string, string>[] pairs) : base(message)
        {
            if (pairs != null)
            {
                foreach (var pair in pairs)
                {
                    base.Data.Add(pair.Key, pair.Value);
                }
            }
        }

        public SapherConfigurationException(string message) : base(message)
        {
        }

        public SapherConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SapherConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}