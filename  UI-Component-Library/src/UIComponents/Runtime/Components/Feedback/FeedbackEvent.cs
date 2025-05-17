namespace PatternCipher.UI.Components.Feedback
{
    /// <summary>
    /// Defines specific game events that can trigger UI feedback (audio, visual, haptic).
    /// This enum is used by UIFeedbackManager to play corresponding feedback definitions.
    /// Implements REQ-6-003, REQ-6-007, and REQ-UIX-013 (related to haptics).
    /// </summary>
    public enum FeedbackEvent
    {
        // General UI Interactions
        ButtonClick,
        ButtonHover,
        ToggleOn,
        ToggleOff,
        SliderValueChanged,
        DropdownOpened,
        DropdownItemSelected,

        // Grid & Tile Interactions
        TileTap,
        TileSelect,
        TileDeselect,
        TileSwapValid,
        TileSwapInvalid,
        TileMove,
        TileLand, // After falling or being moved into place

        // Gameplay Events
        MatchFormation,
        ChainReactionTrigger,
        SpecialTileActivated,
        ObjectiveCompleted,
        LevelStart,
        LevelCompleteSuccess,
        LevelCompleteFail,
        HintDisplayed,
        PowerUpUsed,

        // Notifications & Alerts
        SuccessNotification,
        WarningNotification,
        ErrorNotification,
        TimerTickLow, // When timer is running out
        NewHighScore,

        // Other game-specific events
        SymbolCollected,
        ObstacleCleared,
        LockOpened
    }
}