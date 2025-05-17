using PatternCipher.Client.Core.Events;
using PatternCipher.Client.Domain.ValueObjects; // For PlayerMove

namespace PatternCipher.Client.Domain.Events
{
    public class PlayerMoveMadeEvent : GameEvent
    {
        public PlayerMove Move { get; }
        public bool WasValid { get; } // Renamed from Success to WasValid to avoid conflict with general success
        public int PointsEarnedFromMove { get; } // Points directly from this move/match, not total
        public int TilesAffectedByMove { get; } // Number of tiles cleared or changed by this immediate move

        public int CurrentMoveCount { get; } // Updated move count after this move
        public int CurrentScore { get; } // Updated score after this move

        public PlayerMoveMadeEvent(
            PlayerMove move,
            bool wasValid,
            int pointsEarnedFromMove,
            int tilesAffectedByMove,
            int currentMoveCount,
            int currentScore)
        {
            Move = move;
            WasValid = wasValid;
            PointsEarnedFromMove = pointsEarnedFromMove;
            TilesAffectedByMove = tilesAffectedByMove;
            CurrentMoveCount = currentMoveCount;
            CurrentScore = currentScore;
        }
    }
}