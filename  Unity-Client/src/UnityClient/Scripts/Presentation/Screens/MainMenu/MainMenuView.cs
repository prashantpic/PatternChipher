using UnityEngine;
using UnityEngine.UI;
using System;

namespace PatternCipher.Client.Presentation.Screens.MainMenu
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        // [SerializeField] private Button howToPlayButton; // Optional

        public event Action OnPlayButtonClick;
        public event Action OnSettingsButtonClick;
        public event Action OnQuitButtonClick;
        // public event Action OnHowToPlayButtonClick; // Optional

        private void Awake()
        {
            if (playButton != null)
            {
                playButton.onClick.AddListener(() => OnPlayButtonClick?.Invoke());
            }
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(() => OnSettingsButtonClick?.Invoke());
            }
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(() => OnQuitButtonClick?.Invoke());
            }
            // if (howToPlayButton != null)
            // {
            //    howToPlayButton.onClick.AddListener(() => OnHowToPlayButtonClick?.Invoke());
            // }
        }

        private void OnDestroy()
        {
            if (playButton != null)
            {
                playButton.onClick.RemoveAllListeners();
            }
            if (settingsButton != null)
            {
                settingsButton.onClick.RemoveAllListeners();
            }
            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
            }
            // if (howToPlayButton != null)
            // {
            //    howToPlayButton.onClick.RemoveAllListeners();
            // }
        }
    }
}