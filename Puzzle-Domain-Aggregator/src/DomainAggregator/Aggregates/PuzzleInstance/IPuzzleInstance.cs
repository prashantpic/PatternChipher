using System;
using System.Collections.Generic;
using PatternCipher.Domain.Entities;
using PatternCipher.Domain.ValueObjects;

namespace PatternCipher.Domain.Aggregates.PuzzleInstance
{
    /// <summary>
    /// Defines the contract for a puzzle instance, outlining its properties and operations.
    /// Exposes methods to apply player moves, check completion state, access grid data, rules, and par details.
    /// </summary>
    public interface IPuzzleInstance
    {
        /// <summary>
        /// Gets the unique identifier of the puzzle instance.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the puzzle grid.
        /// </summary>
        PuzzleGrid Grid { get; }

        /// <summary>
        /// Gets the collection of rules applicable to this puzzle instance.
        /// </summary>
        IEnumerable<RuleDefinition> Rules { get; }

        /// <summary>
        /// Gets the par details for this puzzle.
        /// </summary>
        ParDetails ParDetails { get; }

        /// <summary>
        /// Gets the current number of moves made by the player.
        /// </summary>
        int MoveCount { get; }

        /// <summary>
        /// Applies a player's move to the puzzle.
        /// </summary>
        /// <param name="move">The player move to apply.</param>
        void ApplyMove(PlayerMove move);

        /// <summary>
        /// Checks if the puzzle has been completed according to its rules.
        /// </summary>
        /// <returns>True if the puzzle is completed, false otherwise.</returns>
        bool IsCompleted();
    }
}