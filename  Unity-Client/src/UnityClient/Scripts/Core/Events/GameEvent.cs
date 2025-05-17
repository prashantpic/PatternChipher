namespace PatternCipher.Client.Core.Events
{
    public abstract class GameEvent
    {
        // Base class for all game events.
        // Can include common properties like Timestamp if needed in the future.
        // public System.DateTime Timestamp { get; private set; }

        protected GameEvent()
        {
            // Timestamp = System.DateTime.UtcNow;
        }
    }
}