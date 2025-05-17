// Assuming GridPosition struct is defined in PatternCipher.Client.Domain.ValueObjects
// If not, a placeholder definition would be needed here or in a separate file.
// For example:
// namespace PatternCipher.Client.Domain.ValueObjects {
//     public struct GridPosition { public int Row; public int Column; /* ... equality ... */ }
// }
using PatternCipher.Client.Domain.ValueObjects;


namespace PatternCipher.Client.Application.Commands
{
    public enum MoveType
    {
        Tap,
        Swap,
        // Add other move types if necessary (e.g., Drag, MatchSequence)
    }

    public class ProcessPlayerMoveCommand
    {
        public MoveType Type { get; }
        public GridPosition Position1 { get; }
        public GridPosition Position2 { get; } // Nullable or use a specific value if Type is Tap

        // Constructor for Tap
        public ProcessPlayerMoveCommand(MoveType type, GridPosition position1)
        {
            if (type != MoveType.Tap)
            {
                // Consider throwing an ArgumentException if constructor misuse is a concern
                // For simplicity, we'll allow it but Position2 will be default.
            }
            Type = type;
            Position1 = position1;
            Position2 = default; // Or a specific "invalid" GridPosition
        }

        // Constructor for Swap
        public ProcessPlayerMoveCommand(MoveType type, GridPosition position1, GridPosition position2)
        {
            if (type != MoveType.Swap)
            {
                // Consider throwing an ArgumentException
            }
            Type = type;
            Position1 = position1;
            Position2 = position2;
        }
    }
}