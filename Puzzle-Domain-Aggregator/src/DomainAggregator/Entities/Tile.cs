using System;
using PatternCipher.Domain.ValueObjects;
using PatternCipher.Domain.Enums;

namespace PatternCipher.Domain.Entities
{
    /// <summary>
    /// Entity representing an individual tile within the puzzle grid.
    /// Models a tile with its unique ID within the grid, current symbol, type (normal, special), 
    /// state (e.g., locked, active), and potential special properties or behaviors.
    /// </summary>
    public class Tile
    {
        public Guid Id { get; private set; }
        public TilePosition Position { get; private set; }
        public TileSymbol Symbol { get; private set; }
        public TileType Type { get; private set; }
        public TileState State { get; private set; }
        // public Dictionary<string, object> SpecialProperties { get; private set; } // As per SDS, but not in direct plan for this file

        public Tile(Guid id, TilePosition position, TileSymbol symbol, TileType type, TileState state)
        {
            Id = id;
            Position = position;
            Symbol = symbol;
            Type = type;
            State = state;
            // SpecialProperties = new Dictionary<string, object>(); // If SpecialProperties were included
        }

        public void SetPosition(TilePosition newPosition)
        {
            Position = newPosition;
        }

        public void SetSymbol(TileSymbol newSymbol)
        {
            Symbol = newSymbol;
        }

        public void SetType(TileType newType)
        {
            Type = newType;
        }

        public void SetState(TileState newState)
        {
            State = newState;
        }

        // Method from SDS for Tile: ApplyState(TileState newState)
        // This is essentially SetState, but perhaps with more logic or event raising if needed.
        // For now, SetState covers the basic requirement.
        public void ApplyState(TileState newState)
        {
            // Potentially more complex logic here in the future
            SetState(newState);
        }
    }
}