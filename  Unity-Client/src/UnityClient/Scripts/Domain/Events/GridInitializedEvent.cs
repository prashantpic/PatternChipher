using PatternCipher.Client.Core.Events;
using PatternCipher.Client.Domain.ValueObjects; // For GridDimensions
// using PatternCipher.Client.Domain.Aggregates; // If you pass GridAggregate ID or snapshot

namespace PatternCipher.Client.Domain.Events
{
    public class GridInitializedEvent : GameEvent
    {
        public GridDimensions Dimensions { get; }
        // Optionally, include a snapshot of the initial tiles or an ID to fetch the GridAggregate
        // public readonly List<InitialTileData> InitialTiles; // Example
        // public readonly string GridSessionId; // Example

        public GridInitializedEvent(GridDimensions dimensions /*, List<InitialTileData> initialTiles = null, string gridSessionId = null */)
        {
            Dimensions = dimensions;
            // InitialTiles = initialTiles;
            // GridSessionId = gridSessionId;
        }
    }

    // Example placeholder for initial tile data if needed
    // public struct InitialTileData
    // {
    //     public GridPosition Position;
    //     public string SymbolId;
    //     public TileState InitialState;
    // }
}