using System;
using System.Threading.Tasks;
// Assuming PatternCipher.UI.Coordinator.Interfaces.IUIView is defined elsewhere
// Assuming PatternCipher.UI.Coordinator.Navigation.ScreenType is defined elsewhere (e.g., as an enum)

namespace PatternCipher.UI.Coordinator.Navigation
{
    /// <summary>
    /// Interface for screen navigation logic, defining how screens are shown, hidden,
    /// and how navigation history is managed.
    /// </summary>
    public interface IScreenNavigator
    {
        /// <summary>
        /// Shows the specified screen, potentially hiding the current one and managing transitions.
        /// </summary>
        /// <param name="screenType">The type of screen to show.</param>
        /// <param name="payload">Optional data to pass to the screen upon navigation.</param>
        /// <returns>A task that completes when the screen is shown and transition (if any) is complete.</returns>
        Task ShowScreenAsync(ScreenType screenType, NavigationPayload payload);

        /// <summary>
        /// Navigates to the previous screen in the history, if available.
        /// </summary>
        /// <returns>A task that completes when the navigation is complete.</returns>
        Task GoBackAsync();

        /// <summary>
        /// Gets the currently active screen view instance.
        /// </summary>
        /// <returns>The current IUIView instance, or null if no screen is active.</returns>
        PatternCipher.UI.Coordinator.Interfaces.IUIView GetCurrentScreen();

        /// <summary>
        /// Gets the type of the currently active screen.
        /// </summary>
        /// <returns>The ScreenType of the current screen. May return a default/none value if no screen is active.</returns>
        ScreenType GetCurrentScreenType();

        /// <summary>
        /// Registers a factory method for creating instances of a specific screen type.
        /// This allows for custom instantiation logic, potentially outside the Addressables system.
        /// </summary>
        /// <param name="screenType">The screen type to register the factory for.</param>
        /// <param name="factoryMethod">A function that asynchronously creates and returns an IUIView instance.</param>
        void RegisterScreenViewFactory(ScreenType screenType, Func<Task<PatternCipher.UI.Coordinator.Interfaces.IUIView>> factoryMethod);
    }
}