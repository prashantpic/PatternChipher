using System;

namespace PatternCipher.Infrastructure.Persistence.Exceptions
{
    /// <summary>
    /// Custom exception thrown by the data protector when a save file's content
    /// does not match its integrity check, indicating tampering or file corruption.
    /// </summary>
    public class SaveDataCorruptionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveDataCorruptionException"/> class.
        /// </summary>
        public SaveDataCorruptionException() : base("Save data failed integrity check.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveDataCorruptionException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SaveDataCorruptionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveDataCorruptionException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public SaveDataCorruptionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}