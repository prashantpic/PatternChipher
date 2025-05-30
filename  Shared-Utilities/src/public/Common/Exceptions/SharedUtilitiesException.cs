using System;
using System.Runtime.Serialization;

namespace PatternCipher.Utilities.Common.Exceptions
{
    /// <summary>
    /// Base public exception class for all custom exceptions thrown by the Shared-Utilities library.
    /// </summary>
    [Serializable]
    public class SharedUtilitiesException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharedUtilitiesException"/> class.
        /// </summary>
        public SharedUtilitiesException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedUtilitiesException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SharedUtilitiesException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedUtilitiesException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public SharedUtilitiesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedUtilitiesException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected SharedUtilitiesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}