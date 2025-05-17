using PatternCipher.Client.Domain.Entities;
using PatternCipher.Client.Domain.ValueObjects;
using PatternCipher.Client.Core.Events; // For GlobalEventBus
using PatternCipher.Client.Domain.Events; // For LevelCompletedEvent

namespace PatternCipher.Client.Domain.Aggregates
{
    public enum LevelCompletionStatus { InProgress, Won, Lost }

    public class LevelProfileAggregate
    {
        public int LevelId { get; private set; }
        public LevelObjective Objective { get; private set; }
        public int CurrentScore { get; private set; }
        public int MovesTaken { get; private set; }
        public float TimeElapsed { get; private set; } // in seconds
        public LevelCompletionStatus CompletionStatus { get; private set; }

        // Potentially store solution path for hint system or validation
        public SolutionPath Solution { get; private set; } 
        // And original generation parameters
        public LevelGenerationParameters GenerationParameters { get; private set; }


        public LevelProfileAggregate(int levelId, LevelObjective objective, SolutionPath solution = null, LevelGenerationParameters genParams = null)
        {
            LevelId = levelId;
            Objective = objective;
            Solution = solution;
            GenerationParameters = genParams;

            CurrentScore = 0;
            MovesTaken = 0;
            TimeElapsed = 0f;
            CompletionStatus = LevelCompletionStatus.InProgress;
        }

        public void IncrementMoves()
        {
            if (CompletionStatus == LevelCompletionStatus.InProgress)
            {
                MovesTaken++;
                // Potentially check for move limit if objective has one
            }
        }

        public void AddScore(int points)
        {
            if (CompletionStatus == LevelCompletionStatus.InProgress && points > 0)
            {
                CurrentScore += points;
            }
        }

        public void UpdateTime(float deltaTime)
        {
            if (CompletionStatus == LevelCompletionStatus.InProgress)
            {
                TimeElapsed += deltaTime;
                // Potentially check for time limit if objective has one
                // if (Objective.HasTimeLimit && TimeElapsed >= Objective.TimeLimit) {
                //    LoseLevel("Time ran out!");
                // }
            }
        }

        public bool CheckCompletion(GridAggregate currentGrid)
        {
            if (CompletionStatus != LevelCompletionStatus.InProgress)
            {
                return CompletionStatus == LevelCompletionStatus.Won; // Already completed
            }

            if (Objective != null && Objective.IsCompleted(currentGrid, this))
            {
                WinLevel();
                return true;
            }
            
            // Add other loss conditions here, e.g., out of moves if applicable
            // if (Objective.HasMoveLimit && MovesTaken >= Objective.MoveLimit && !Objective.IsCompleted(currentGrid, this)) {
            //    LoseLevel("Out of moves!");
            //    return false;
            // }

            return false;
        }

        private void WinLevel()
        {
            if (CompletionStatus == LevelCompletionStatus.InProgress)
            {
                CompletionStatus = LevelCompletionStatus.Won;
                // Calculate stars, final bonuses, etc.
                int starsAwarded = CalculateStars(); 
                GlobalEventBus.Instance.Publish(new LevelCompletedEvent(LevelId, CurrentScore, MovesTaken, TimeElapsed, starsAwarded, LevelCompletionStatus.Won));
            }
        }

        public void LoseLevel(string reason = "Objective failed") // Public if external factors can cause loss
        {
            if (CompletionStatus == LevelCompletionStatus.InProgress)
            {
                CompletionStatus = LevelCompletionStatus.Lost;
                GlobalEventBus.Instance.Publish(new LevelCompletedEvent(LevelId, CurrentScore, MovesTaken, TimeElapsed, 0, LevelCompletionStatus.Lost, reason));
            }
        }

        private int CalculateStars()
        {
            // Example star calculation logic
            int stars = 1; // Base star for completion
            if (CurrentScore > (GenerationParameters?.TargetScoreForTwoStars ?? 5000)) stars = 2; // Example: TargetScoreForTwoStars from LevelGenerationParameters
            if (CurrentScore > (GenerationParameters?.TargetScoreForThreeStars ?? 10000)) stars = 3;

            // Could also depend on moves, time, etc.
            // if (MovesTaken <= (Solution?.ParMoves ?? int.MaxValue)) stars++;
            
            return UnityEngine.Mathf.Clamp(stars, 1, 3);
        }

        // For loading from persistence
        public void RestoreState(int score, int moves, float time, LevelCompletionStatus status)
        {
            CurrentScore = score;
            MovesTaken = moves;
            TimeElapsed = time;
            CompletionStatus = status;
        }
    }
}