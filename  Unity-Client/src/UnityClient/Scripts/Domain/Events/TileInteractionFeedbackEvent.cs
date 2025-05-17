using PatternCipher.Client.Core.Events;
using PatternCipher.Client.Domain.ValueObjects;

namespace PatternCipher.Client.Domain.Events
{
    public enum InteractionType // Domain-level interaction type
    {
        Tap,
        Swap,
        Match,
        Cascade,
        SpecialActivation,
        InvalidOperation
    }

    public enum FeedbackType // Requested presentation feedback
    {
        None,
        GenericSuccess,
        GenericFailure,
        TileSelect,
        TileDeselect,
        TileSwapAnimation,
        TileMatchClear,
        TileDropAnimation,
        SpecialEffectParticle,
        InvalidMoveShake,
        ObjectiveProgress,
        // Add more specific feedback types
    }

    public class TileInteractionFeedbackEvent : GameEvent
    {
        public GridPosition Position { get; } // Primary position for the feedback
        public GridPosition? Position2 { get; } // Secondary position (e.g., for a swap)
        public InteractionType Type { get; }
        public FeedbackType RequestedFeedback { get; }
        public bool WasSuccessful { get; } // Contextual success of the domain operation
        public string SymbolId { get; } // Optional: Symbol involved, if relevant for feedback
        public int ComboCount { get; } // Optional: For combo-related feedback

        public TileInteractionFeedbackEvent(
            GridPosition position,
            InteractionType type,
            FeedbackType requestedFeedback,
            bool wasSuccessful,
            GridPosition? position2 = null,
            string symbolId = null,
            int comboCount = 0)
        {
            Position = position;
            Type = type;
            RequestedFeedback = requestedFeedback;
            WasSuccessful = wasSuccessful;
            Position2 = position2;
            SymbolId = symbolId;
            ComboCount = comboCount;
        }
    }
}