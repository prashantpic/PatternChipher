using PatternCipher.Client.Domain.ValueObjects;

namespace PatternCipher.Client.Domain.Entities
{
    public enum TileState
    {
        Normal,
        Selected,
        Matched,
        Locked,
        Falling,
        Vanishing
        // Add more states as needed
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
            // Potentially add validation or logic before changing state
            State = newState;
            // Consider publishing an event if a tile's state change is significant domain-wise
            // e.g., GlobalEventBus.Instance.Publish(new TileStateChangedEvent(this, oldState, newState));
        }

        public void ChangeSymbol(string newSymbolId)
        {
            // Potentially add validation
            SymbolId = newSymbolId;
        }

        public void UpdatePosition(GridPosition newPosition)
        {
            // This might be needed for tiles that move (e.g. gravity)
            Position = newPosition;
        }
    }
}