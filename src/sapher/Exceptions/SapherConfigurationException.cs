namespace Sapher.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an issue while configuring Sapher
    /// </summary>
    [Serializable]
    public class SapherConfigurationException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="SapherConfigurationException"></see> class.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="additionalInfo">Exception additional information</param>
        public SapherConfigurationException(string message, params KeyValuePair<string, string>[] additionalInfo) : base(message)
        {
            if (additionalInfo != null)
            {
                foreach (var pair in additionalInfo)
                {
                    base.Data.Add(pair.Key, pair.Value);
                }
            }
        }

        /// <summary>Initializes a new instance of the <see cref="SapherConfigurationException"></see> class.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public SapherConfigurationException(string message) : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SapherConfigurationException"></see> class.</summary>
        public SapherConfigurationException() : base()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SapherConfigurationException"></see> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public SapherConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SapherConfigurationException"></see> class with serialized data.</summary>
        /// <param name="info">The <see cref="SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"></see> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="info">info</paramref> parameter is null.</exception>
        /// <exception cref="SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0).</exception>
        protected SapherConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}