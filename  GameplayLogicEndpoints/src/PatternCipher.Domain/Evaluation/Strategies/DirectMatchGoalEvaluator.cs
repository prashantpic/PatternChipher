using PatternCipher.Domain.Evaluation.Interfaces;
using PatternCipher.Domain.Puzzles.Aggregates;
using PatternCipher.Domain.Puzzles.ValueObjects;
using PatternCipher.Shared.Enums;
using PatternCipher.Shared.Models.Goals;
using System;

namespace PatternCipher.Domain.Evaluation.Strategies
{
    /// <summary>
    /// A concrete implementation of the goal evaluation strategy for 'Direct Match' puzzles.
    /// It checks if the player's grid configuration exactly matches a target pattern.
    /// </summary>
    public sealed class DirectMatchGoalEvaluator : IGoalEvaluationStrategy
    {
        /// <summary>
        /// Gets the puzzle type this evaluator handles, which is <see cref="PuzzleType.DirectMatch"/>.
        /// </summary>
        public PuzzleType PuzzleType => PuzzleType.DirectMatch;

        /// <summary>
        /// Checks if the puzzle's current grid state has every tile's symbol matching
        /// the corresponding tile in the goal's target grid.
        /// </summary>
        /// <param name="puzzle">The puzzle to evaluate.</param>
        /// <returns>True if all symbols match perfectly, otherwise false.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the puzzle's goal is not a <see cref="DirectMatchGoal"/>.</exception>
        public bool IsGoalMet(Puzzle puzzle)
        {
            if (puzzle.Goal is not DirectMatchGoal goal)
            {
                throw new InvalidOperationException(
                    $"The {nameof(DirectMatchGoalEvaluator)} can only evaluate puzzles with a {nameof(DirectMatchGoal)}. " +
                    $"Puzzle goal type was {puzzle.Goal.GetType().Name}.");
            }

            var currentGrid = puzzle.GetGridState();
            var targetGrid = goal.TargetGrid;

            for (int r = 0; r < currentGrid.Rows; r++)
            {
                for (int c = 0; c < currentGrid.Columns; c++)
                {
                    var position = new GridPosition(r, c);
                    var currentTile = currentGrid.GetTileAt(position);
                    var targetTile = targetGrid.GetTileAt(position);

                    if (currentTile.Symbol.Id != targetTile.Symbol.Id)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}