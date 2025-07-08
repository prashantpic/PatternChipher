using PatternCipher.Domain.Evaluation.Interfaces;
using PatternCipher.Domain.Puzzles.Entities;
using PatternCipher.Domain.Puzzles.ValueObjects;
using PatternCipher.Shared.Models;
using PatternCipher.Shared.Models.Goals;
using PatternCipher.Shared.Results;
using System;
using System.Collections.Generic;

namespace PatternCipher.Domain.Puzzles.Aggregates
{
    /// <summary>
    /// Represents the Puzzle Aggregate Root. This is the central entity for a single gameplay instance,
    /// encapsulating the grid, goal, and solution. It is responsible for enforcing all game rules
    /// and maintaining a consistent state, acting as the primary transactional boundary.
    /// </summary>
    public sealed class Puzzle
    {
        /// <summary>
        /// The unique identifier for this puzzle instance.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The objective of the puzzle (e.g., a target pattern, a set of rules).
        /// </summary>
        public IPuzzleGoal Goal { get; }

        /// <summary>
        /// The guaranteed solution path and 'par' move count for this puzzle.
        /// </summary>
        public SolutionPath Solution { get; }

        private readonly Grid _grid;
        private readonly List<Move> _moveHistory;

        /// <summary>
        /// A read-only record of all valid moves applied to the puzzle during the session.
        /// </summary>
        public IReadOnlyList<Move> MoveHistory => _moveHistory.AsReadOnly();

        /// <summary>
        /// Initializes a new, valid puzzle instance.
        /// </summary>
        /// <param name="id">The unique identifier for the puzzle.</param>
        /// <param name="grid">The initial state of the game board.</param>
        /// <param name="goal">The objective of the puzzle.</param>
        /// <param name="solution">The pre-calculated solution path and par.</param>
        public Puzzle(Guid id, Grid grid, IPuzzleGoal goal, SolutionPath solution)
        {
            Id = id;
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
            Goal = goal ?? throw new ArgumentNullException(nameof(goal));
            Solution = solution ?? throw new ArgumentNullException(nameof(solution));
            _moveHistory = new List<Move>();
        }

        /// <summary>
        /// Attempts to apply a move to the puzzle. The move is first validated, and if valid,
        /// the grid state is updated and the move is recorded.
        /// </summary>
        /// <param name="move">The move to apply.</param>
        /// <returns>A Result object indicating success or failure.</returns>
        public Result ApplyMove(Move move)
        {
            if (!_grid.IsMoveValid(move))
            {
                return Result.Failure("Invalid move. Tiles may be locked or not adjacent.");
            }

            switch (move.Type)
            {
                case Shared.Enums.MoveType.Swap:
                    _grid.SwapTiles(move.Position1, move.Position2);
                    break;
                // Add cases for other move types if they exist
            }
            
            _moveHistory.Add(move);
            return Result.Success();
        }

        /// <summary>
        /// Checks if the puzzle's goal has been met using a specific evaluation strategy.
        /// </summary>
        /// <param name="evaluator">The strategy to use for evaluating the goal.</param>
        /// <returns>True if the puzzle is solved, otherwise false.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the provided evaluator's type does not match the puzzle's goal type.</exception>
        public bool IsSolved(IGoalEvaluationStrategy evaluator)
        {
            if (evaluator.PuzzleType != this.Goal.Type)
            {
                throw new InvalidOperationException(
                    $"Mismatched evaluator. The puzzle goal type is '{this.Goal.Type}' but the evaluator is for '{evaluator.PuzzleType}'.");
            }
            return evaluator.IsGoalMet(this);
        }

        /// <summary>
        /// Gets a read-only snapshot of the current grid state.
        /// </summary>
        /// <returns>A deep copy of the current grid, ensuring the aggregate's internal state cannot be modified externally.</returns>
        public Grid GetGridState()
        {
            return _grid.DeepCopy();
        }
    }
}