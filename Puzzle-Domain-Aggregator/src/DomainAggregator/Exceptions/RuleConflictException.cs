using System;

namespace PatternCipher.Domain.Exceptions
{
    /// <summary>
    /// Custom exception for conflicts or errors during rule validation or application.
    /// Indicates a problem with rule evaluation, such as conflicting rules or inability to apply a rule.
    /// Thrown by RuleValidator or PuzzleInstance when rule engine encounters issues or contradictory rule outcomes.
    /// </summary>
    [Serializable]
    public class RuleConflictException : Exception
    {
        public RuleConflictException() { }
        public RuleConflictException(string message) : base(message) { }
        public RuleConflictException(string message, Exception inner) : base(message, inner) { }
        protected RuleConflictException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}