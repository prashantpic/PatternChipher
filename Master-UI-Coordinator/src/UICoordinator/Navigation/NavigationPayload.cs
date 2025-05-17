namespace PatternCipher.UI.Coordinator.Navigation
{
    /// <summary>
    /// Base class for data passed during screen navigation.
    /// Specific screens can expect derived payload types to carry context-specific information.
    /// </summary>
    public abstract class NavigationPayload
    {
        // This base class is intentionally empty.
        // Derived classes will add specific properties.
        // For example:
        // public class GameScreenPayload : NavigationPayload
        // {
        //     public string LevelId { get; set; }
        // }
    }
}