using PatternCipher.Client.Core.Events;
using PatternCipher.Client.Domain.ValueObjects; // For PlayerMove

namespace PatternCipher.Client.Domain.Events
{
    public class PlayerMoveMadeEvent : GameEvent
    {
        public PlayerMove Move { get; }
        public bool Success { get; } // Was the move valid and processed?
        public int PointsEarned { get; } // Points from immediate matches due to this move
        public int TilesAffected { get; } // Number of tiles changed/removed/spawned by this move and its direct consequences
        
        // You might also include:
        // public int CurrentMoveCount { get; }
        // public int CurrentScore { get; }

        public PlayerMoveMadeEvent(
            PlayerMove move,
            bool success,
            int pointsEarned = 0,
            int tilesAffected = 0)
        {
            Move = move;
            Success = success;
            PointsEarned = pointsEarned;
            TilesAffected = tilesAffected;
        }
    }
}