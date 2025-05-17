using System;

namespace PatternCipher.Domain.Exceptions
{
    /// <summary>
    /// Custom exception for issues related to puzzle integrity.
    /// Indicates a failure in puzzle integrity validation, such as an unsolvable state or inconsistent data.
    /// Thrown by RuleValidator or PuzzleInstance when integrity checks fail.
    /// </summary>
    [Serializable]
    public class PuzzleIntegrityException : Exception
    {
        public PuzzleIntegrityException() { }
        public PuzzleIntegrityException(string message) : base(message) { }
        public PuzzleIntegrityException(string message, Exception inner) : base(message, inner) { }
        protected PuzzleIntegrityException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}