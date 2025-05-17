using PatternCipher.Client.Domain.Entities;
using PatternCipher.Client.Domain.ValueObjects; // For GridPosition, LevelGenerationParameters, SolutionPath
using PatternCipher.Client.Core.Events; // For GlobalEventBus and GameEvent
using PatternCipher.Client.Domain.Events; // For LevelCompletedEvent
using UnityEngine; // For Debug.Log, remove if not desired

namespace PatternCipher.Client.Domain.Aggregates
{
    public enum LevelCompletionStatus
    {
        InProgress,
        Won,
        Lost_TimeUp,
        Lost_NoMoves,
        // Add other statuses as needed
    }

    public class LevelProfileAggregate
    {
        public int LevelId { get; private set; }
        public LevelObjective Objective { get; private set; }
        public int CurrentScore { get; private set; }
        public int MovesTaken { get; private set; }
        public float TimeElapsed { get; private set; } // in seconds
        public LevelCompletionStatus Status { get; private set; }

        // Optional: Store initial parameters or solution for reference
        // public LevelGenerationParameters GenerationParams { get; private set; }
        // public SolutionPath OptimalSolution { get; private set; }

        private GlobalEventBus eventBus;
        private float maxTimeAllowed = -1f; // -1 means no time limit, or set from objective/params
        private int maxMovesAllowed = -1; // -1 means no move limit

        public LevelProfileAggregate(
            int levelId,
            LevelObjective objective,
            GlobalEventBus bus,
            float timeLimit = -1f, // e.g., from Level data
            int moveLimit = -1 // e.g., from Level data
            // LevelGenerationParameters genParams = null, 
            // SolutionPath solution = null
            )
        {
            LevelId = levelId;
            Objective = objective;
            eventBus = bus;
            Status = LevelCompletionStatus.InProgress;

            CurrentScore = 0;
            MovesTaken = 0;
            TimeElapsed = 0f;
            maxTimeAllowed = timeLimit;
            maxMovesAllowed = moveLimit;

            // GenerationParams = genParams;
            // OptimalSolution = solution;
        }

        public void IncrementMoves()
        {
            if (Status != LevelCompletionStatus.InProgress) return;
            MovesTaken++;
            CheckLossConditions(); // Check if out of moves
        }

        public void AddScore(int points)
        {
            if (Status != LevelCompletionStatus.InProgress) return;
            if (points < 0 && CurrentScore + points < 0)
            {
                CurrentScore = 0; // Score cannot go below zero
            }
            else
            {
                CurrentScore += points;
            }
        }

        public void UpdateTime(float deltaTime)
        {
            if (Status != LevelCompletionStatus.InProgress) return;
            TimeElapsed += deltaTime;
            CheckLossConditions(); // Check if time is up
        }

        public void CheckCompletion(GridAggregate currentGrid) // Pass current grid state
        {
            if (Status != LevelCompletionStatus.InProgress) return;

            if (Objective != null && Objective.IsCompleted(currentGrid, this))
            {
                Status = LevelCompletionStatus.Won;
                PublishLevelCompletedEvent();
            }
            else
            {
                CheckLossConditions(currentGrid); // Check for no more possible moves
            }
        }
        
        private void CheckLossConditions(GridAggregate currentGrid = null)
        {
            if (Status != LevelCompletionStatus.InProgress) return;

            if (maxTimeAllowed > 0 && TimeElapsed >= maxTimeAllowed)
            {
                Status = LevelCompletionStatus.Lost_TimeUp;
                PublishLevelCompletedEvent();
                return;
            }

            if (maxMovesAllowed > 0 && MovesTaken >= maxMovesAllowed)
            {
                 // If objectives not met by now, it's a loss by move limit
                if (Objective == null || (currentGrid != null && !Objective.IsCompleted(currentGrid, this)))
                {
                    Status = LevelCompletionStatus.Lost_NoMoves; // Or a specific "Lost_MoveLimitExceeded"
                    PublishLevelCompletedEvent();
                    return;
                }
            }
            
            // Optional: Check for no more possible moves on the grid
            // This requires more complex logic, possibly a domain service call
            // if (currentGrid != null && !AreTherePossibleMoves(currentGrid))
            // {
            //     if (Objective == null || !Objective.IsCompleted(currentGrid, this))
            //     {
            //         Status = LevelCompletionStatus.Lost_NoMoves;
            //         PublishLevelCompletedEvent();
            //     }
            // }
        }

        // Placeholder for a more complex check
        // private bool AreTherePossibleMoves(GridAggregate grid)
        // {
        //     // Logic to analyze the grid and see if any valid moves exist
        //     // This might involve trying all possible swaps and checking if they lead to a match
        //     // Or, if hints are available, checking if a hint can be generated.
        //     return true; // Placeholder
        // }


        private void PublishLevelCompletedEvent()
        {
            if (eventBus == null)
            {
                Debug.LogWarning("GlobalEventBus is null in LevelProfileAggregate. Cannot publish LevelCompletedEvent.");
                return;
            }

            // Calculate stars (placeholder)
            int starsAwarded = 0;
            if (Status == LevelCompletionStatus.Won)
            {
                starsAwarded = 3; // Simple static stars for now
                if (maxMovesAllowed > 0 && MovesTaken > maxMovesAllowed / 2) starsAwarded--; // Example condition
                if (maxTimeAllowed > 0 && TimeElapsed > maxTimeAllowed / 2) starsAwarded--;   // Example condition
                if (starsAwarded < 1) starsAwarded = 1;
            }

            var completedEvent = new LevelCompletedEvent(
                LevelId,
                CurrentScore,
                MovesTaken,
                TimeElapsed,
                starsAwarded,
                Status // Pass the actual completion status
            );
            eventBus.Publish(completedEvent);
        }
    }
}