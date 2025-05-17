using System.Collections.Generic;
using PatternCipher.Client.Domain.Aggregates;
using PatternCipher.Client.Domain.Entities;
using PatternCipher.Client.Domain.ValueObjects;
using PatternCipher.Client.Domain.Events;
using PatternCipher.Client.Core.Events;
using PatternCipher.Client.Domain.Exceptions;
using UnityEngine; // For Debug

namespace PatternCipher.Client.Domain.DomainServices
{
    public class GridManagementService
    {
        private readonly GlobalEventBus _eventBus;
        // private readonly SpecialTileBehaviorManager _specialTileBehaviorManager; // If special tiles are complex

        public class ProcessMoveResult
        {
            public bool Success { get; set; }
            public int ScoreGained { get; set; }
            public int TilesAffectedCount { get; set; }
            public List<GridPosition> MatchedPositions { get; set; } = new List<GridPosition>();
            // Add other relevant data like special tiles triggered, etc.
        }


        public GridManagementService(GlobalEventBus eventBus /*, SpecialTileBehaviorManager specialTileBehaviorManager */)
        {
            _eventBus = eventBus;
            // _specialTileBehaviorManager = specialTileBehaviorManager;
        }

        public ProcessMoveResult ProcessPlayerTap(GridAggregate grid, GridPosition position)
        {
            if (!grid.Dimensions.IsValidPosition(position))
            {
                throw new InvalidGridOperationException($"Invalid tap position: {position}");
            }

            Tile tappedTile = grid.GetTileAt(position);
            if (tappedTile == null || tappedTile.State == TileState.Empty || tappedTile.State == TileState.Locked)
            {
                 _eventBus.Publish(new TileInteractionFeedbackEvent(position, InteractionType.Tap, FeedbackType.TapInvalid, false));
                return new ProcessMoveResult { Success = false };
            }
            
            // Example tap logic: if it's a special tile, activate it.
            // For now, let's assume tap primarily selects or triggers simple specials.
            // If tile is part of a "tap to clear" mechanic:
            bool canTapClear = CheckTapClearCondition(grid, tappedTile); // Implement this logic

            if (canTapClear)
            {
                // Perform clear operation
                List<Tile> clearedTiles = PerformTapClear(grid, tappedTile); // Modifies grid
                int score = CalculateScoreForClear(clearedTiles);
                
                ApplyGravityAndRefill(grid); // This can be complex
                
                var cascadeResult = ProcessCascades(grid); // Check for new matches after refill

                _eventBus.Publish(new TileInteractionFeedbackEvent(position, InteractionType.Tap, FeedbackType.TapSuccess, true));
                _eventBus.Publish(new PlayerMoveMadeEvent(new PlayerMove(position), true, score + cascadeResult.ScoreGained, clearedTiles.Count + cascadeResult.TilesAffectedCount));

                return new ProcessMoveResult { Success = true, ScoreGained = score + cascadeResult.ScoreGained, TilesAffectedCount = clearedTiles.Count + cascadeResult.TilesAffectedCount };
            }


            // Default: tap does nothing actionable beyond selection (handled by Presentation/Application)
            _eventBus.Publish(new TileInteractionFeedbackEvent(position, InteractionType.Tap, FeedbackType.TapNeutral, true)); // Neutral if tap just selects
            return new ProcessMoveResult { Success = true, ScoreGained = 0, TilesAffectedCount = 0 }; // Or false if tap must do something
        }

        public ProcessMoveResult ProcessPlayerSwap(GridAggregate grid, GridPosition pos1, GridPosition pos2)
        {
            if (!grid.Dimensions.IsValidPosition(pos1) || !grid.Dimensions.IsValidPosition(pos2))
            {
                throw new InvalidGridOperationException($"Invalid swap positions: {pos1}, {pos2}");
            }
            if (!pos1.IsAdjacentTo(pos2))
            {
                 _eventBus.Publish(new TileInteractionFeedbackEvent(pos1, InteractionType.Swap, FeedbackType.SwapInvalid, false, pos2, "Tiles not adjacent."));
                throw new InvalidGridOperationException("Tiles must be adjacent to swap.");
            }

            Tile tile1 = grid.GetTileAt(pos1);
            Tile tile2 = grid.GetTileAt(pos2);

            if (tile1 == null || tile2 == null || tile1.State == TileState.Empty || tile2.State == TileState.Empty ||
                tile1.State == TileState.Locked || tile2.State == TileState.Locked)
            {
                _eventBus.Publish(new TileInteractionFeedbackEvent(pos1, InteractionType.Swap, FeedbackType.SwapInvalid, false, pos2, "Cannot swap empty or locked tiles."));
                return new ProcessMoveResult { Success = false };
            }

            // Simulate swap and check for matches
            grid.SwapTiles(pos1, pos2); // This should be an internal GridAggregate method that just swaps data

            List<MatchData> matches = grid.FindMatches(); // MatchData should contain positions and matched symbol/type
            if (matches.Count == 0)
            {
                // No match, revert swap
                grid.SwapTiles(pos1, pos2); // Swap back
                _eventBus.Publish(new TileInteractionFeedbackEvent(pos1, InteractionType.Swap, FeedbackType.SwapInvalid, false, pos2, "No match formed."));
                return new ProcessMoveResult { Success = false };
            }

            // Valid swap, process matches
            _eventBus.Publish(new TileInteractionFeedbackEvent(pos1, InteractionType.Swap, FeedbackType.SwapSuccess, true, pos2));
            
            var processResult = ProcessMatchesAndCascades(grid, matches);
            
            _eventBus.Publish(new PlayerMoveMadeEvent(new PlayerMove(pos1, pos2), true, processResult.ScoreGained, processResult.TilesAffectedCount));
            return processResult;
        }

        private ProcessMoveResult ProcessMatchesAndCascades(GridAggregate grid, List<MatchData> initialMatches)
        {
            int totalScore = 0;
            int totalTilesAffected = 0;
            List<GridPosition> allMatchedPositionsThisTurn = new List<GridPosition>();

            List<MatchData> currentMatches = initialMatches;

            while (currentMatches.Count > 0)
            {
                foreach (var match in currentMatches)
                {
                    totalScore += CalculateScoreForMatch(match);
                    totalTilesAffected += match.MatchedPositions.Count;
                    allMatchedPositionsThisTurn.AddRange(match.MatchedPositions);

                    foreach (var pos in match.MatchedPositions)
                    {
                        grid.GetTileAt(pos)?.SetState(TileState.Clearing); // Mark for clearing
                         _eventBus.Publish(new TileInteractionFeedbackEvent(pos, InteractionType.Match, FeedbackType.MatchFound, true, matchSize: match.MatchedPositions.Count));
                    }
                    // Handle special tile creation from match (e.g., 4-match creates bomb)
                    // CreateSpecialTilesFromMatch(grid, match);
                }

                grid.ClearMarkedTiles(); // Changes tiles to Empty state
                // foreach (var pos in allMatchedPositionsThisTurn)
                // {
                //      _eventBus.Publish(new TileInteractionFeedbackEvent(pos, InteractionType.Clear, FeedbackType.TileCleared, true));
                // }


                ApplyGravityAndRefill(grid);
                currentMatches = grid.FindMatches(); // Check for new matches after refill
            }
            
            return new ProcessMoveResult { Success = true, ScoreGained = totalScore, TilesAffectedCount = totalTilesAffected, MatchedPositions = allMatchedPositionsThisTurn };
        }
        
        private ProcessMoveResult ProcessCascades(GridAggregate grid) // Simplified version for after tap-clear
        {
            int totalScore = 0;
            int totalTilesAffected = 0;
            List<GridPosition> allMatchedPositionsThisTurn = new List<GridPosition>();
            List<MatchData> currentMatches = grid.FindMatches();

            while (currentMatches.Count > 0)
            {
                foreach (var match in currentMatches)
                {
                    totalScore += CalculateScoreForMatch(match);
                    totalTilesAffected += match.MatchedPositions.Count;
                    allMatchedPositionsThisTurn.AddRange(match.MatchedPositions);

                    foreach (var pos in match.MatchedPositions)
                    {
                        grid.GetTileAt(pos)?.SetState(TileState.Clearing);
                        _eventBus.Publish(new TileInteractionFeedbackEvent(pos, InteractionType.Match, FeedbackType.MatchFound, true, matchSize: match.MatchedPositions.Count));
                    }
                }
                grid.ClearMarkedTiles();
                ApplyGravityAndRefill(grid);
                currentMatches = grid.FindMatches();
            }
            return new ProcessMoveResult { Success = true, ScoreGained = totalScore, TilesAffectedCount = totalTilesAffected, MatchedPositions = allMatchedPositionsThisTurn };
        }


        private void ApplyGravityAndRefill(GridAggregate grid)
        {
            grid.ApplyGravity(); 
            // Tiles that moved due to gravity should have an event or their TileView should be notified to animate
            // Example: grid.GetMovedTiles() might return a list of (fromPos, toPos, tile)
            // foreach (var movedTileInfo in grid.GetMovedTiles()) {
            //     _eventBus.Publish(new TileMovedEvent(movedTileInfo.fromPos, movedTileInfo.toPos, movedTileInfo.tile.SymbolId));
            // }

            grid.RefillEmptyTiles();
            // New tiles appearing should have an event or their TileView should be notified
            // Example: grid.GetNewTiles() might return a list of (pos, tile)
            // foreach (var newTileInfo in grid.GetNewTiles()) {
            //      _eventBus.Publish(new TileSpawnedEvent(newTileInfo.pos, newTileInfo.tile.SymbolId));
            // }
            _eventBus.Publish(new GridStateChangedEvent(grid)); // General event for presentation to update all visuals
        }


        private int CalculateScoreForMatch(MatchData match)
        {
            // Basic scoring: e.g., 10 points per tile, bonus for longer matches
            int baseScorePerTile = 10;
            int score = match.MatchedPositions.Count * baseScorePerTile;
            if (match.MatchedPositions.Count >= 4) score += (match.MatchedPositions.Count - 3) * baseScorePerTile * 2; // Bonus
            return score;
        }
        
        private bool CheckTapClearCondition(GridAggregate grid, Tile tile)
        {
            // Implement logic: e.g., tile is a special bomb, or part of a cluster of same-colored tiles
            // For simplicity, let's say any "special" tile can be tap-cleared
            // return tile.SymbolId.StartsWith("special_"); 
            return false; // Placeholder
        }

        private List<Tile> PerformTapClear(GridAggregate grid, Tile tappedTile)
        {
            // Implement logic: e.g., clear tappedTile and adjacent tiles if it's a bomb
            List<Tile> clearedTiles = new List<Tile>();
            // ... logic to identify and mark tiles for clearing ...
            // grid.ClearMarkedTiles();
            // clearedTiles.Add(tappedTile); // Add all affected tiles
            return clearedTiles; // Placeholder
        }
        
        private int CalculateScoreForClear(List<Tile> clearedTiles)
        {
            return clearedTiles.Count * 5; // Example score
        }
    }
}