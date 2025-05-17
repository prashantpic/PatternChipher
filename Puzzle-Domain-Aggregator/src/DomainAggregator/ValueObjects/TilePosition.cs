using System;
using Unity.Mathematics;

namespace PatternCipher.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing the position of a tile on the grid.
    /// </summary>
    public readonly partial struct TilePosition : IEquatable<TilePosition>
    {
        public int X { get; }
        public int Y { get; }

        public TilePosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object? obj)
        {
            return obj is TilePosition position && Equals(position);
        }

        public bool Equals(TilePosition other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(TilePosition left, TilePosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TilePosition left, TilePosition right)
        {
            return !(left == right);
        }

        public override string ToString() => $"({X}, {Y})";

        // Explicit conversion to Unity.Mathematics.int2
        public static explicit operator int2(TilePosition position) => new int2(position.X, position.Y);

        // Explicit conversion from Unity.Mathematics.int2
        public static explicit operator TilePosition(int2 vec) => new TilePosition(vec.x, vec.y);
    }
}