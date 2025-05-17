using PatternCipher.Client.Core.Events;
using PatternCipher.Client.Domain.ValueObjects;
// using PatternCipher.Client.Domain.Aggregates; // If GridAggregate snapshot or ID is needed

namespace PatternCipher.Client.Domain.Events
{
    public class GridInitializedEvent : GameEvent
    {
        public GridDimensions Dimensions { get; }
        // public GridAggregate InitialGridState { get; } // Could pass a snapshot or reference
        public string LevelId { get; } // Identifier for the level whose grid was initialized

        // If passing full tile data (consider performance for large grids, might be too much for an event)
        // public IEnumerable<InitialTileData> InitialTiles { get; } 
        // public struct InitialTileData { public GridPosition Position; public string SymbolId; public TileState State; }

        public GridInitializedEvent(GridDimensions dimensions, string levelId /*, GridAggregate initialGridState = null (or other data)*/)
        {
            Dimensions = dimensions;
            LevelId = levelId;
            // InitialGridState = initialGridState;
        }
    }
}