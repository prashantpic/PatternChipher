using PatternCipher.Client.Core.Events;

namespace PatternCipher.Client.Domain.Events
{
    public enum LevelCompletionStatus
    {
        Victory,
        DefeatTimeUp,
        DefeatOutOfMoves,
        // Other defeat conditions
    }

    public class LevelCompletedEvent : GameEvent
    {
        public string LevelId { get; } // Changed from int to string to be more generic
        public LevelCompletionStatus Status { get; }
        public int FinalScore { get; }
        public int MovesTaken { get; }
        public float TimeElapsed { get; } // in seconds
        public int StarsAwarded { get; }
        // Potentially other stats like longest_combo, specials_used, etc.

        public LevelCompletedEvent(
            string levelId,
            LevelCompletionStatus status,
            int finalScore,
            int movesTaken,
            float timeElapsed,
            int starsAwarded)
        {
            LevelId = levelId;
            Status = status;
            FinalScore = finalScore;
            MovesTaken = movesTaken;
            TimeElapsed = timeElapsed;
            StarsAwarded = starsAwarded;
        }
    }
}