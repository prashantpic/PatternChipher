using System;
using System.Runtime.Serialization;

namespace PatternCipher.Utilities.Common.Exceptions
{
    /// <summary>
    /// Public exception specific to errors encountered during configuration loading or processing.
    /// </summary>
    /// <remarks>
    /// Signals errors specifically related to configuration management, such as parsing errors or missing configuration files.
    /// </remarks>
    [Serializable]
    public class ConfigurationException : SharedUtilitiesException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        public ConfigurationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ConfigurationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing"/> in Visual Basic) if no inner exception is specified.</param>
        public ConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected ConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}