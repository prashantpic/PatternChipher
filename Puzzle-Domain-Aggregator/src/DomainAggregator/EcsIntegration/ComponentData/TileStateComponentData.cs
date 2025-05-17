using System;
using Unity.Mathematics;

namespace PatternCipher.Domain.EcsIntegration.ComponentData
{
    /// <summary>
    /// Data-only struct representing tile state for potential ECS integration.
    /// </summary>
    public readonly struct TileStateComponentData
    {
        public int2 Position { get; }
        public int TileSymbolId { get; }
        public int TileTypeId { get; } // Could be TileType enum cast to int
        public bool IsLocked { get; }
        public bool IsHighlighted { get; }

        public TileStateComponentData(
            int2 position,
            int tileSymbolId,
            int tileTypeId,
            bool isLocked,
            bool isHighlighted)
        {
            Position = position;
            TileSymbolId = tileSymbolId;
            TileTypeId = tileTypeId;
            IsLocked = isLocked;
            IsHighlighted = isHighlighted;
        }
    }
}