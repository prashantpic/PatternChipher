using System;

namespace PatternCipher.Infrastructure.Firebase.Common
{
    /// <summary>
    /// Provides a structured, service-agnostic error object for Firebase operations.
    /// </summary>
    public readonly struct FirebaseError
    {
        /// <summary>
        /// Gets the service-specific error code, typically from a Firebase SDK enum.
        /// A value of 0 may indicate a general, non-Firebase exception.
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// Gets a descriptive message for the error.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the original exception that caused this error, for logging and debugging purposes.
        /// This can be null.
        /// </summary>
        public Exception OriginalException { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirebaseError"/> struct.
        /// </summary>
        /// <param name="errorCode">The service-specific error code.</param>
        /// <param name="message">The error message.</param>
        /// <param name="originalException">The underlying exception, if any.</param>
        public FirebaseError(int errorCode, string message, Exception originalException = null)
        {
            ErrorCode = errorCode;
            Message = message ?? "An unknown error occurred.";
            OriginalException = originalException;
        }
    }
}