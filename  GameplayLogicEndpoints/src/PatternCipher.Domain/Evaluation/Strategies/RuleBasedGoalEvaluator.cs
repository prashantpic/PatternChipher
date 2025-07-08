using PatternCipher.Domain.Evaluation.Interfaces;
using PatternCipher.Domain.Puzzles.Aggregates;
using PatternCipher.Shared.Enums;
using PatternCipher.Shared.Models.Goals;
using System;
using System.Linq;

namespace PatternCipher.Domain.Evaluation.Strategies
{
    /// <summary>
    /// A concrete implementation of the goal evaluation strategy for 'Rule-Based' puzzles.
    /// It checks if the player's grid configuration satisfies a collection of logical rules.
    /// </summary>
    public sealed class RuleBasedGoalEvaluator : IGoalEvaluationStrategy
    {
        /// <summary>
        /// Gets the puzzle type this evaluator handles, which is <see cref="PuzzleType.RuleBased"/>.
        /// </summary>
        public PuzzleType PuzzleType => PuzzleType.RuleBased;

        /// <summary>
        /// Checks if the puzzle's current grid state satisfies all logical rules
        /// defined in the puzzle's goal.
        /// </summary>
        /// <param name="puzzle">The puzzle to evaluate.</param>
        /// <returns>True if all rules are satisfied, otherwise false.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the puzzle's goal is not a <see cref="RuleBasedGoal"/>.</exception>
        public bool IsGoalMet(Puzzle puzzle)
        {
            if (puzzle.Goal is not RuleBasedGoal goal)
            {
                throw new InvalidOperationException(
                    $"The {nameof(RuleBasedGoalEvaluator)} can only evaluate puzzles with a {nameof(RuleBasedGoal)}. " +
                    $"Puzzle goal type was {puzzle.Goal.GetType().Name}.");
            }

            var currentGrid = puzzle.GetGridState();

            // The goal is met if all rules are satisfied by the current grid.
            // The All() LINQ method efficiently stops and returns false on the first rule that fails.
            return goal.Rules.All(rule => rule.IsSatisfiedBy(currentGrid));
        }
    }
}