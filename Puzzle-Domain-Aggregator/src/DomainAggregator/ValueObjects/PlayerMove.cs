using System;

namespace PatternCipher.Domain.ValueObjects
{
    public enum MoveType
    {
        Tap,
        Swap
    }

    /// <summary>
    /// Value object representing a player's action, such as a tile swap or tap.
    /// </summary>
    public readonly struct PlayerMove : IEquatable<PlayerMove>
    {
        public MoveType Type { get; }
        public TilePosition PrimaryPosition { get; }
        public TilePosition? SecondaryPosition { get; } // Nullable for Tap moves

        // Constructor for Tap moves
        public PlayerMove(MoveType type, TilePosition primaryPosition)
        {
            if (type == MoveType.Swap)
                throw new ArgumentException("For Swap moves, SecondaryPosition must be provided.", nameof(type));

            Type = type;
            PrimaryPosition = primaryPosition;
            SecondaryPosition = null;
        }

        // Constructor for Swap moves
        public PlayerMove(MoveType type, TilePosition primaryPosition, TilePosition secondaryPosition)
        {
            if (type == MoveType.Tap)
                throw new ArgumentException("For Tap moves, SecondaryPosition should not be provided.", nameof(type));
            
            if (primaryPosition == secondaryPosition)
                throw new ArgumentException("Primary and Secondary positions cannot be the same for a swap move.");

            Type = type;
            PrimaryPosition = primaryPosition;
            SecondaryPosition = secondaryPosition;
        }

        public override bool Equals(object? obj)
        {
            return obj is PlayerMove move && Equals(move);
        }

        public bool Equals(PlayerMove other)
        {
            return Type == other.Type &&
                   PrimaryPosition.Equals(other.PrimaryPosition) &&
                   Nullable.Equals(SecondaryPosition, other.SecondaryPosition);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, PrimaryPosition, SecondaryPosition);
        }

        public static bool operator ==(PlayerMove left, PlayerMove right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlayerMove left, PlayerMove right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return SecondaryPosition.HasValue
                ? $"{Type}: {PrimaryPosition} <-> {SecondaryPosition.Value}"
                : $"{Type}: {PrimaryPosition}";
        }
    }
}