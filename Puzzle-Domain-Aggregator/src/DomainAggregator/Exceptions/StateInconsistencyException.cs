using System;

namespace PatternCipher.Domain.Exceptions
{
    /// <summary>
    /// Custom exception for detected inconsistencies between local and cloud states.
    /// Indicates that local and cloud game states are not consistent after a check.
    /// Thrown by StateConsistencyChecker when significant, unresolvable discrepancies are found.
    /// </summary>
    [Serializable]
    public class StateInconsistencyException : Exception
    {
        public StateInconsistencyException() { }
        public StateInconsistencyException(string message) : base(message) { }
        public StateInconsistencyException(string message, Exception inner) : base(message, inner) { }
        protected StateInconsistencyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}