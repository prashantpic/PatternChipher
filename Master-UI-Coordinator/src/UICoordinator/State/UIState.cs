using System.Collections.Generic;
using PatternCipher.UI.Coordinator.Navigation; // Assuming ScreenType is defined here

namespace PatternCipher.UI.Coordinator.State
{
    /// <summary>
    /// Data model for the shared UI state, representing information relevant across the UI.
    /// Contains properties relevant to the overall UI state, persisted by UIStateManager.
    /// </summary>
    [System.Serializable]
    public class UIState
    {
        /// <summary>
        /// Gets or sets the identifier for the currently selected or active level pack.
        /// </summary>
        public string CurrentLevelPackId;

        /// <summary>
        /// Gets or sets a collection of identifiers for tutorials that the user has completed.
        /// </summary>
        public HashSet<string> CompletedTutorials = new HashSet<string>();

        /// <summary>
        /// Gets or sets the last active screen the user was on.
        /// </summary>
        public ScreenType LastActiveScreen; // Assuming ScreenType is an enum defined in PatternCipher.UI.Coordinator.Navigation
    }
}