using System;

namespace PatternCipher.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing the symbol displayed on a tile.
    /// </summary>
    public readonly struct TileSymbol : IEquatable<TileSymbol>
    {
        public int Id { get; }

        public TileSymbol(int id)
        {
            Id = id;
        }

        public override bool Equals(object? obj)
        {
            return obj is TileSymbol symbol && Equals(symbol);
        }

        public bool Equals(TileSymbol other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(TileSymbol left, TileSymbol right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TileSymbol left, TileSymbol right)
        {
            return !(left == right);
        }

        public override string ToString() => $"Symbol({Id})";
    }
}