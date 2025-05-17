using System.Threading.Tasks;
using PatternCipher.UI.Coordinator.Navigation;

namespace PatternCipher.UI.Coordinator.Core
{
    /// <summary>
    /// Interface for the main UI Coordinator service, defining the primary contract for UI orchestration.
    /// Defines the contract for the central UI coordination service, managing screen flow, theme, layout, and state.
    /// </summary>
    public interface IUICoordinatorService
    {
        /// <summary>
        /// Initializes the UI Coordinator service and its sub-components.
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Navigates to the specified screen.
        /// </summary>
        /// <param name="screenType">The type of screen to navigate to.</param>
        /// <param name="payload">Optional data to pass to the screen.</param>
        Task ShowScreenAsync(ScreenType screenType, NavigationPayload payload);

        /// <summary>
        /// Navigates to the previous screen in the history, if available.
        /// </summary>
        Task GoBackAsync();

        /// <summary>
        /// Applies the currently configured theme and accessibility settings to all registered UI elements.
        /// </summary>
        void ApplyCurrentThemeAndAccessibility();

        /// <summary>
        /// Triggers a recalculation and update of layouts for all relevant UI elements.
        /// </summary>
        void UpdateLayoutForAllScreens();

        /// <summary>
        /// Triggers a global UI feedback event.
        /// </summary>
        /// <param name="feedbackKey">The key identifying the feedback to play.</param>
        void TriggerUIGlobalFeedback(string feedbackKey);
        
        /// <summary>
        /// Gets the currently active screen type.
        /// </summary>
        /// <returns>The ScreenType of the currently active screen.</returns>
        ScreenType GetCurrentScreenType();
    }
}