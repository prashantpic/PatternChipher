using System;

namespace PatternCipher.Domain.DomainEvents
{
    /// <summary>
    /// Domain event raised after local and cloud states have been compared.
    /// Signals the result of a state consistency check between local and cloud data.
    /// </summary>
    public class StateConsistencyCheckedEvent
    {
        public bool IsConsistent { get; }
        public string DiscrepancyDetails { get; } // Can be null if consistent

        public StateConsistencyCheckedEvent(bool isConsistent, string discrepancyDetails)
        {
            IsConsistent = isConsistent;
            DiscrepancyDetails = discrepancyDetails;
        }
    }
}