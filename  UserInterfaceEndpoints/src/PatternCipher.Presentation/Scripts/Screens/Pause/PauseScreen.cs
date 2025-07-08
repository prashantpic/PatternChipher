using UnityEngine;
using UnityEngine.UI;
using PatternCipher.Presentation.Managers;
using PatternCipher.Application; // Placeholder

namespace PatternCipher.Presentation.Screens.Pause
{
    /// <summary>
    /// Manages the UI and logic for the in-game pause menu, providing navigation
    /// and game state control options to the player.
    /// </summary>
    public class PauseScreen : BaseScreen
    {
        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mainMenuButton;

        private void Awake()
        {
            resumeButton.onClick.AddListener(OnResumeButtonClicked);
            restartButton.onClick.AddListener(OnRestartButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }

        private void OnDestroy()
        {
            resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
            restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
            mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
        }

        /// <summary>
        /// Pauses the game by setting time scale to 0 when the screen is shown.
        /// </summary>
        protected override void OnShow(object data)
        {
            base.OnShow(data);
            Time.timeScale = 0f;
        }

        /// <summary>
        /// Resumes the game by setting time scale back to 1 when the screen is hidden.
        /// </summary>
        protected override void OnHide()
        {
            base.OnHide();
            Time.timeScale = 1f;
        }

        private void OnResumeButtonClicked()
        {
            UIManager.Instance.HideCurrentScreen();
        }

        private void OnRestartButtonClicked()
        {
            // Hide the pause menu *before* restarting to ensure OnHide() restores time scale
            UIManager.Instance.HideCurrentScreen();
            GameManager.Instance.RestartLevel();
        }

        private void OnSettingsButtonClicked()
        {
            // Show the settings screen as a layer on top of the pause menu
            UIManager.Instance.ShowScreen<Settings.SettingsScreen>();
        }

        private void OnMainMenuButtonClicked()
        {
            // Time scale must be restored before changing scenes/states
            Time.timeScale = 1f;
            GameManager.Instance.GoToMainMenu();
        }
    }
}