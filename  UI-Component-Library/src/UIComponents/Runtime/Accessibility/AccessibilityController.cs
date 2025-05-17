using UnityEngine;
using System;
using System.Collections.Generic;

namespace PatternCipher.UI.Accessibility
{
    // Depends on:
    // - IAccessibilitySettingsProvider interface
    // - AccessibilityProfile ScriptableObject definition
    // - ColorPaletteDefinition ScriptableObject definition
    // - ColorVisionDeficiencyMode enum
    // - ThemeManager service (PatternCipher.UI.Services.ThemeManager)
    // - IColorStylable, ITextStylable, IMotionControllable interfaces (for subscribers)

    public class AccessibilityController : MonoBehaviour
    {
        private IAccessibilitySettingsProvider _settingsProvider;
        private Services.ThemeManager _themeManager; // Assuming ThemeManager is a plain C# class
        private AccessibilityProfile _currentProfile;

        public event Action<ColorPaletteDefinition> OnColorPaletteChanged;
        public event Action<float, ColorPaletteDefinition> OnTextStylesChanged; // textSizeMultiplier, activePalette
        public event Action<bool> OnReducedMotionChanged;
        public event Action<bool> OnHapticsChanged;
        
        // For direct subscription if event-based is not sufficient or for specific cases
        private readonly List<IColorStylable> _colorStylables = new List<IColorStylable>();
        private readonly List<ITextStylable> _textStylables = new List<ITextStylable>();
        private readonly List<IMotionControllable> _motionControllables = new List<IMotionControllable>();


        public void Initialize(IAccessibilitySettingsProvider settingsProvider, Services.ThemeManager themeManager)
        {
            _settingsProvider = settingsProvider ?? throw new ArgumentNullException(nameof(settingsProvider));
            _themeManager = themeManager ?? throw new ArgumentNullException(nameof(themeManager));

            if (_settingsProvider.CurrentAccessibilityProfile != null)
            {
                ApplyAccessibilityProfile(_settingsProvider.CurrentAccessibilityProfile);
            }
            // TODO: Subscribe to _settingsProvider.OnSettingsChanged if it exists to auto-update profile
        }

        public void ApplyAccessibilityProfile(AccessibilityProfile profile)
        {
            if (profile == null)
            {
                Debug.LogError("[AccessibilityController] Cannot apply a null profile.");
                return;
            }
            _currentProfile = profile;
            Debug.Log($"[AccessibilityController] Applying profile: {_currentProfile.ProfileName}");

            ColorPaletteDefinition activePalette = DetermineActivePalette();
            
            OnColorPaletteChanged?.Invoke(activePalette);
            foreach (var stylable in _colorStylables) { stylable.ApplyColorPalette(activePalette); }

            OnTextStylesChanged?.Invoke(_currentProfile.TextSizeMultiplier, activePalette);
            foreach (var stylable in _textStylables) { stylable.ApplyTextStyles(_currentProfile.TextSizeMultiplier, activePalette); }
            
            OnReducedMotionChanged?.Invoke(_currentProfile.EnableReducedMotion);
            foreach (var controllable in _motionControllables) { controllable.SetReducedMotion(_currentProfile.EnableReducedMotion); }

            OnHapticsChanged?.Invoke(_currentProfile.EnableHaptics);

            // Apply theme using ThemeManager (e.g., for root UI or specific USS variables)
            // This part needs careful design: ThemeManager might apply a base USS and then specific overrides.
            // For now, let's assume ThemeManager can apply a palette to a root VisualElement if needed.
            // Example: _themeManager.ApplyTheme(rootVisualElement, activePalette);
            // This requires a reference to the root VisualElement of the UI, which AccessibilityController might not own.
            // This might be better handled by having UI root components subscribe to OnColorPaletteChanged
            // and call ThemeManager themselves, or ThemeManager subscribes.
            // For simplicity, we'll assume ThemeManager is primarily used by components that ARE IColorStylable or via events.
        }

        private ColorPaletteDefinition DetermineActivePalette()
        {
            if (_currentProfile == null) return null;

            if (_currentProfile.ColorVisionMode != Enums.ColorVisionDeficiencyMode.Normal && 
                _currentProfile.ColorBlindPaletteOverride != null)
            {
                return _currentProfile.ColorBlindPaletteOverride;
            }
            return _currentProfile.BaseColorPalette;
        }

        public void SetColorVisionDeficiencyMode(Enums.ColorVisionDeficiencyMode mode)
        {
            if (_currentProfile == null) return;
            if (_currentProfile.ColorVisionMode == mode) return;

            _currentProfile.ColorVisionMode = mode; // Modifying SO instance; consider if this is desired or if we should work with a copy
            NotifyProfileUpdated();
        }

        public void SetReducedMotion(bool enabled)
        {
            if (_currentProfile == null) return;
            if (_currentProfile.EnableReducedMotion == enabled) return;

            _currentProfile.EnableReducedMotion = enabled;
            OnReducedMotionChanged?.Invoke(enabled);
            foreach (var controllable in _motionControllables) { controllable.SetReducedMotion(enabled); }
        }

        public void SetHapticsEnabled(bool enabled)
        {
            if (_currentProfile == null) return;
            if (_currentProfile.EnableHaptics == enabled) return;
            
            _currentProfile.EnableHaptics = enabled;
            OnHapticsChanged?.Invoke(enabled);
        }

        public void SetTextSizeMultiplier(float multiplier)
        {
            if (_currentProfile == null) return;
            multiplier = Mathf.Clamp(multiplier, 0.5f, 3.0f); // As per editor constraint
            if (Mathf.Approximately(_currentProfile.TextSizeMultiplier, multiplier)) return;

            _currentProfile.TextSizeMultiplier = multiplier;
            ColorPaletteDefinition activePalette = DetermineActivePalette();
            OnTextStylesChanged?.Invoke(multiplier, activePalette);
            foreach (var stylable in _textStylables) { stylable.ApplyTextStyles(multiplier, activePalette); }
        }
        
        private void NotifyProfileUpdated()
        {
            if (_currentProfile == null) return;
            // Re-broadcast all relevant changes based on the profile.
            // This is simpler than tracking individual sub-property changes.
            ApplyAccessibilityProfile(_currentProfile);
        }

        public ColorPaletteDefinition GetCurrentColorPalette()
        {
            return DetermineActivePalette();
        }

        public AccessibilityProfile GetCurrentProfile()
        {
            return _currentProfile;
        }

        // Subscriber management methods (optional, could be handled by direct event subscription)
        public void RegisterColorStylable(IColorStylable stylable) { if (!_colorStylables.Contains(stylable)) _colorStylables.Add(stylable); }
        public void UnregisterColorStylable(IColorStylable stylable) { _colorStylables.Remove(stylable); }
        public void RegisterTextStylable(ITextStylable stylable) { if (!_textStylables.Contains(stylable)) _textStylables.Add(stylable); }
        public void UnregisterTextStylable(ITextStylable stylable) { _textStylables.Remove(stylable); }
        public void RegisterMotionControllable(IMotionControllable controllable) { if (!_motionControllables.Contains(controllable)) _motionControllables.Add(controllable); }
        public void UnregisterMotionControllable(IMotionControllable controllable) { _motionControllables.Remove(controllable); }
    }
}