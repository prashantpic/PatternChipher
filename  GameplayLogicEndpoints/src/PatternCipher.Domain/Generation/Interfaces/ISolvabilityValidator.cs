using PatternCipher.Domain.Puzzles.Aggregates;
using PatternCipher.Domain.Puzzles.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace PatternCipher.Domain.Generation.Interfaces
{
    /// <summary>
    /// Defines a contract for a service that can determine if a puzzle is solvable
    /// from its current state and find an optimal solution path.
    /// </summary>
    public interface ISolvabilityValidator
    {
        /// <summary>
        /// Attempts to find a solution for a given puzzle.
        /// </summary>
        /// <param name="puzzle">The puzzle to solve.</param>
        /// <param name="solution">When this method returns, contains the found solution path if the method returned true; otherwise, null.
        /// </param>
        /// <returns>true if a solution was found; otherwise, false.</returns>
        bool TryFindSolution(Puzzle puzzle, [MaybeNullWhen(false)] out SolutionPath? solution);
    }
}