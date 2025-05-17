using UnityEngine.UIElements;
using System; // For Action

namespace PatternCipher.UI.Utils
{
    /// <summary>
    /// Provides convenient extension methods for working with UI Toolkit VisualElements,
    /// aiding in UI construction, manipulation, and querying.
    /// </summary>
    public static class VisualElementUtils
    {
        /// <summary>
        /// Sets the display style of a VisualElement.
        /// </summary>
        /// <param name="element">The visual element.</param>
        /// <param name="isVisible">True to set display to Flex, false to set to None.</param>
        public static void SetVisible(this VisualElement element, bool isVisible)
        {
            if (element == null) return;
            element.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /// <summary>
        /// Checks if the VisualElement is currently visible (display style is not None).
        /// </summary>
        /// <param name="element">The visual element.</param>
        /// <returns>True if visible, false otherwise.</returns>
        public static bool IsVisible(this VisualElement element)
        {
            if (element == null) return false;
            return element.style.display != DisplayStyle.None;
        }

        /// <summary>
        /// Toggles the display style of a VisualElement between Flex and None.
        /// </summary>
        /// <param name="element">The visual element.</param>
        public static void ToggleVisibility(this VisualElement element)
        {
            if (element == null) return;
            element.SetVisible(!element.IsVisible());
        }

        /// <summary>
        /// Safely adds a USS class to the element's class list if it's not already present.
        /// </summary>
        /// <param name="element">The visual element.</param>
        /// <param name="className">The USS class name to add.</param>
        public static void AddSafeClass(this VisualElement element, string className)
        {
            if (element == null || string.IsNullOrEmpty(className)) return;
            if (!element.ClassListContains(className))
            {
                element.AddToClassList(className);
            }
        }

        /// <summary>
        /// Safely removes a USS class from the element's class list if it's present.
        /// </summary>
        /// <param name="element">The visual element.</param>
        /// <param name="className">The USS class name to remove.</param>
        public static void RemoveSafeClass(this VisualElement element, string className)
        {
            if (element == null || string.IsNullOrEmpty(className)) return;
            if (element.ClassListContains(className))
            {
                element.RemoveFromClassList(className);
            }
        }

        /// <summary>
        /// Toggles a USS class on the element. Adds it if not present, removes it if present.
        /// </summary>
        /// <param name="element">The visual element.</param>
        /// <param name="className">The USS class name to toggle.</param>
        public static void ToggleClass(this VisualElement element, string className)
        {
            if (element == null || string.IsNullOrEmpty(className)) return;
            if (element.ClassListContains(className))
            {
                element.RemoveFromClassList(className);
            }
            else
            {
                element.AddToClassList(className);
            }
        }
        
        /// <summary>
        /// Awaits the next geometry change event for a VisualElement.
        /// Useful for operations that need to run after layout calculations.
        /// </summary>
        /// <param name="element">The visual element.</param>
        /// <param name="onLayoutReady">Action to invoke once the geometry has changed (layout is ready).</param>
        public static void OnNextLayoutReady(this VisualElement element, Action onLayoutReady)
        {
            if (element == null || onLayoutReady == null) return;

            void GeometryChangedHandler(GeometryChangedEvent evt)
            {
                element.UnregisterCallback<GeometryChangedEvent>(GeometryChangedHandler);
                onLayoutReady.Invoke();
            }
            element.RegisterCallback<GeometryChangedEvent>(GeometryChangedHandler);
        }
    }
}