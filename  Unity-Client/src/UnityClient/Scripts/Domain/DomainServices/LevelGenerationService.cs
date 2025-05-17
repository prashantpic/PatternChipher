using PatternCipher.Client.Domain.Aggregates;
using PatternCipher.Client.Domain.Entities;
using PatternCipher.Client.Domain.ValueObjects;
using PatternCipher.Client.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq; // For LINQ operations like .ToList()
using UnityEngine; // For Random and Debug

namespace PatternCipher.Client.Domain.DomainServices
{
    public class GeneratedLevelData
    {
        public GridAggregate Grid { get; }
        public LevelObjective Objective { get; }
        public SolutionPath Solution { get; }

        public GeneratedLevelData(GridAggregate grid, LevelObjective objective, SolutionPath solution)
        {
            Grid = grid;
            Objective = objective;
            Solution = solution;
        }
    }

    public class LevelGenerationService
    {
        private readonly PuzzleSolvingService _puzzleSolvingService;

        public LevelGenerationService(PuzzleSolvingService puzzleSolvingService)
        {
            _puzzleSolvingService = puzzleSolvingService;
        }

        public GeneratedLevelData GenerateLevel(LevelGenerationParameters parameters)
        {
            Debug.Log($"Generating level with parameters: {parameters.GridDimensions.Rows}x{parameters.GridDimensions.Columns}, Symbols: {parameters.AllowedSymbolIds.Count}, MinMoves: {parameters.MinMovesToSolve}");

            int maxAttempts = 100; // Max attempts to generate a solvable level meeting criteria
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // 1. Create initial grid configuration
                GridAggregate initialGrid = CreateInitialGrid(parameters.GridDimensions, parameters.AllowedSymbolIds);

                // 2. Define a level objective (can be part of parameters or generated here)
                LevelObjective objective = GenerateObjective(initialGrid, parameters);
                
                // 3. Attempt to solve the puzzle
                PuzzleSolvingService.SolutionResult solutionResult = _puzzleSolvingService.Solve(initialGrid, objective);

                if (solutionResult.IsSolvable && solutionResult.Solution.ParMoves >= parameters.MinMovesToSolve)
                {
                    Debug.Log($"Level generated successfully on attempt {attempt + 1}. Par moves: {solutionResult.Solution.ParMoves}");
                    return new GeneratedLevelData(initialGrid, objective, solutionResult.Solution);
                }
                else if (solutionResult.IsSolvable)
                {
                    Debug.Log($"Attempt {attempt + 1}: Solvable, but par moves ({solutionResult.Solution.ParMoves}) less than min ({parameters.MinMovesToSolve}). Retrying.");
                }
                else
                {
                     Debug.Log($"Attempt {attempt + 1}: Not solvable. Retrying.");
                }
            }

            throw new PuzzleGenerationException($"Failed to generate a solvable level meeting criteria after {maxAttempts} attempts.");
        }

        private GridAggregate CreateInitialGrid(GridDimensions dimensions, List<string> allowedSymbolIds)
        {
            List<Tile> tiles = new List<Tile>();
            for (int r = 0; r < dimensions.Rows; r++)
            {
                for (int c = 0; c < dimensions.Columns; c++)
                {
                    string randomSymbolId = allowedSymbolIds[Random.Range(0, allowedSymbolIds.Count)];
                    tiles.Add(new Tile(new GridPosition(r, c), randomSymbolId, TileState.Normal));
                }
            }
            
            // Initial fill might create matches. Some games resolve these, others require PCG to avoid them.
            // For now, let's assume it's fine or the solver handles it.
            // A more advanced PCG would ensure no initial matches or use specific generation patterns.
            var grid = new GridAggregate(dimensions, tiles.ToArray());

            // Iteratively resolve initial matches if game rules require no matches at start
            int initialMatchResolveAttempts = 5;
            for(int i=0; i < initialMatchResolveAttempts; ++i)
            {
                var initialMatches = grid.FindMatches();
                if(initialMatches.Count == 0) break;

                foreach(var match in initialMatches)
                {
                    foreach(var pos in match.MatchedPositions)
                    {
                        string randomSymbolId = allowedSymbolIds[Random.Range(0, allowedSymbolIds.Count)];
                        grid.GetTileAt(pos)?.ChangeSymbol(randomSymbolId); // Assuming ChangeSymbol exists
                    }
                }
                if (i == initialMatchResolveAttempts -1)
                    Debug.LogWarning("Could not resolve all initial matches during PCG grid creation.");
            }

            return grid;
        }

        private LevelObjective GenerateObjective(GridAggregate grid, LevelGenerationParameters parameters)
        {
            // Example: Objective to clear a certain number of a specific symbol type
            // This can be much more complex based on game design.
            // For instance, selecting a symbol that is reasonably present on the board.
            
            string targetSymbolId = parameters.AllowedSymbolIds[Random.Range(0, parameters.AllowedSymbolIds.Count)];
            int requiredCount = Mathf.Clamp(parameters.GridDimensions.Rows * parameters.GridDimensions.Columns / 5, 5, 20); // Example count

            return new LevelObjective(
                ObjectiveType.ClearSymbolCount, // Assuming this enum type exists
                new Dictionary<string, object> {
                    { "TargetSymbolId", targetSymbolId },
                    { "RequiredCount", requiredCount }
                }
            );
        }
    }
}