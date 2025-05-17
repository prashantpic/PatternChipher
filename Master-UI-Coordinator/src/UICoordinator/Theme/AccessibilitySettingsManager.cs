using System.Threading.Tasks;
using UnityEngine; // For Debug.Log, etc.
// Assuming IPersistenceService exists in PatternCipher.Utilities namespace
// using PatternCipher.Utilities;

namespace PatternCipher.UI.Coordinator.Theme
{
    public class AccessibilitySettingsManager
    {
        private readonly PatternCipher.Utilities.IPersistenceService _persistenceService;
        private const string SettingsKey = "AccessibilitySettings";

        public AccessibilitySettingsManager(PatternCipher.Utilities.IPersistenceService persistenceService)
        {
            _persistenceService = persistenceService;
            if (_persistenceService == null)
            {
                Debug.LogError("AccessibilitySettingsManager: IPersistenceService dependency is null.");
            }
        }

        public async Task<AccessibilitySettings> LoadSettingsAsync()
        {
            if (_persistenceService == null)
            {
                Debug.LogWarning("AccessibilitySettingsManager: Persistence service not available. Returning default settings.");
                return CreateDefaultSettings();
            }

            var loadedSettings = await _persistenceService.LoadAsync<AccessibilitySettingsData>(SettingsKey);
            if (loadedSettings != null)
            {
                // Convert from a simple data class to ScriptableObject instance if needed,
                // or directly use a serializable class that's not a ScriptableObject.
                // For simplicity, let's assume AccessibilitySettings can be directly (de)serialized
                // or we create a new SO and copy values.
                // If AccessibilitySettings MUST be a ScriptableObject loaded from assets, this model changes.
                // The SDS says "ScriptableObject/Data class". For persistence, a data class is easier.
                // Let's assume we persist a data class and map to a ScriptableObject instance.
                
                AccessibilitySettings settingsSO = ScriptableObject.CreateInstance<AccessibilitySettings>();
                settingsSO.ColorblindMode = loadedSettings.ColorblindMode;
                settingsSO.HighContrastEnabled = loadedSettings.HighContrastEnabled;
                settingsSO.BaseFontSizeMultiplier = loadedSettings.BaseFontSizeMultiplier;
                settingsSO.IsReducedMotionEnabled = loadedSettings.IsReducedMotionEnabled;
                settingsSO.HapticFeedbackEnabled = loadedSettings.HapticFeedbackEnabled;
                return settingsSO;
            }
            return CreateDefaultSettings(); // Return a new ScriptableObject with default values
        }

        public async Task SaveSettingsAsync(AccessibilitySettings settings)
        {
            if (_persistenceService == null)
            {
                Debug.LogWarning("AccessibilitySettingsManager: Persistence service not available. Cannot save settings.");
                return;
            }
            if (settings == null)
            {
                 Debug.LogWarning("AccessibilitySettingsManager: Provided settings are null. Cannot save.");
                return;
            }

            AccessibilitySettingsData dataToSave = new AccessibilitySettingsData
            {
                ColorblindMode = settings.ColorblindMode,
                HighContrastEnabled = settings.HighContrastEnabled,
                BaseFontSizeMultiplier = settings.BaseFontSizeMultiplier,
                IsReducedMotionEnabled = settings.IsReducedMotionEnabled,
                HapticFeedbackEnabled = settings.HapticFeedbackEnabled
            };
            await _persistenceService.SaveAsync(SettingsKey, dataToSave);
        }

        private AccessibilitySettings CreateDefaultSettings()
        {
            AccessibilitySettings defaultSettings = ScriptableObject.CreateInstance<AccessibilitySettings>();
            // Set default values as defined in AccessibilitySettings class
            defaultSettings.ColorblindMode = ColorblindTypeEnum.Normal;
            defaultSettings.HighContrastEnabled = false;
            defaultSettings.BaseFontSizeMultiplier = 1.0f;
            defaultSettings.IsReducedMotionEnabled = false;
            defaultSettings.HapticFeedbackEnabled = true;
            return defaultSettings;
        }

        // Helper data class for persistence, as ScriptableObjects themselves are not ideal for direct JSON persistence.
        [System.Serializable]
        private class AccessibilitySettingsData
        {
            public ColorblindTypeEnum ColorblindMode;
            public bool HighContrastEnabled;
            public float BaseFontSizeMultiplier;
            public bool IsReducedMotionEnabled;
            public bool HapticFeedbackEnabled;
        }
    }
}