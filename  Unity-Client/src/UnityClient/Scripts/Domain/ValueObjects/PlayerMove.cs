// This file is added because PlayerMove is used by SolutionPath and PlayerMoveMadeEvent
// and was instructed to be defined if needed.
using PatternCipher.Client.Domain.ValueObjects;

namespace PatternCipher.Client.Domain.ValueObjects
{
    public enum MoveType
    {
        Tap,
        Swap,
        // Potentially other types like Drag, ActivateSpecial
    }

    public readonly struct PlayerMove
    {
        public readonly MoveType Type;
        public readonly GridPosition Position1;
        public readonly GridPosition? Position2; // Nullable if not a swap or for single-tile moves

        public PlayerMove(MoveType type, GridPosition position1, GridPosition? position2 = null)
        {
            Type = type;
            Position1 = position1;
            Position2 = position2;

            if (type == MoveType.Swap && position2 == null)
            {
                throw new System.ArgumentException("Position2 cannot be null for a Swap move.", nameof(position2));
            }
        }

        public override string ToString()
        {
            if (Type == MoveType.Swap && Position2.HasValue)
            {
                return $"{Type}: {Position1} <-> {Position2.Value}";
            }
            return $"{Type}: {Position1}";
        }
    }
}