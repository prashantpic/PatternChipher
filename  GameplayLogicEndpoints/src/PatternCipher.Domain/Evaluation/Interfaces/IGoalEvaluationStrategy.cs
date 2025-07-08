using PatternCipher.Domain.Puzzles.Aggregates;
using PatternCipher.Shared.Enums;

namespace PatternCipher.Domain.Evaluation.Interfaces
{
    /// <summary>
    /// Defines a contract for a strategy that evaluates whether a puzzle's goal has been met.
    /// This decouples the puzzle completion logic from the puzzle itself, allowing for different
    /// types of puzzle goals (e.g., direct match, rule-based) to be checked with a common mechanism.
    /// </summary>
    public interface IGoalEvaluationStrategy
    {
        /// <summary>
        /// Gets the type of puzzle this strategy is designed to evaluate.
        /// </summary>
        PuzzleType PuzzleType { get; }

        /// <summary>
        /// Determines if the goal of the specified puzzle has been achieved.
        /// </summary>
        /// <param name="puzzle">The current puzzle instance to evaluate.</param>
        /// <returns>True if the puzzle is solved according to this strategy's rules; otherwise, false.</returns>
        bool IsGoalMet(Puzzle puzzle);
    }
}