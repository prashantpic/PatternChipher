using PatternCipher.Shared.Enums;

namespace PatternCipher.Domain.Generation.ValueObjects
{
    /// <summary>
    /// A data transfer object that encapsulates all parameters for generating a new puzzle.
    /// This provides a structured and type-safe way to define the complexity of a new puzzle.
    /// </summary>
    /// <param name="GridWidth">The number of columns in the puzzle grid.</param>
    /// <param name="GridHeight">The number of rows in the puzzle grid.</param>
    /// <param name="UniqueSymbolCount">The number of distinct symbols to use in the puzzle.</param>
    /// <param name="PuzzleType">The type of puzzle to generate (e.g., DirectMatch, RuleBased).</param>
    /// <param name="MinimumSolutionMoves">The minimum number of moves the optimal solution must have for the puzzle to be considered non-trivial.</param>
    public record DifficultyProfile(
        int GridWidth,
        int GridHeight,
        int UniqueSymbolCount,
        PuzzleType PuzzleType,
        int MinimumSolutionMoves);
}