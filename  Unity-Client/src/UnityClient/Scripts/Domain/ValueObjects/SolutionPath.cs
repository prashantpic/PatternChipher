using System.Collections.Generic;
using System.Linq; // For IEnumerable operations like ToList

namespace PatternCipher.Client.Domain.ValueObjects
{
    public class SolutionPath
    {
        public IReadOnlyList<PlayerMove> Moves { get; }
        public int ParMoves { get; }
        // Potentially other metrics like solution difficulty score, branches explored, etc.

        public SolutionPath(IEnumerable<PlayerMove> moves, int parMoves)
        {
            Moves = (moves ?? Enumerable.Empty<PlayerMove>()).ToList().AsReadOnly();
            ParMoves = parMoves;

            // Validate parMoves against actual moves if necessary
            if (ParMoves < 0) ParMoves = Moves.Count; // Default par to actual moves if invalid
            // Or if (ParMoves == 0 && Moves.Count > 0) ParMoves = Moves.Count;
        }

        public bool IsEmpty => Moves.Count == 0;
    }
}