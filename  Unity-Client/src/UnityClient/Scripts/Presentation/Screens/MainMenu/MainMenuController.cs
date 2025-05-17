using UnityEngine;
using PatternCipher.Client.Presentation.Screens.Common; // For BaseScreenController
using PatternCipher.Client.Presentation.Screens; // For ScreenManager

namespace PatternCipher.Client.Presentation.Screens.MainMenu
{
    public class MainMenuController : BaseScreenController
    {
        [SerializeField] private MainMenuView mainMenuView;
        [SerializeField] private ScreenManager screenManager; // Assign in Inspector or find dynamically

        protected override void OnEnable()
        {
            base.OnEnable();
            if (mainMenuView != null)
            {
                mainMenuView.OnPlayButtonClick += HandlePlayButtonClick;
                mainMenuView.OnSettingsButtonClick += HandleSettingsButtonClick;
                mainMenuView.OnQuitButtonClick += HandleQuitButtonClick;
                // Add HowToPlay listener if button exists
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (mainMenuView != null)
            {
                mainMenuView.OnPlayButtonClick -= HandlePlayButtonClick;
                mainMenuView.OnSettingsButtonClick -= HandleSettingsButtonClick;
                mainMenuView.OnQuitButtonClick -= HandleQuitButtonClick;
            }
        }

        private void HandlePlayButtonClick()
        {
            Debug.Log("Play button clicked");
            // Example: Navigate to Level Selection Screen or directly to Game Screen
            // if (screenManager != null)
            // {
            //     screenManager.ShowOnlyScreen<LevelSelectionScreenController>(); // Assuming LevelSelectionScreenController exists
            // }
        }

        private void HandleSettingsButtonClick()
        {
            Debug.Log("Settings button clicked");
            // Example: Navigate to Settings Screen
            // if (screenManager != null)
            // {
            //     screenManager.PushScreen<SettingsScreenController>(); // Assuming SettingsScreenController exists
            // }
        }

        private void HandleQuitButtonClick()
        {
            Debug.Log("Quit button clicked");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}