using PatternCipher.Shared.Enums;
using PatternCipher.Shared.Gameplay.Symbols;

namespace PatternCipher.Shared.Gameplay.Grid
{
    /// <summary>
    /// Models a single tile on the game grid, holding its position, symbol, and state.
    /// </summary>
    public class Tile
    {
        public GridPosition Position { get; private set; }
        public Symbol CurrentSymbol { get; private set; }
        public TileState State { get; private set; }

        public Tile(GridPosition position, Symbol initialSymbol, TileState initialState = TileState.Default)
        {
            Position = position;
            CurrentSymbol = initialSymbol;
            State = initialState;
        }

        public void ChangeSymbol(Symbol newSymbol)
        {
            if (State == TileState.Locked) return; // Or throw exception as per domain rules
            CurrentSymbol = newSymbol;
        }

        public void ChangeState(TileState newState)
        {
            State = newState;
        }
        
        // Internal method to be called by a higher-level grid or board service.
        internal void UpdatePosition(GridPosition newPosition)
        {
            Position = newPosition;
        }
    }
}