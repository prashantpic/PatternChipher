using System.Collections.Generic;
using UnityEngine;
// Assuming PatternCipher.UI.Coordinator.Core.UIEvents is defined
// Assuming PatternCipher.UI.Coordinator.Layout.ResponsiveProfile, .ResponsiveRule,
// .CanvasScalerAdapter, .SafeAreaHandler, .PlatformLayoutAdjuster are defined.

namespace PatternCipher.UI.Coordinator.Layout
{
    /// <summary>
    /// Manages and applies responsive layout rules across UI screens and components,
    /// adapting to device formats, orientations, and safe areas.
    /// </summary>
    public class LayoutManager // Typically implements ILayoutManager
    {
        private ResponsiveProfile _currentProfile;
        private readonly List<IResponsiveUIElement> _responsiveElements;
        private readonly CanvasScalerAdapter _canvasScalerAdapter;
        private readonly SafeAreaHandler _safeAreaHandler; // Assuming static or instance
        private readonly PlatformLayoutAdjuster _platformLayoutAdjuster; // May handle input like back button

        private Rect _currentSafeArea;
        private ScreenOrientation _currentOrientation;
        private Vector2Int _currentScreenSize;


        public LayoutManager(
            ResponsiveProfile defaultProfile,
            CanvasScalerAdapter canvasScalerAdapter,
            SafeAreaHandler safeAreaHandler,
            PlatformLayoutAdjuster platformLayoutAdjuster)
        {
            _currentProfile = defaultProfile;
            _canvasScalerAdapter = canvasScalerAdapter;
            _safeAreaHandler = safeAreaHandler; // Assuming SafeAreaHandler might be a utility class with static methods or an injectable service.
            _platformLayoutAdjuster = platformLayoutAdjuster;
            _responsiveElements = new List<IResponsiveUIElement>();

            // Listen to UIEvents for safe area changes if not handled by direct calls
            // Core.UIEvents.OnSafeAreaInsetsChanged += UpdateSafeAreaInsets;
        }

        /// <summary>
        /// Initializes the LayoutManager with a specific responsive profile.
        /// </summary>
        /// <param name="initialProfile">The initial ResponsiveProfile to use.</param>
        public void Initialize(ResponsiveProfile initialProfile)
        {
            _currentProfile = initialProfile ?? _currentProfile; // Use initial if provided, else keep existing default
            _currentScreenSize = new Vector2Int(Screen.width, Screen.height);
            _currentOrientation = Screen.orientation;
            // Initial application of layout
            // UpdateSafeAreaInsets(Screen.safeArea); // Done by UICoordinatorService typically
        }
        
        /// <summary>
        /// Registers a UI element that should be managed by this LayoutManager.
        /// </summary>
        public void RegisterResponsiveElement(IResponsiveUIElement element)
        {
            if (element != null && !_responsiveElements.Contains(element))
            {
                _responsiveElements.Add(element);
                // Optionally apply current layout immediately
                // RecalculateLayoutForElement(element);
            }
        }

        /// <summary>
        /// Unregisters a UI element, so it's no longer managed by this LayoutManager.
        /// </summary>
        public void UnregisterResponsiveElement(IResponsiveUIElement element)
        {
            if (element != null)
            {
                _responsiveElements.Remove(element);
            }
        }

        /// <summary>
        /// Updates the stored safe area insets and triggers a layout recalculation.
        /// </summary>
        /// <param name="newSafeArea">The new safe area rectangle.</param>
        public void UpdateSafeAreaInsets(Rect newSafeArea)
        {
            if (_currentSafeArea != newSafeArea)
            {
                _currentSafeArea = newSafeArea;
                Core.UIEvents.NotifySafeAreaInsetsChanged(_currentSafeArea); // Notify others
                RecalculateLayouts();
            }
        }
        
        /// <summary>
        /// Call this method when screen size or orientation changes.
        /// Can be hooked to an update loop or Unity's orientation/resize events.
        /// </summary>
        public void CheckAndApplyLayoutChanges()
        {
            bool changed = false;
            if (_currentScreenSize.x != Screen.width || _currentScreenSize.y != Screen.height)
            {
                _currentScreenSize = new Vector2Int(Screen.width, Screen.height);
                changed = true;
            }
            if (_currentOrientation != Screen.orientation)
            {
                _currentOrientation = Screen.orientation;
                changed = true;
            }

            if (changed)
            {
                RecalculateLayouts();
            }
        }

        /// <summary>
        /// Recalculates and applies layout rules to all registered responsive elements.
        /// This is the core logic for adapting the UI.
        /// </summary>
        public void RecalculateLayouts()
        {
            if (_currentProfile == null)
            {
                Debug.LogWarning("LayoutManager: No ResponsiveProfile set. Cannot recalculate layouts.");
                return;
            }

            // Determine the best matching ResponsiveRule from _currentProfile
            // This logic depends on how ResponsiveRule conditions are defined (e.g., aspect ratio, width/height thresholds)
            ResponsiveRule activeRule = _currentProfile.GetBestMatchingRule(Screen.width, Screen.height, Screen.dpi, Screen.orientation);

            if (activeRule == null)
            {
                Debug.LogWarning($"LayoutManager: No matching ResponsiveRule found for current screen configuration (W:{Screen.width}, H:{Screen.height}, O:{Screen.orientation}).");
                // Potentially use a default fallback rule from the profile
                activeRule = _currentProfile.DefaultRule; 
                if(activeRule == null) return;
            }
            
            // Apply CanvasScaler settings from the rule if available
            _canvasScalerAdapter?.ApplyRuleSettings(activeRule);

            // Iterate _responsiveElements and apply the rule
            foreach (var element in _responsiveElements)
            {
                element.UpdateLayout(activeRule, _currentSafeArea);
            }

            // Consider REQ-5-014: tap target sizes and legibility constraints.
            // This might be handled within IResponsiveUIElement implementations based on rule parameters,
            // or LayoutManager could provide helper methods/checks.

            Core.UIEvents.NotifyResponsiveLayoutRecalculated();
            Debug.Log($"LayoutManager: Recalculated layouts with rule: {activeRule?.RuleName ?? "None"} and safe area: {_currentSafeArea}");
        }

        // private void RecalculateLayoutForElement(IResponsiveUIElement element)
        // {
        //     if (_currentProfile == null) return;
        //     ResponsiveRule activeRule = _currentProfile.GetBestMatchingRule(Screen.width, Screen.height, Screen.dpi, Screen.orientation);
        //     if (activeRule == null && _currentProfile.DefaultRule == null) return;
        //     activeRule = activeRule ?? _currentProfile.DefaultRule;
        //     element.UpdateLayout(activeRule, _currentSafeArea);
        // }


        public void SetProfile(ResponsiveProfile newProfile)
        {
            if (newProfile != null)
            {
                _currentProfile = newProfile;
                RecalculateLayouts();
            }
        }
        
        public void Dispose()
        {
            // Core.UIEvents.OnSafeAreaInsetsChanged -= UpdateSafeAreaInsets;
            _responsiveElements.Clear();
        }
    }
}