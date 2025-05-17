using System;

namespace PatternCipher.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing the various states a tile can be in.
    /// </summary>
    public readonly struct TileState : IEquatable<TileState>
    {
        public bool IsLocked { get; }
        public bool IsHighlighted { get; }
        public int CustomStateFlag { get; } // Can be used as a bitmask or an enum cast to int

        public TileState(bool isLocked = false, bool isHighlighted = false, int customStateFlag = 0)
        {
            IsLocked = isLocked;
            IsHighlighted = isHighlighted;
            CustomStateFlag = customStateFlag;
        }

        public override bool Equals(object? obj)
        {
            return obj is TileState state && Equals(state);
        }

        public bool Equals(TileState other)
        {
            return IsLocked == other.IsLocked &&
                   IsHighlighted == other.IsHighlighted &&
                   CustomStateFlag == other.CustomStateFlag;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsLocked, IsHighlighted, CustomStateFlag);
        }

        public static bool operator ==(TileState left, TileState right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TileState left, TileState right)
        {
            return !(left == right);
        }

        public override string ToString() => 
            $"Locked: {IsLocked}, Highlighted: {IsHighlighted}, Custom: {CustomStateFlag}";
    }
}