using System.Collections.Generic;
using PatternCipher.Client.Domain.ValueObjects; // For GridPosition

namespace PatternCipher.Client.Domain.ValueObjects
{
    // Simple struct to represent a player move within the solution path
    // This could be expanded based on how ProcessPlayerMoveCommand is structured
    public enum PlayerMoveType { Tap, Swap }

    [System.Serializable]
    public struct PlayerMove
    {
        public PlayerMoveType MoveType { get; }
        public GridPosition Position1 { get; }
        public GridPosition Position2 { get; } // Nullable or use a sentinel for Tap

        public PlayerMove(PlayerMoveType moveType, GridPosition position1, GridPosition position2 = default)
        {
            MoveType = moveType;
            Position1 = position1;
            // For Tap, Position2 might be same as Position1 or a specific invalid value
            Position2 = (moveType == PlayerMoveType.Tap) ? position1 : position2;
        }
    }

    public class SolutionPath
    {
        public IReadOnlyList<PlayerMove> Moves { get; }
        public int ParMoves { get; } // Could be Moves.Count or a separately calculated metric

        public SolutionPath(List<PlayerMove> moves, int parMoves = -1)
        {
            Moves = moves?.AsReadOnly() ?? new List<PlayerMove>().AsReadOnly();
            ParMoves = (parMoves == -1) ? Moves.Count : parMoves;
        }

        // Potentially add methods to get next move, verify a move against the path, etc.
    }
}