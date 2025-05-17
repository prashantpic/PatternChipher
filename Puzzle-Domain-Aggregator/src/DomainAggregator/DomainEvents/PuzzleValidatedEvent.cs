using System;
using System.Collections.Generic;
using System.Linq;

namespace PatternCipher.Domain.DomainEvents
{
    /// <summary>
    /// Domain event raised when a puzzle's state has been validated.
    /// Signals that a puzzle validation process has completed, indicating its integrity or solvability status.
    /// </summary>
    public class PuzzleValidatedEvent
    {
        public Guid PuzzleId { get; }
        public bool IsValid { get; }
        public IEnumerable<string> ValidationIssues { get; }

        public PuzzleValidatedEvent(Guid puzzleId, bool isValid, IEnumerable<string> validationIssues)
        {
            PuzzleId = puzzleId;
            IsValid = isValid;
            ValidationIssues = validationIssues?.ToList().AsReadOnly() ?? new List<string>().AsReadOnly();
        }
    }
}