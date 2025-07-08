namespace PatternCipher.Client.Scenes
{
    /// <summary>
    /// Defines an enumeration for all scenes in the game.
    /// This provides a type-safe way to reference Unity scenes by name,
    /// avoiding hardcoded strings which are prone to typos and difficult to maintain.
    /// Each enum member name must correspond exactly to a scene file name
    /// that is included in the Unity Build Settings.
    /// </summary>
    public enum SceneId
    {
        /// <summary>
        /// The initial scene that bootstraps the application and initializes persistent managers.
        /// </summary>
        Bootstrap,
        
        /// <summary>
        /// The main menu scene, the primary user interface hub.
        /// </summary>
        MainMenu,
        
        /// <summary>
        /// The scene where the core gameplay takes place.
        /// </summary>
        Game
    }
}