using System;
using System.Runtime.Serialization;

namespace Qf.Core
{
    /// <summary>
    /// Base exception type.
    /// </summary>
    public class EPTException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="EPTException"/> object.
        /// </summary>
        public EPTException() { }

        /// <summary>
        /// Creates a new <see cref="EPTException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public EPTException(string message) : base(message) { }

        /// <summary>
        /// Creates a new <see cref="EPTException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public EPTException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Constructor for serializing.
        /// </summary>
        public EPTException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }
    }
}
