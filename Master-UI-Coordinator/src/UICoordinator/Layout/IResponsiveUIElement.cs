using UnityEngine; // For Rect
// Assuming PatternCipher.UI.Coordinator.Layout.ResponsiveRule is defined elsewhere

namespace PatternCipher.UI.Coordinator.Layout
{
    /// <summary>
    /// Interface for UI elements that can be dynamically adjusted by the LayoutManager
    /// in response to screen size, orientation, or safe area changes.
    /// </summary>
    public interface IResponsiveUIElement
    {
        /// <summary>
        /// Gets a unique identifier for this responsive element.
        /// Used for debugging or specific element targeting if needed.
        /// </summary>
        string ElementID { get; }

        /// <summary>
        /// Called by the LayoutManager to instruct the element to update its layout.
        /// </summary>
        /// <param name="appliedRule">The ResponsiveRule determined by the LayoutManager to be active for the current screen configuration.</param>
        /// <param name="safeArea">The current safe area rectangle in screen coordinates.</param>
        void UpdateLayout(ResponsiveRule appliedRule, Rect safeArea);
    }
}