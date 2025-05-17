namespace PatternCipher.UI.Coordinator.Navigation
{
    /// <summary>
    /// Enum defining all unique screen identifiers used for navigation and asset mapping.
    /// Provides a standardized and type-safe way to identify different UI screens within the application.
    /// </summary>
    public enum ScreenType
    {
        /// <summary>
        /// Represents no screen or an uninitialized state.
        /// </summary>
        None = 0,

        /// <summary>
        /// The main menu screen.
        /// </summary>
        MainMenu = 1,

        /// <summary>
        /// The core gameplay screen.
        /// </summary>
        GameScreen = 2,

        /// <summary>
        /// The screen for selecting levels or level packs.
        /// </summary>
        LevelSelect = 3,

        /// <summary>
        /// The screen for application settings.
        /// </summary>
        SettingsScreen = 4,

        /// <summary>
        /// The pause menu screen, typically overlaid on the game screen.
        /// </summary>
        PauseMenu = 5,

        /// <summary>
        /// Screen shown upon successful completion of a level.
        /// </summary>
        LevelCompleteScreen = 6,

        /// <summary>
        /// Screen(s) used for tutorials or onboarding.
        /// </summary>
        TutorialScreen = 7
    }
}