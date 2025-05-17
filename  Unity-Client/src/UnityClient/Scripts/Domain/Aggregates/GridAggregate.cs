using System.Collections.Generic;
using System.Linq;
using PatternCipher.Client.Domain.Entities;
using PatternCipher.Client.Domain.ValueObjects;
using PatternCipher.Client.Core.Events; // For GlobalEventBus and GameEvent
using PatternCipher.Client.Domain.Events; // For domain-specific events
using PatternCipher.Client.Domain.Exceptions; // For InvalidGridOperationException
using UnityEngine; // For Debug.Log, remove if not desired in pure domain logic


namespace PatternCipher.Client.Domain.Aggregates
{
    public class GridAggregate
    {
        public GridDimensions Dimensions { get; private set; }
        private Dictionary<GridPosition, Tile> tiles; // Using Dictionary for easy lookup by GridPosition

        // Consider injecting GlobalEventBus if events are published from here
        private GlobalEventBus eventBus;

        public GridAggregate(GridDimensions dimensions, IEnumerable<Tile> initialTiles, GlobalEventBus bus)
        {
            Dimensions = dimensions;
            tiles = new Dictionary<GridPosition, Tile>();
            eventBus = bus;

            if (initialTiles != null)
            {
                foreach (var tile in initialTiles)
                {
                    if (Dimensions.IsValidPosition(tile.Position))
                    {
                        tiles[tile.Position] = tile;
                    }
                    else
                    {
                        // Handle error: tile position out of bounds
                        Debug.LogError($"Tile at {tile.Position} is out of grid bounds {Dimensions}.");
                    }
                }
            }
             // Fill any missing spots with empty/default tiles if necessary, or ensure initialTiles covers all.
            for(int r = 0; r < Dimensions.Rows; r++)
            {
                for(int c = 0; c < Dimensions.Columns; c++)
                {
                    var currentPos = new GridPosition(r,c);
                    if(!tiles.ContainsKey(currentPos))
                    {
                        // Decide on default tile creation, e.g., an "Empty" symbol or null.
                        // For now, let's assume initialTiles is comprehensive or this is handled by generation.
                        // tiles[currentPos] = new Tile(currentPos, "EMPTY_SYMBOL_ID", TileState.Normal);
                    }
                }
            }
        }

        public Tile GetTile(GridPosition position)
        {
            if (tiles.TryGetValue(position, out Tile tile))
            {
                return tile;
            }
            // Consider returning a NullObjectTile or throwing if position is invalid/empty
            return null;
        }

        public IEnumerable<Tile> GetAllTiles()
        {
            return tiles.Values.ToList().AsReadOnly();
        }

        public void ApplyTap(GridPosition position)
        {
            if (!Dimensions.IsValidPosition(position))
            {
                eventBus?.Publish(new TileInteractionFeedbackEvent(position, InteractionType.Tap, FeedbackType.TapInvalid, false));
                throw new InvalidGridOperationException($"Tap position {position} is outside grid dimensions.");
            }

            Tile tappedTile = GetTile(position);
            if (tappedTile == null || tappedTile.State == TileState.Locked || tappedTile.State == TileState.Empty) // Assuming Empty state
            {
                eventBus?.Publish(new TileInteractionFeedbackEvent(position, InteractionType.Tap, FeedbackType.TapInvalid, false));
                // Optionally, throw new InvalidGridOperationException($"Cannot tap tile at {position}. It is null, locked or empty.");
                return;
            }

            // --- Core Tap Logic Placeholder ---
            // Example: If tile is special, activate it. If it's part of a selection, process it.
            // For a simple match game, tap might not do much on its own without a second tap or swap.
            // Let's assume tap highlights the tile.
            if (tappedTile.State == TileState.Normal)
            {
                tappedTile.SetState(TileState.Selected);
                eventBus?.Publish(new TileInteractionFeedbackEvent(position, InteractionType.Tap, FeedbackType.TapSuccess, true)); // Or a specific "Selected" feedback
            }
            else if (tappedTile.State == TileState.Selected)
            {
                tappedTile.SetState(TileState.Normal);
                 eventBus?.Publish(new TileInteractionFeedbackEvent(position, InteractionType.Tap, FeedbackType.TapSuccess, true)); // Or a specific "Deselected" feedback
            }
            // --- End Core Tap Logic Placeholder ---
            
            // Example: PlayerMoveMadeEvent for a tap action, if taps are considered moves
            // eventBus?.Publish(new PlayerMoveMadeEvent(new PlayerMove(MoveType.Tap, position), true, 0, 1));
        }

        public void ApplySwap(GridPosition pos1, GridPosition pos2)
        {
            if (!Dimensions.IsValidPosition(pos1) || !Dimensions.IsValidPosition(pos2))
            {
                eventBus?.Publish(new TileInteractionFeedbackEvent(pos1, InteractionType.Swap, FeedbackType.SwapInvalid, false));
                throw new InvalidGridOperationException("One or both swap positions are outside grid dimensions.");
            }
            if (!pos1.IsAdjacentTo(pos2)) // Assuming IsAdjacentTo is defined in GridPosition
            {
                eventBus?.Publish(new TileInteractionFeedbackEvent(pos1, InteractionType.Swap, FeedbackType.SwapInvalid, false));
                throw new InvalidGridOperationException("Tiles must be adjacent to be swapped.");
            }

            Tile tile1 = GetTile(pos1);
            Tile tile2 = GetTile(pos2);

            if (tile1 == null || tile2 == null || tile1.State == TileState.Locked || tile2.State == TileState.Locked)
            {
                eventBus?.Publish(new TileInteractionFeedbackEvent(pos1, InteractionType.Swap, FeedbackType.SwapInvalid, false));
                // Optionally, throw new InvalidGridOperationException("Cannot swap locked or non-existent tiles.");
                return;
            }

            // Perform the swap in the dictionary
            tiles[pos1] = tile2;
            tiles[pos2] = tile1;

            // Update the positions within the Tile objects themselves
            tile1.UpdatePosition(pos2);
            tile2.UpdatePosition(pos1);
            
            // --- Swap Outcome Logic Placeholder ---
            // After swapping, check for matches. If no matches, potentially swap back or penalize.
            // For now, assume swap is always "successful" in terms of execution. Match checking is separate.
            // --- End Swap Outcome Logic Placeholder ---

            eventBus?.Publish(new TileInteractionFeedbackEvent(pos1, InteractionType.Swap, FeedbackType.SwapSuccess, true, pos2));
            // PlayerMove would need to be defined
            // eventBus?.Publish(new PlayerMoveMadeEvent(new PlayerMove(MoveType.Swap, pos1, pos2), true, 0, 2));
        }
        
        public List<List<GridPosition>> FindMatches()
        {
            var allMatches = new List<List<GridPosition>>();
            if (tiles == null || tiles.Count == 0) return allMatches;

            bool[,] visited = new bool[Dimensions.Rows, Dimensions.Columns];

            for (int r = 0; r < Dimensions.Rows; r++)
            {
                for (int c = 0; c < Dimensions.Columns; c++)
                {
                    GridPosition currentPos = new GridPosition(r, c);
                    Tile currentTile = GetTile(currentPos);

                    if (currentTile == null || currentTile.State == TileState.Empty || string.IsNullOrEmpty(currentTile.SymbolId) || currentTile.SymbolId == "EMPTY_SYMBOL_ID") continue;
                    
                    // Horizontal matches
                    if (c <= Dimensions.Columns - 3) // Check bounds for a match of 3
                    {
                        Tile tilePlus1 = GetTile(new GridPosition(r, c + 1));
                        Tile tilePlus2 = GetTile(new GridPosition(r, c + 2));
                        if (tilePlus1 != null && tilePlus2 != null &&
                            currentTile.SymbolId == tilePlus1.SymbolId && currentTile.SymbolId == tilePlus2.SymbolId)
                        {
                            var match = new List<GridPosition> { currentPos, new GridPosition(r,c+1), new GridPosition(r,c+2) };
                            // Extend match if more than 3
                            for(int k = c + 3; k < Dimensions.Columns; k++)
                            {
                                Tile nextTile = GetTile(new GridPosition(r,k));
                                if(nextTile != null && nextTile.SymbolId == currentTile.SymbolId) match.Add(new GridPosition(r,k)); else break;
                            }
                            allMatches.Add(match);
                        }
                    }

                    // Vertical matches
                    if (r <= Dimensions.Rows - 3) // Check bounds
                    {
                        Tile tilePlus1 = GetTile(new GridPosition(r + 1, c));
                        Tile tilePlus2 = GetTile(new GridPosition(r + 2, c));
                         if (tilePlus1 != null && tilePlus2 != null &&
                            currentTile.SymbolId == tilePlus1.SymbolId && currentTile.SymbolId == tilePlus2.SymbolId)
                        {
                            var match = new List<GridPosition> { currentPos, new GridPosition(r+1,c), new GridPosition(r+2,c) };
                            for(int k = r + 3; k < Dimensions.Rows; k++)
                            {
                                Tile nextTile = GetTile(new GridPosition(k,c));
                                if(nextTile != null && nextTile.SymbolId == currentTile.SymbolId) match.Add(new GridPosition(k,c)); else break;
                            }
                            allMatches.Add(match);
                        }
                    }
                }
            }
            // Deduplicate matches (e.g. a 5-match found as two 3-matches)
            // This is a naive implementation; more robust matching (e.g. flood fill or checking only unvisited) is better.
            // For this pass, we'll return potentially overlapping simple matches.
            return allMatches.GroupBy(m => string.Join(",", m.OrderBy(p => p.Row).ThenBy(p=>p.Column).Select(p => $"{p.Row}-{p.Column}")))
                             .Select(g => g.First())
                             .ToList();
        }


        public int ProcessMatches(List<List<GridPosition>> matches)
        {
            if (matches == null || matches.Count == 0) return 0;

            int scoreFromMatches = 0;
            HashSet<GridPosition> matchedPositions = new HashSet<GridPosition>();

            foreach (var match in matches)
            {
                foreach (var pos in match)
                {
                    matchedPositions.Add(pos);
                }
                scoreFromMatches += CalculateScoreForMatch(match); // Placeholder for scoring logic
                eventBus?.Publish(new TileInteractionFeedbackEvent(match.First(), InteractionType.Match, FeedbackType.MatchFound, true, match.ToArray()));
            }

            foreach (var pos in matchedPositions)
            {
                Tile matchedTile = GetTile(pos);
                if (matchedTile != null)
                {
                    // matchedTile.SetState(TileState.Matched); // Or directly to Empty/toBeRemoved
                    // For simplicity, we'll set them to a placeholder "empty" or remove them.
                    // The actual removal might be better handled by ApplyGravity which replaces them.
                    // tiles.Remove(pos); // This would require ApplyGravity to know where to fill.
                    // A common approach is to set their symbol to null/empty and let gravity handle.
                    matchedTile.ClearSymbolAndState(); // New method in Tile.cs
                    eventBus?.Publish(new TileInteractionFeedbackEvent(pos, InteractionType.Clear, FeedbackType.TileRemoved, true));
                }
            }
            return scoreFromMatches;
        }

        public void ApplyGravity()
        {
            if (tiles == null) return;

            for (int c = 0; c < Dimensions.Columns; c++)
            {
                int emptySlotRow = -1;
                // Start from bottom row up to find first empty slot
                for (int r = Dimensions.Rows - 1; r >= 0; r--)
                {
                    Tile currentTile = GetTile(new GridPosition(r, c));
                    if (currentTile == null || currentTile.State == TileState.Empty || string.IsNullOrEmpty(currentTile.SymbolId) || currentTile.SymbolId == "EMPTY_SYMBOL_ID")
                    {
                        emptySlotRow = r;
                        break;
                    }
                }

                if (emptySlotRow != -1) // If there's an empty slot in this column
                {
                    for (int r = emptySlotRow - 1; r >= 0; r--) // Iterate from above the empty slot
                    {
                        Tile tileToDrop = GetTile(new GridPosition(r, c));
                        if (tileToDrop != null && tileToDrop.State != TileState.Empty && !string.IsNullOrEmpty(tileToDrop.SymbolId) && tileToDrop.SymbolId != "EMPTY_SYMBOL_ID")
                        {
                            // Move tileToDrop to emptySlotRow
                            tiles[new GridPosition(emptySlotRow, c)] = tileToDrop;
                            tileToDrop.UpdatePosition(new GridPosition(emptySlotRow, c));

                            // Mark original spot as empty
                            // tiles.Remove(new GridPosition(r, c)); // Or set to an "empty" tile
                            Tile originalSpotTile = GetTile(new GridPosition(r,c));
                            if (originalSpotTile == tileToDrop) // ensure we are clearing the original one if it wasn't moved to collection yet
                            {
                                tiles[new GridPosition(r,c)] = new Tile(new GridPosition(r,c), "EMPTY_SYMBOL_ID", TileState.Empty); // placeholder
                            }


                            // Find next empty slot downwards from the original spot
                            emptySlotRow--;
                        }
                    }
                }
            }
            // After gravity, top rows might be empty. These need to be refilled.
            // Refill logic is typically separate or part of a game loop step.
        }
        
        public void RefillEmptyTiles(System.Func<GridPosition, string> symbolProvider)
        {
            if (tiles == null || symbolProvider == null) return;

            for (int r = 0; r < Dimensions.Rows; r++)
            {
                for (int c = 0; c < Dimensions.Columns; c++)
                {
                    GridPosition currentPos = new GridPosition(r, c);
                    Tile tile = GetTile(currentPos);
                    if (tile == null || tile.State == TileState.Empty || string.IsNullOrEmpty(tile.SymbolId) || tile.SymbolId == "EMPTY_SYMBOL_ID")
                    {
                        string newSymbolId = symbolProvider(currentPos); // Get a new symbol (e.g., random)
                        if(tile == null)
                        {
                            tiles[currentPos] = new Tile(currentPos, newSymbolId, TileState.Normal);
                        }
                        else
                        {
                            tile.Reinitialize(newSymbolId, TileState.Normal); // New method in Tile.cs
                        }
                         // eventBus?.Publish(new TileSpawnedEvent(currentPos, newSymbolId)); // Example event
                    }
                }
            }
        }


        private int CalculateScoreForMatch(List<GridPosition> match)
        {
            // Basic scoring: e.g., 10 points per tile in a match
            // More complex: longer matches = higher scores, special tile bonuses
            if (match == null) return 0;
            return match.Count * 10;
        }

        // Placeholder for PlayerMove, define it properly if used for events
        // public struct PlayerMove { /* ... */ }
    }

    // Define placeholder InteractionType, FeedbackType if not defined elsewhere yet
    // This would typically be in a shared enums file or Domain.Events
    public enum InteractionType { Tap, Swap, Match, Clear /* ... */ }
    // FeedbackType already defined in FeedbackConfigSO.cs stub, ensure consistency or centralize
}