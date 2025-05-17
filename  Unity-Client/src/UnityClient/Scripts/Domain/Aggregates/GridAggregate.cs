using System.Collections.Generic;
using PatternCipher.Client.Domain.Entities;
using PatternCipher.Client.Domain.ValueObjects;
using PatternCipher.Client.Domain.Events;
using PatternCipher.Client.Core.Events; // For GlobalEventBus
using PatternCipher.Client.Domain.Exceptions; // For InvalidGridOperationException

namespace PatternCipher.Client.Domain.Aggregates
{
    public class GridAggregate
    {
        public GridDimensions Dimensions { get; private set; }
        private Dictionary<GridPosition, Tile> _tiles; // Using Dictionary for quick access by GridPosition

        // Constructor requires initial tiles and dimensions.
        public GridAggregate(GridDimensions dimensions, IEnumerable<Tile> initialTiles)
        {
            Dimensions = dimensions;
            _tiles = new Dictionary<GridPosition, Tile>();
            foreach (var tile in initialTiles)
            {
                if (Dimensions.IsValidPosition(tile.Position))
                {
                    _tiles[tile.Position] = tile;
                }
                else
                {
                    // Handle error: tile position out of bounds
                    // This might throw an exception or log an error depending on design.
                    throw new System.ArgumentException($"Tile position {tile.Position} is out of bounds for grid dimensions {dimensions}.");
                }
            }
        }

        public Tile GetTile(GridPosition position)
        {
            _tiles.TryGetValue(position, out Tile tile);
            return tile; // Returns null if not found
        }

        public IEnumerable<Tile> GetAllTiles()
        {
            return _tiles.Values;
        }
        
        public void UpdateTile(Tile tile)
        {
            if (tile == null || !_tiles.ContainsKey(tile.Position))
            {
                // Log error or throw if trying to update a non-existent tile
                return;
            }
            _tiles[tile.Position] = tile;
        }


        // Example operation: ApplyTap
        // The GridManagementService would call this after validating context.
        public bool ApplyTap(GridPosition position)
        {
            if (!Dimensions.IsValidPosition(position))
            {
                throw new InvalidGridOperationException($"Tap position {position} is invalid.");
            }

            Tile tappedTile = GetTile(position);
            if (tappedTile == null || tappedTile.State == TileState.Locked) // Assuming TileState enum
            {
                GlobalEventBus.Instance.Publish(new TileInteractionFeedbackEvent(position, InteractionType.Tap, FeedbackType.Failure, false)); // Assuming these event types exist
                return false; // Tap had no effect or was invalid
            }

            // --- Core tap logic here ---
            // Example: Change tile state, trigger special effect, etc.
            // tappedTile.SetState(TileState.Selected); // Assuming SetState method and TileState enum
            // UpdateTile(tappedTile);

            // Publish feedback event
            GlobalEventBus.Instance.Publish(new TileInteractionFeedbackEvent(position, InteractionType.Tap, FeedbackType.Success, true));
            
            // Potentially publish PlayerMoveMadeEvent if tap constitutes a "move"
            // GlobalEventBus.Instance.Publish(new PlayerMoveMadeEvent(new PlayerMove(position), true, 0, 1)); // Assuming PlayerMove struct

            return true; // Tap was successful
        }

        // Example operation: ApplySwap
        public bool ApplySwap(GridPosition pos1, GridPosition pos2)
        {
            if (!Dimensions.IsValidPosition(pos1) || !Dimensions.IsValidPosition(pos2))
            {
                throw new InvalidGridOperationException("One or both swap positions are invalid.");
            }
            if (!pos1.IsAdjacentTo(pos2)) // Assuming GridPosition has IsAdjacentTo
            {
                GlobalEventBus.Instance.Publish(new TileInteractionFeedbackEvent(pos1, InteractionType.Swap, FeedbackType.Failure, false)); // And pos2
                return false; // Cannot swap non-adjacent tiles
            }

            Tile tile1 = GetTile(pos1);
            Tile tile2 = GetTile(pos2);

            if (tile1 == null || tile2 == null || tile1.State == TileState.Locked || tile2.State == TileState.Locked)
            {
                GlobalEventBus.Instance.Publish(new TileInteractionFeedbackEvent(pos1, InteractionType.Swap, FeedbackType.Failure, false));
                return false; // Cannot swap locked or non-existent tiles
            }

            // --- Core swap logic: Swap symbols or entire tiles ---
            string tempSymbolId = tile1.SymbolId;
            // tile1.ChangeSymbol(tile2.SymbolId); // Assuming Tile has ChangeSymbol
            // tile2.ChangeSymbol(tempSymbolId);
            // This is simplified; actual tile symbol/data swap logic would be here.
            // For now, let's just swap them in our dictionary (by swapping their full Tile objects after adjusting positions)
            
            // Create new tile instances with swapped SymbolIds but original positions and states for a moment
            var newTile1AtPos1 = new Tile(tile1.Position, tile2.SymbolId, tile1.State); // Tile at pos1 gets symbol of tile2
            var newTile2AtPos2 = new Tile(tile2.Position, tile1.SymbolId, tile2.State); // Tile at pos2 gets symbol of tile1

            _tiles[pos1] = newTile1AtPos1;
            _tiles[pos2] = newTile2AtPos2;


            // Publish feedback
            GlobalEventBus.Instance.Publish(new TileInteractionFeedbackEvent(pos1, InteractionType.Swap, FeedbackType.Success, true));
            GlobalEventBus.Instance.Publish(new TileInteractionFeedbackEvent(pos2, InteractionType.Swap, FeedbackType.Success, true));
            
            // Potentially publish PlayerMoveMadeEvent
            // GlobalEventBus.Instance.Publish(new PlayerMoveMadeEvent(new PlayerMove(pos1, pos2), true, 0, 2));

            return true;
        }

        // Placeholder for FindMatches. Actual logic is complex.
        public List<List<GridPosition>> FindMatches()
        {
            var allMatches = new List<List<GridPosition>>();
            // --- Match finding logic (e.g., 3+ in a row/column) ---
            // Iterate through _tiles, check neighbors, identify match groups.
            // This is highly game-specific.
            return allMatches;
        }

        // Placeholder for ProcessMatches.
        public int ProcessMatches(List<List<GridPosition>> matches)
        {
            int scoreFromMatches = 0;
            if (matches == null || matches.Count == 0) return 0;

            foreach (var matchGroup in matches)
            {
                foreach (var pos in matchGroup)
                {
                    Tile matchedTile = GetTile(pos);
                    if (matchedTile != null)
                    {
                        // Mark tile for removal or change its state
                        // matchedTile.SetState(TileState.Matched);
                        // For actual removal, might nullify it or use a placeholder "empty" symbol
                        _tiles[pos] = new Tile(pos, null, TileState.Empty); // Example: "Empty" symbol ID and state

                        scoreFromMatches += 10; // Example scoring
                        GlobalEventBus.Instance.Publish(new TileInteractionFeedbackEvent(pos, InteractionType.Match, FeedbackType.Success, true));
                    }
                }
            }
            // Publish PlayerMoveMadeEvent if matches contribute to score/progress directly from here
            // or let GridManagementService aggregate and publish.
            return scoreFromMatches;
        }

        // Placeholder for ApplyGravity.
        public bool ApplyGravity()
        {
            bool tilesMoved = false;
            // --- Gravity logic ---
            // Iterate columns from bottom up. If an empty space is found,
            // shift tiles above it downwards.
            for (int col = 0; col < Dimensions.Columns; col++)
            {
                int emptyRow = -1;
                for (int row = Dimensions.Rows - 1; row >= 0; row--) // Start from bottom
                {
                    GridPosition currentPos = new GridPosition(row, col);
                    Tile tile = GetTile(currentPos);

                    if ((tile == null || tile.State == TileState.Empty) && emptyRow == -1)
                    {
                        emptyRow = row; // Found the highest empty spot in this column streak
                    }
                    else if (tile != null && tile.State != TileState.Empty && emptyRow != -1)
                    {
                        // Move this tile down to emptyRow
                        GridPosition targetPos = new GridPosition(emptyRow, col);
                        
                        _tiles[targetPos] = new Tile(targetPos, tile.SymbolId, tile.State);
                        _tiles[currentPos] = new Tile(currentPos, null, TileState.Empty); // Old spot is now empty
                        
                        // GlobalEventBus.Instance.Publish(new TileMovedEvent(currentPos, targetPos, tile.SymbolId));
                        tilesMoved = true;
                        emptyRow--; // Next available empty spot is one row up
                    }
                }
            }
            return tilesMoved;
        }

        // Placeholder for Refill.
        public bool RefillEmptyTiles()
        {
            bool tilesRefilled = false;
            // --- Refill logic ---
            // Iterate top rows or designated entry points.
            // If a tile is empty, generate a new tile.
            for (int col = 0; col < Dimensions.Columns; col++)
            {
                for (int row = 0; row < Dimensions.Rows; row++) // Can iterate from top or where gravity stops
                {
                    GridPosition currentPos = new GridPosition(row, col);
                    Tile tile = GetTile(currentPos);
                    if (tile == null || tile.State == TileState.Empty)
                    {
                        // Generate new symbol (this logic would typically be more complex, e.g., from LevelGenerationParameters)
                        string newSymbolId = "RandomSymbol" + UnityEngine.Random.Range(1, 4); // Placeholder
                        _tiles[currentPos] = new Tile(currentPos, newSymbolId, TileState.Normal);
                        // GlobalEventBus.Instance.Publish(new TileSpawnedEvent(currentPos, newSymbolId));
                        tilesRefilled = true;
                    }
                }
            }
            return tilesRefilled;
        }
    }

    // Assuming these enums/structs are defined elsewhere, like in Domain.Events or Domain.ValueObjects
    // For compilation within this file if they are not yet generated:
    // public enum InteractionType { Tap, Swap, Match }
    // public enum FeedbackType { Success, Failure }
    // public struct PlayerMove { /* ... */ }
}