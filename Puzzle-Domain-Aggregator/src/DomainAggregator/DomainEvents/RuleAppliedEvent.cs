using System;

namespace PatternCipher.Domain.DomainEvents
{
    /// <summary>
    /// Domain event raised when a specific game rule has been successfully applied or evaluated.
    /// Signals that a rule was triggered and its effects processed, or its conditions met/unmet.
    /// </summary>
    public class RuleAppliedEvent
    {
        public Guid PuzzleId { get; }
        public Guid RuleId { get; }
        public bool WasConditionMet { get; }
        public object OutcomeDetails { get; } // Can be null or specific object detailing outcome

        public RuleAppliedEvent(Guid puzzleId, Guid ruleId, bool wasConditionMet, object outcomeDetails)
        {
            PuzzleId = puzzleId;
            RuleId = ruleId;
            WasConditionMet = wasConditionMet;
            OutcomeDetails = outcomeDetails;
        }
    }
}