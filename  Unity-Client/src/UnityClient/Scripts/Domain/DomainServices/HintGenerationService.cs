using PatternCipher.Client.Domain.Aggregates;
using PatternCipher.Client.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq; // For LINQ
using UnityEngine; // For Debug

namespace PatternCipher.Client.Domain.DomainServices
{
    public class HintData
    {
        public PlayerMove SuggestedMove { get; }
        public string HintMessage { get; } // Optional message

        public HintData(PlayerMove suggestedMove, string hintMessage = null)
        {
            SuggestedMove = suggestedMove;
            HintMessage = hintMessage;
        }
    }

    public class HintGenerationService
    {
        // This service might need to maintain some state about hint usage if there are costs or limits.
        // For now, it's stateless and just provides a hint from the solution path.

        public HintData GetHint(GridAggregate currentGrid, SolutionPath solution)
        {
            if (solution == null || solution.Moves == null || solution.Moves.Count == 0)
            {
                Debug.LogWarning("HintGenerationService: No solution path available to provide a hint.");
                return null;
            }

            // A simple hint system: suggest the first move from the pre-calculated solution path
            // that is still applicable or makes sense from the currentGrid state.
            // This is naive because the player might have deviated from the optimal path.
            // A more advanced system would re-evaluate from currentGrid or find a "next best move".

            // For this example, let's just return the *first* move of the solution path.
            // In a real game, you'd want to find the next logical step from `currentGrid`
            // that aligns with `solution` or re-solve from `currentGrid` for a short sequence.

            PlayerMove firstMove = solution.Moves.FirstOrDefault();
            if (firstMove != null)
            {
                // Validate if this move is still somewhat valid or makes sense on currentGrid
                // (e.g., are the tiles involved still present and swappable/tappable?)
                // This validation depends heavily on game mechanics.
                if (IsMoveStillPotentiallyValid(currentGrid, firstMove))
                {
                     Debug.Log($"HintGenerationService: Suggesting move from solution path: {firstMove}");
                    return new HintData(firstMove, "Try this move!");
                }
                else
                {
                    // The start of the pre-calculated path might no longer be directly applicable.
                    // Fallback: find *any* valid move using a quick check (similar to PuzzleSolvingService's CanSwapCreateMatch)
                    // This would be a "recovery" hint rather than an optimal path hint.
                    PlayerMove emergencyHint = FindAnyValidMove(currentGrid);
                    if (emergencyHint != null)
                    {
                        Debug.Log($"HintGenerationService: Solution path move invalid, suggesting emergency hint: {emergencyHint}");
                        return new HintData(emergencyHint, "Here's a possible move!");
                    }
                }
            }
            
            Debug.LogWarning("HintGenerationService: Could not determine a valid hint.");
            return null;
        }
        
        private bool IsMoveStillPotentiallyValid(GridAggregate grid, PlayerMove move)
        {
            if (move.Type == MoveType.Tap)
            {
                Tile t = grid.GetTileAt(move.Position1);
                return t != null && t.State != TileState.Empty && t.State != TileState.Locked;
            }
            else // Swap
            {
                Tile t1 = grid.GetTileAt(move.Position1);
                Tile t2 = grid.GetTileAt(move.Position2);
                return t1 != null && t2 != null && 
                       t1.State != TileState.Empty && t2.State != TileState.Empty &&
                       t1.State != TileState.Locked && t2.State != TileState.Locked &&
                       move.Position1.IsAdjacentTo(move.Position2);
            }
        }

        // This is a very simplified version of finding *any* move.
        // A real game might use a partial solver or heuristics.
        private PlayerMove FindAnyValidMove(GridAggregate grid)
        {
            GridDimensions dimensions = grid.Dimensions;

            // Check horizontal swaps
            for (int r = 0; r < dimensions.Rows; r++)
            {
                for (int c = 0; c < dimensions.Columns - 1; c++)
                {
                    GridPosition pos1 = new GridPosition(r, c);
                    GridPosition pos2 = new GridPosition(r, c + 1);
                    if (CanSwapCreateMatch(grid, pos1, pos2)) return new PlayerMove(pos1, pos2);
                }
            }
            // Check vertical swaps
            for (int r = 0; r < dimensions.Rows - 1; r++)
            {
                for (int c = 0; c < dimensions.Columns; c++)
                {
                    GridPosition pos1 = new GridPosition(r, c);
                    GridPosition pos2 = new GridPosition(r + 1, c);
                    if (CanSwapCreateMatch(grid, pos1, pos2)) return new PlayerMove(pos1, pos2);
                }
            }
            // Check for tappable moves if applicable to the game
            // ...
            return null;
        }
        
        private bool CanSwapCreateMatch(GridAggregate grid, GridPosition pos1, GridPosition pos2)
        {
            GridAggregate tempGrid = grid.Clone();
            Tile tile1 = tempGrid.GetTileAt(pos1);
            Tile tile2 = tempGrid.GetTileAt(pos2);
            if (tile1 == null || tile2 == null || tile1.State == TileState.Empty || tile2.State == TileState.Empty ||
                tile1.State == TileState.Locked || tile2.State == TileState.Locked) return false;
            
            tempGrid.SwapTiles(pos1, pos2);
            return tempGrid.FindMatches().Count > 0;
        }
    }
}