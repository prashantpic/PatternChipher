using System;

namespace PatternCipher.Services.Exceptions
{
    /// <summary>
    /// Custom exception for failures during par calculation.
    /// Indicates that par calculation for a successfully solved level failed.
    /// </summary>
    [Serializable]
    public class ParCalculationFailedException : Exception
    {
        public ParCalculationFailedException() { }
        public ParCalculationFailedException(string message) : base(message) { }
        public ParCalculationFailedException(string message, Exception inner) : base(message, inner) { }
        protected ParCalculationFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}