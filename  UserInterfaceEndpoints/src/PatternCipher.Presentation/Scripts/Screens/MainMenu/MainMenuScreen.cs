using UnityEngine;
using UnityEngine.UI;
using PatternCipher.Presentation.Managers;

// Placeholder references to other screen classes
namespace PatternCipher.Presentation.Screens.LevelSelect { public class LevelSelectScreen : BaseScreen {} }
namespace PatternCipher.Presentation.Screens.Settings { public class SettingsScreen : BaseScreen {} }
namespace PatternCipher.Presentation.Screens.HowToPlay { public class HowToPlayScreen : BaseScreen {} }

namespace PatternCipher.Presentation.Screens.MainMenu
{
    /// <summary>
    /// The controller for the game's main entry point screen.
    /// It handles user interactions on the main menu, triggering navigation
    /// to other parts of the application like level selection or settings.
    /// </summary>
    public class MainMenuScreen : BaseScreen
    {
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button howToPlayButton;
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            howToPlayButton.onClick.AddListener(OnHowToPlayButtonClicked);
            exitButton.onClick.AddListener(OnExitButtonClicked);

            // Conditionally show the exit button based on the platform
#if UNITY_ANDROID || UNITY_IOS
            exitButton.gameObject.SetActive(false);
#else
            exitButton.gameObject.SetActive(true);
#endif
        }

        private void OnDestroy()
        {
            playButton.onClick.RemoveListener(OnPlayButtonClicked);
            settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
            howToPlayButton.onClick.RemoveListener(OnHowToPlayButtonClicked);
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }

        /// <summary>
        /// Handles the click event for the 'Play' button.
        /// </summary>
        private void OnPlayButtonClicked()
        {
            UIManager.Instance.ShowScreen<LevelSelect.LevelSelectScreen>();
        }

        /// <summary>
        /// Handles the click event for the 'Settings' button.
        /// </summary>
        private void OnSettingsButtonClicked()
        {
            UIManager.Instance.ShowScreen<Settings.SettingsScreen>();
        }

        /// <summary>
        /// Handles the click event for the 'How to Play' button.
        /// </summary>
        private void OnHowToPlayButtonClicked()
        {
            UIManager.Instance.ShowScreen<HowToPlay.HowToPlayScreen>();
        }

        /// <summary>
        /// Handles the click event for the 'Exit' button.
        /// </summary>
        private void OnExitButtonClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            Application.Quit();
#endif
        }
    }
}