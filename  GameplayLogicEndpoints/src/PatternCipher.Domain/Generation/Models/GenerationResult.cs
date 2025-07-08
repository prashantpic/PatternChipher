using PatternCipher.Domain.Puzzles.Aggregates;
using System;

namespace PatternCipher.Domain.Generation.Models
{
    /// <summary>
    /// A data transfer object that holds the output from a successful puzzle generation process.
    /// It encapsulates the final, validated puzzle instance.
    /// </summary>
    public sealed class GenerationResult
    {
        /// <summary>
        /// Gets the generated puzzle.
        /// </summary>
        public Puzzle Puzzle { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerationResult"/> class.
        /// </summary>
        /// <param name="puzzle">The puzzle that was generated.</param>
        public GenerationResult(Puzzle puzzle)
        {
            Puzzle = puzzle ?? throw new ArgumentNullException(nameof(puzzle));
        }
    }
}