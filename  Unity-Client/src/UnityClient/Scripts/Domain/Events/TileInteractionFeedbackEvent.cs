using PatternCipher.Client.Core.Events;
using PatternCipher.Client.Domain.ValueObjects; // For GridPosition

namespace PatternCipher.Client.Domain.Events
{
    public enum InteractionType
    {
        Tap,
        Swap,
        Match,
        Cascade,
        TileSpawn,
        TileDestroy,
        InvalidMoveAttempt
        // Add more specific interaction types as needed
    }

    public enum FeedbackType // This could also be a more complex object
    {
        // Visual
        TileSelect,
        TileDeselect,
        TileSwapAnimation,
        TileMatchAnimation,
        TileClearAnimation,
        TileFallAnimation,
        SpecialEffectActivation,
        ScreenShake,
        // Audio
        TapSuccessSound,
        TapInvalidSound,
        SwapSound,
        MatchSound,
        CascadeSound,
        SpecialEffectSound,
        LevelGoalProgressSound
        // Add more feedback types as needed
    }

    public class TileInteractionFeedbackEvent : GameEvent
    {
        public GridPosition Position { get; } // Primary position of interaction
        public GridPosition Position2 { get; } // Secondary position (e.g., for swap)
        public InteractionType Type { get; }
        public FeedbackType RequestedFeedback { get; } // Specific feedback requested
        public bool WasSuccessful { get; } // Indicates if the underlying domain operation was valid/successful
        public string SymbolId { get; } // Optional: symbol involved, for symbol-specific feedback
        public int ScoreChange { get; } // Optional: score change related to this feedback

        public TileInteractionFeedbackEvent(
            GridPosition position,
            InteractionType type,
            FeedbackType requestedFeedback,
            bool wasSuccessful,
            GridPosition position2 = default, // Make default if not always applicable
            string symbolId = null,
            int scoreChange = 0)
        {
            Position = position;
            Position2 = position2;
            Type = type;
            RequestedFeedback = requestedFeedback;
            WasSuccessful = wasSuccessful;
            SymbolId = symbolId;
            ScoreChange = scoreChange;
        }
    }
}