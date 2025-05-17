namespace PatternCipher.UI.Coordinator.Input
{
    /// <summary>
    /// Interface for coordinating global UI input, particularly complex touch gestures
    /// that might span multiple UI components or require specific interpretation logic.
    /// </summary>
    public interface IGlobalInputCoordinator
    {
        /// <summary>
        /// Initializes the input coordinator, setting up necessary listeners or configurations.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Processes raw input data or high-level input events.
        /// The implementation will interpret these inputs, detect gestures,
        /// and trigger appropriate actions or UI feedback.
        /// This method might be called on an update loop or in response to specific input system events.
        /// </summary>
        void ProcessInput(); // Parameters might be added if specific input data structures are passed directly.
    }
}