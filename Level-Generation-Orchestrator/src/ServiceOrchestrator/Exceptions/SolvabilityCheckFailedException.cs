using System;

namespace PatternCipher.Services.Exceptions
{
    /// <summary>
    /// Custom exception for failures during solvability verification.
    /// Indicates that a generated level could not be verified as solvable by the puzzle solver.
    /// </summary>
    [Serializable]
    public class SolvabilityCheckFailedException : Exception
    {
        public SolvabilityCheckFailedException() { }
        public SolvabilityCheckFailedException(string message) : base(message) { }
        public SolvabilityCheckFailedException(string message, Exception inner) : base(message, inner) { }
        protected SolvabilityCheckFailedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}