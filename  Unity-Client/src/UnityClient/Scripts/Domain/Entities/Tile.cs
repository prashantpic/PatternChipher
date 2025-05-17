using PatternCipher.Client.Domain.ValueObjects;

namespace PatternCipher.Client.Domain.Entities
{
    public enum TileState
    {
        Normal,
        Selected,
        Matched,
        Locked,
        Clearing, // For effects
        Spawning, // For effects
        // Add other states as needed
    }

    public class Tile
    {
        public GridPosition Position { get; private set; }
        public string SymbolId { get; private set; }
        public TileState State { get; private set; }

        public Tile(GridPosition position, string symbolId, TileState initialState = TileState.Normal)
        {
            Position = position;
            SymbolId = symbolId;
            State = initialState;
        }

        public void SetState(TileState newState)
        {
            // Add validation or side effects if necessary
            State = newState;
        }

        public void SetSymbol(string newSymbolId)
        {
            // Add validation or side effects if necessary
            SymbolId = newSymbolId;
        }

        public void SetPosition(GridPosition newPosition)
        {
            // Typically, position is immutable or set only during grid reshuffles
            Position = newPosition;
        }
    }
}