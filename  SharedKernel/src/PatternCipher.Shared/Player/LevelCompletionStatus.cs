namespace PatternCipher.Shared.Player
{
    /// <summary>
    /// Stores the completion status and best performance for a single level.
    /// </summary>
    public class LevelCompletionStatus
    {
        public string LevelId { get; set; }
        public bool IsCompleted { get; set; } = false;
        public int StarsEarned { get; set; } = 0;
        public int BestScore { get; set; } = 0;
        public int BestTimeInSeconds { get; set; } = -1; // -1 indicates not set
    }
}