using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PatternCipher.Presentation.Managers;
using PatternCipher.Presentation.Accessibility;
using PatternCipher.Application; // Placeholder

namespace PatternCipher.Presentation.Screens.Settings
{
    /// <summary>
    /// Manages the user interface for all player-configurable game settings.
    /// It reads current settings to populate the view and saves changes made by the user.
    /// </summary>
    public class SettingsScreen : BaseScreen
    {
        [Header("UI Controls")]
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TMP_Dropdown colorblindModeDropdown;
        [SerializeField] private Toggle reducedMotionToggle;
        [SerializeField] private Button resetProgressButton; // As per SDS file structure

        protected override void OnShow(object data)
        {
            base.OnShow(data);
            LoadCurrentSettings();
            AddListeners();
        }
        
        protected override void OnHide()
        {
            base.OnHide();
            RemoveListeners();
        }

        /// <summary>
        /// Populates UI controls with the current values from their respective managers.
        /// </summary>
        private void LoadCurrentSettings()
        {
            // Assuming AudioManager and AccessibilityManager exist and are singletons
            if (AudioManager.Instance != null)
            {
                musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
                sfxVolumeSlider.value = AudioManager.Instance.GetSfxVolume();
            }

            if (AccessibilityManager.Instance != null)
            {
                // Populate dropdown options
                colorblindModeDropdown.ClearOptions();
                foreach (string mode in System.Enum.GetNames(typeof(ColorblindMode)))
                {
                    colorblindModeDropdown.options.Add(new TMP_Dropdown.OptionData(mode));
                }
                colorblindModeDropdown.value = (int)AccessibilityManager.Instance.CurrentColorblindMode;
                colorblindModeDropdown.RefreshShownValue();

                reducedMotionToggle.isOn = AccessibilityManager.Instance.IsReducedMotionEnabled;
            }
        }

        private void AddListeners()
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            colorblindModeDropdown.onValueChanged.AddListener(OnColorblindModeChanged);
            reducedMotionToggle.onValueChanged.AddListener(OnReducedMotionChanged);
            if(resetProgressButton != null) resetProgressButton.onClick.AddListener(OnResetProgressClicked);
        }

        private void RemoveListeners()
        {
            musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
            colorblindModeDropdown.onValueChanged.RemoveListener(OnColorblindModeChanged);
            reducedMotionToggle.onValueChanged.RemoveListener(OnReducedMotionChanged);
            if(resetProgressButton != null) resetProgressButton.onClick.RemoveListener(OnResetProgressClicked);
        }

        private void OnMusicVolumeChanged(float value)
        {
            AudioManager.Instance?.SetMusicVolume(value);
        }

        private void OnSfxVolumeChanged(float value)
        {
            AudioManager.Instance?.SetSfxVolume(value);
        }

        private void OnColorblindModeChanged(int index)
        {
            AccessibilityManager.Instance?.SetColorblindMode((ColorblindMode)index);
        }

        private void OnReducedMotionChanged(bool value)
        {
            AccessibilityManager.Instance?.SetReducedMotion(value);
        }
        
        private void OnResetProgressClicked()
        {
            // This would typically show a confirmation dialog first
            // For now, it directly calls the application layer
            ProgressionManager.Instance?.ResetAllProgress();
            
            // Optionally, give feedback to the user
            Debug.Log("Player progress has been reset.");
        }
    }
}