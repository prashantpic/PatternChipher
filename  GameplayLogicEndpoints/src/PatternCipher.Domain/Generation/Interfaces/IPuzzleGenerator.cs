using PatternCipher.Domain.Generation.Models;
using PatternCipher.Domain.Generation.ValueObjects;

namespace PatternCipher.Domain.Generation.Interfaces
{
    /// <summary>
    /// Defines a contract for a domain service responsible for generating new, solvable puzzles.
    /// This provides a clean, high-level interface for creating puzzles, abstracting away
    /// the complexity of procedural generation, solvability validation, and difficulty tuning.
    /// </summary>
    public interface IPuzzleGenerator
    {
        /// <summary>
        /// Generates a new puzzle based on the specified difficulty parameters.
        /// The returned puzzle is guaranteed to be solvable and non-trivial.
        /// </summary>
        /// <param name="difficulty">A profile containing parameters that control puzzle complexity.</param>
        /// <returns>A result object containing the fully constructed and validated puzzle.</returns>
        GenerationResult Generate(DifficultyProfile difficulty);
    }
}