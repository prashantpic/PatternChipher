using PatternCipher.Client.Domain.ValueObjects; // For GridPosition

namespace PatternCipher.Client.Application.Commands
{
    public enum PlayerMoveType
    {
        Tap,
        Swap
        // Potentially Drag, etc.
    }

    public class ProcessPlayerMoveCommand
    {
        public PlayerMoveType MoveType { get; }
        public GridPosition Position1 { get; }
        public GridPosition? Position2 { get; } // Nullable if MoveType is Tap

        // Constructor for Tap
        public ProcessPlayerMoveCommand(PlayerMoveType moveType, GridPosition position1)
        {
            if (moveType != PlayerMoveType.Tap)
            {
                throw new System.ArgumentException("This constructor is for Tap moves only. Use the two-position constructor for other move types.", nameof(moveType));
            }
            MoveType = moveType;
            Position1 = position1;
            Position2 = null;
        }

        // Constructor for Swap (or other two-position moves)
        public ProcessPlayerMoveCommand(PlayerMoveType moveType, GridPosition position1, GridPosition position2)
        {
            if (moveType == PlayerMoveType.Tap)
            {
                 throw new System.ArgumentException("This constructor is for two-position moves like Swap. Use the single-position constructor for Tap.", nameof(moveType));
            }
            MoveType = moveType;
            Position1 = position1;
            Position2 = position2;
        }
    }
}