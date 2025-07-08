using PatternCipher.Domain.Generation.Interfaces;
using PatternCipher.Domain.Generation.Models;
using PatternCipher.Domain.Generation.ValueObjects;
using PatternCipher.Domain.Puzzles.Aggregates;
using PatternCipher.Domain.Puzzles.Entities;
using PatternCipher.Domain.Puzzles.ValueObjects;
using PatternCipher.Shared.Models;
using PatternCipher.Shared.Models.Goals;
using PatternCipher.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PatternCipher.Domain.Generation.Services
{
    /// <summary>
    /// A domain service that orchestrates the procedural generation of puzzles.
    /// It ensures each puzzle is solvable and adheres to the specified difficulty.
    /// </summary>
    public sealed class PuzzleGenerator : IPuzzleGenerator
    {
        private readonly ISolvabilityValidator _solver;
        private readonly Random _random = new Random();
        private const int MAX_GENERATION_ATTEMPTS = 20;

        /// <summary>
        /// Initializes a new instance of the <see cref="PuzzleGenerator"/> class.
        /// </summary>
        /// <param name="solver">A solver service to validate puzzle solvability and find optimal paths.</param>
        public PuzzleGenerator(ISolvabilityValidator solver)
        {
            _solver = solver ?? throw new ArgumentNullException(nameof(solver));
        }

        /// <summary>
        /// Generates a unique, non-trivial, and guaranteed solvable puzzle.
        /// </summary>
        /// <param name="difficulty">The parameters defining the puzzle's complexity.</param>
        /// <returns>A result containing the final, validated puzzle.</returns>
        /// <exception cref="InvalidOperationException">Thrown if a valid puzzle cannot be generated within the attempt limit.</exception>
        public GenerationResult Generate(DifficultyProfile difficulty)
        {
            for (int i = 0; i < MAX_GENERATION_ATTEMPTS; i++)
            {
                // 1. Create the solved state (goal)
                var (goalGrid, goal) = CreateSolvedState(difficulty);
                if (goalGrid == null || goal == null) continue; // Could fail for unsupported types

                // 2. Shuffle the grid by applying reverse moves
                var (shuffledGrid, idealSolutionPath) = ShuffleGrid(goalGrid, difficulty);

                // 3. Create a preliminary puzzle to be validated
                var tempSolution = new SolutionPath(idealSolutionPath);
                var preliminaryPuzzle = new Puzzle(Guid.NewGuid(), shuffledGrid, goal, tempSolution);
                
                // 4. Validate with the solver to find the true optimal path
                if (_solver.TryFindSolution(preliminaryPuzzle, out var optimalSolution))
                {
                    // 5. Check if the puzzle is non-trivial
                    if (optimalSolution != null && optimalSolution.Par >= difficulty.MinimumSolutionMoves)
                    {
                        // 6. Success! Create the final puzzle with the true solution
                        var finalPuzzle = new Puzzle(preliminaryPuzzle.Id, shuffledGrid, goal, optimalSolution);
                        return new GenerationResult(finalPuzzle);
                    }
                }
            }
            
            throw new InvalidOperationException("Failed to generate a valid puzzle within the specified constraints and attempts.");
        }

        private (Grid, IPuzzleGoal) CreateSolvedState(DifficultyProfile difficulty)
        {
            // For now, only supports DirectMatch
            if (difficulty.PuzzleType != PuzzleType.DirectMatch)
            {
                 throw new NotImplementedException($"Puzzle generation for type '{difficulty.PuzzleType}' is not yet implemented.");
            }

            var symbols = Enumerable.Range(1, difficulty.UniqueSymbolCount).Select(id => new Symbol(id)).ToList();
            var tiles = new List<Tile>();
            var availableSymbols = new List<Symbol>(symbols);
            
            for (int r = 0; r < difficulty.GridHeight; r++)
            {
                for (int c = 0; c < difficulty.GridWidth; c++)
                {
                    // Simple distribution, can be improved for better layouts
                    if (!availableSymbols.Any()) availableSymbols.AddRange(symbols);
                    var symbol = availableSymbols[_random.Next(availableSymbols.Count)];
                    availableSymbols.Remove(symbol);
                    
                    tiles.Add(new Tile(new GridPosition(r, c), symbol, false));
                }
            }

            var grid = new Grid(difficulty.GridHeight, difficulty.GridWidth, tiles);
            var goal = new DirectMatchGoal(grid);
            return (grid, goal);
        }

        private (Grid, IReadOnlyList<Move>) ShuffleGrid(Grid goalGrid, DifficultyProfile difficulty)
        {
            var shuffledGrid = goalGrid.DeepCopy();
            var forwardMoves = new List<Move>();
            
            // Shuffle a bit more than the minimum moves to ensure complexity
            int shuffleCount = (int)(difficulty.MinimumSolutionMoves * 1.5) + 5;
            var validMoves = new List<Move>();

            for (int i = 0; i < shuffleCount; i++)
            {
                validMoves.Clear();
                // Find all possible valid swaps
                for (int r = 0; r < shuffledGrid.Rows; r++)
                {
                    for (int c = 0; c < shuffledGrid.Columns; c++)
                    {
                        var pos1 = new GridPosition(r, c);
                        if (c + 1 < shuffledGrid.Columns) validMoves.Add(new Move(MoveType.Swap, pos1, new GridPosition(r, c + 1)));
                        if (r + 1 < shuffledGrid.Rows) validMoves.Add(new Move(MoveType.Swap, pos1, new GridPosition(r + 1, c)));
                    }
                }

                if (!validMoves.Any()) break; // No more moves possible

                var randomMove = validMoves[_random.Next(validMoves.Count)];
                shuffledGrid.SwapTiles(randomMove.Position1, randomMove.Position2);
                
                // The forward move is the same as the reverse move (a swap is its own inverse)
                forwardMoves.Add(randomMove);
            }
            
            // The ideal path is the reverse of the shuffle sequence
            forwardMoves.Reverse();
            return (shuffledGrid, forwardMoves);
        }
    }
}