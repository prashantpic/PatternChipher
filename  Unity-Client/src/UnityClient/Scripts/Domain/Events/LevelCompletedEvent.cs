using PatternCipher.Client.Core.Events;

namespace PatternCipher.Client.Domain.Events
{
    public enum LevelCompletionStatus { Win, Loss, Quit } // Added from spec for GameFlowService.EndLevel

    public class LevelCompletedEvent : GameEvent
    {
        public int LevelId { get; }
        public LevelCompletionStatus Status { get; }
        public int FinalScore { get; }
        public int MovesTaken { get; }
        public float TimeElapsed { get; } // in seconds
        public int StarsAwarded { get; }

        public LevelCompletedEvent(
            int levelId,
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