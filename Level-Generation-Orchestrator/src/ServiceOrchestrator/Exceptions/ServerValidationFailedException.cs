using System;

namespace PatternCipher.Services.Exceptions
{
    /// <summary>
    /// Custom exception for failures during server-side validation.
    /// Indicates that server-side validation of generated level data (via Firebase Functions) failed.
    /// </summary>
    [Serializable]
    public class ServerValidationFailedException : Exception
    {
        public ServerValidationFailedException() { }
        public ServerValidationFailedException(string message) : base(message) { }
        public ServerValidationFailedException(string message, Exception inner) : base(message, inner) { }
        protected ServerValidationFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}