namespace PatternCipher.Client.Application
{
    /// <summary>
    /// Defines the finite high-level states the game can be in, such as MainMenu, InGame, or Paused.
    /// This is used by the GameManager's state machine to control the overall application flow and behavior.
    /// Using a strongly-typed enum improves code readability and maintainability by preventing the use of "magic strings".
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// The initial state when the application is first starting up and initializing services.
        /// </summary>
        Initializing,
        
        /// <summary>
        /// The state when the user is in the main menu.
        /// </summary>
        MainMenu,
        
        /// <summary>
        /// The state when the user is browsing the level selection screen.
        /// </summary>
        LevelSelection,
        
        /// <summary>
        /// The state when the user is actively playing a level.
        /// </summary>
        InGame,
        
        /// <summary>
        /// The state when the game is paused during active gameplay.
        /// </summary>
        Paused,
        
        /// <summary>
        /// The state shown after a level has been successfully completed, displaying results.
        /// </summary>
        LevelComplete
    }
}