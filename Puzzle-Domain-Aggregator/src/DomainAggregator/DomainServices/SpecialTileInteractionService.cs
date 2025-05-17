using System.Collections.Generic;
using System.Linq;
using PatternCipher.Domain.Aggregates.PuzzleInstance;
using PatternCipher.Domain.Entities;
using PatternCipher.Domain.ValueObjects;
using PatternCipher.Domain.Enums; // For TileType

namespace PatternCipher.Domain.Services
{
    /// <summary>
    /// Domain service handling the logic and effects of special tile interactions.
    /// Manages the behavior and outcomes of special tile activations and interactions within a PuzzleInstance.
    /// </summary>
    public class SpecialTileInteractionService
    {
        // Dependencies can be injected here if specific strategies or rule access is needed
        // For example: private readonly Dictionary<TileType, ISpecialTileStrategy> _strategies;
        // public SpecialTileInteractionService(Dictionary<TileType, ISpecialTileStrategy> strategies)
        // {
        // _strategies = strategies;
        // }

        public SpecialTileInteractionService()
        {
            // Default constructor, or inject dependencies if needed
        }

        /// <summary>
        /// Identifies special tiles among triggers, determines effects based on tile type and rules,
        /// and applies changes to the PuzzleInstance grid.
        /// </summary>
        /// <param name="puzzle">The puzzle instance being affected.</param>
        /// <param name="triggerTilePositions">The positions of tiles that might trigger special effects.</param>
        /// <returns>A collection of tiles whose state was changed by special tile effects.</returns>
        public IEnumerable<Tile> ApplySpecialTileEffects(
            PuzzleInstance puzzle,
            IEnumerable<TilePosition> triggerTilePositions)
        {
            var affectedTilesByEffects = new List<Tile>();
            var processedTriggers = new HashSet<TilePosition>();

            // A queue for cascading effects, if any special tile can trigger another
            var effectQueue = new Queue<TilePosition>(triggerTilePositions.Distinct());

            while(effectQueue.Any())
            {
                var currentTriggerPos = effectQueue.Dequeue();
                if (processedTriggers.Contains(currentTriggerPos)) continue;
                processedTriggers.Add(currentTriggerPos);

                Tile triggerTile = puzzle.Grid.GetTile(currentTriggerPos);

                if (triggerTile == null || triggerTile.Type == TileType.Normal) // Or any non-special type
                {
                    continue;
                }

                // Example: Use a strategy or switch based on triggerTile.Type
                // if (_strategies.TryGetValue(triggerTile.Type, out var strategy))
                // {
                //    var newlyAffected = strategy.ApplyEffect(puzzle, triggerTile);
                //    affectedTilesByEffects.AddRange(newlyAffected);
                //    // If strategy can cause cascades, add new trigger positions to queue
                // }
                // else
                // {
                // Placeholder logic for special tile effects
                switch (triggerTile.Type)
                {
                    case TileType.Transformer:
                        // Example: Transform adjacent NORMAl tiles
                        foreach (var neighborPos in puzzle.Grid.GetNeighbors(currentTriggerPos))
                        {
                            var neighborTile = puzzle.Grid.GetTile(neighborPos);
                            if (neighborTile != null && neighborTile.Type == TileType.Normal)
                            {
                                // Simulate changing symbol or type - requires ModifyTileState or similar on PuzzleInstance
                                // For now, we assume PuzzleInstance provides a way to modify tiles
                                // puzzle.ModifyTileState(neighborPos, new TileState(isLocked: false, customStateFlag: 1 /* e.g. 'Transformed' */));
                                // This service should call methods on `puzzle` or `puzzle.Grid` that in turn update the Tile entity
                                // For simplicity, let's say we modify the tile's state directly for this example,
                                // and PuzzleInstance will be responsible for creating TileStateChangedEvent from these.
                                
                                var originalState = neighborTile.State;
                                var newState = new TileState(originalState.IsLocked, originalState.IsHighlighted, 1 /* 'Transformed' */);
                                puzzle.ModifyTileState(neighborPos, newState); // PuzzleInstance internal method call
                                
                                // Add the modified neighbor to the list of affected tiles
                                var updatedNeighbor = puzzle.Grid.GetTile(neighborPos); // Re-fetch to get the updated tile
                                if (updatedNeighbor != null)
                                {
                                    affectedTilesByEffects.Add(updatedNeighbor);
                                }
                            }
                        }
                        break;
                    case TileType.Locked:
                        // Locked tiles might not have active effects but passive ones (cannot be moved).
                        // Activation might occur if, e.g., a key is used on it.
                        break;
                    // Add cases for other TileTypes like Wildcard, Obstacle etc.
                    default:
                        // No special effect defined for this tile type
                        break;
                }
                // }
            }
            return affectedTilesByEffects.Distinct().ToList();
        }
    }
}