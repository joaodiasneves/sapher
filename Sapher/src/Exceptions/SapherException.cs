namespace Sapher.Exceptions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    public class SapherException : Exception
    {
        public SapherException()
        {
        }

        public SapherException(string message, params KeyValuePair<string, string>[] pairs) : base(message)
        {
            if(pairs != null)
            {
                foreach (var pair in pairs)
                {
                    base.Data.Add(pair.Key, pair.Value);
                }
            }
        }

        public SapherException(string message) : base(message)
        {
        }

        public SapherException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SapherException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}