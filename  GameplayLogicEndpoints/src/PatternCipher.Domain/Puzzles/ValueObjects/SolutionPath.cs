using PatternCipher.Shared.Models;
using System;
using System.Collections.Generic;

namespace PatternCipher.Domain.Puzzles.ValueObjects
{
    /// <summary>
    /// An immutable value object that encapsulates the known solution for a puzzle.
    /// This includes the specific sequence of moves and the optimal move count ('par').
    /// </summary>
    public sealed class SolutionPath
    {
        /// <summary>
        /// Gets the sequence of moves that constitute the solution.
        /// </summary>
        public IReadOnlyList<Move> Moves { get; }

        /// <summary>
        /// Gets the 'par' value for the puzzle, representing the optimal or intended number of moves to solve it.
        /// </summary>
        public int Par { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionPath"/> class.
        /// </summary>
        /// <param name="moves">The list of moves that form the solution path. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if the moves list is null.</exception>
        public SolutionPath(IReadOnlyList<Move> moves)
        {
            Moves = moves ?? throw new ArgumentNullException(nameof(moves));
            Par = moves.Count;
        }
    }
}