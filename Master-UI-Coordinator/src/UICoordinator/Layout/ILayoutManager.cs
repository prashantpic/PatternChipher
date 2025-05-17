using UnityEngine;

namespace PatternCipher.UI.Coordinator.Layout
{
    /// <summary>
    /// Interface for the Layout Manager, defining contracts for responsive UI and safe area adaptation.
    /// Defines the contract for managing responsive layouts, safe areas, and adapting UI to various device formats and orientations.
    /// </summary>
    public interface ILayoutManager
    {
        /// <summary>
        /// Initializes the LayoutManager with a default responsive profile.
        /// </summary>
        /// <param name="initialProfile">The initial responsive profile to use.</param>
        void Initialize(ResponsiveProfile initialProfile);

        /// <summary>
        /// Recalculates and applies layout rules to all registered responsive elements.
        /// This should be called when screen size, orientation, or safe area changes.
        /// </summary>
        void RecalculateLayouts();

        /// <summary>
        /// Registers a UI element that needs to be managed for responsive layout updates.
        /// </summary>
        /// <param name="element">The responsive UI element to register.</param>
        void RegisterResponsiveElement(IResponsiveUIElement element);

        /// <summary>
        /// Unregisters a UI element from responsive layout management.
        /// </summary>
        /// <param name="element">The responsive UI element to unregister.</param>
        void UnregisterResponsiveElement(IResponsiveUIElement element);

        /// <summary>
        /// Updates the safe area insets. This typically triggers a layout recalculation.
        /// </summary>
        /// <param name="safeArea">The new safe area rectangle.</param>
        void UpdateSafeAreaInsets(Rect safeArea);
        
        /// <summary>
        /// Gets the current safe area insets.
        /// </summary>
        /// <returns>The current safe area as a Rect.</returns>
        Rect GetSafeAreaInsets();
    }
}