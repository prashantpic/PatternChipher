using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using PatternCipher.Presentation.GameplayViews;
using PatternCipher.Presentation.Managers;
using PatternCipher.Application; // Placeholder
using PatternCipher.Domain; // Placeholder

// Placeholder classes
namespace PatternCipher.Presentation.Screens.Game { public class GoalDisplayView : MonoBehaviour { public void SetGoal(object goal) {} } }
namespace PatternCipher.Presentation.Screens.Pause { public class PauseScreen : BaseScreen { } }

namespace PatternCipher.Presentation.Screens.Game
{
    /// <summary>
    /// Manages the display of all in-game UI elements (HUD), providing the player with
    /// real-time information about the current puzzle state.
    /// </summary>
    /// <remarks>
    /// This is a "dumb" view that primarily listens to events from the GameManager
    /// and updates its UI elements accordingly. It holds references to the HUD components
    /// and the main GridView.
    /// </remarks>
    public class GameScreenView : BaseScreen
    {
        [Header("Gameplay Views")]
        [SerializeField] private GridView gridView;
        [SerializeField] private GoalDisplayView goalDisplay;

        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI moveCounterText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Button pauseButton;

        private void Awake()
        {
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }
        
        protected override void OnShow(object data)
        {
            // When the game screen is shown, it expects level data
            if (data is LevelData levelData)
            {
                SetupLevel(levelData);
            }
            
            // Subscribe to game state updates
            GameManager.Instance.OnMoveCountChanged += UpdateMoveCounter;
            GameManager.Instance.OnTimerUpdated += UpdateTimer;
        }

        protected override void OnHide()
        {
            // Unsubscribe to prevent memory leaks
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnMoveCountChanged -= UpdateMoveCounter;
                GameManager.Instance.OnTimerUpdated -= UpdateTimer;
            }
        }

        private void OnDestroy()
        {
            pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
        }

        /// <summary>
        /// Initializes the game screen for a new level.
        /// </summary>
        /// <param name="levelData">The data for the level being started.</param>
        public void SetupLevel(LevelData levelData)
        {
            gridView.CreateGrid(levelData.Grid);
            goalDisplay.SetGoal(levelData.Goal);
            UpdateMoveCounter(0, levelData.ParMoves);
            UpdateTimer(0);
        }

        /// <summary>
        /// Updates the move counter text display.
        /// </summary>
        public void UpdateMoveCounter(int currentMoves, int parMoves)
        {
            moveCounterText.text = $"Moves: {currentMoves} / {parMoves}";
        }

        /// <summary>
        /// Updates the timer display.
        /// </summary>
        public void UpdateTimer(float time)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            timerText.text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        
        /// <summary>
        /// Handles the pause button click event.
        /// </summary>
        private void OnPauseButtonClicked()
        {
            UIManager.Instance.ShowScreen<Pause.PauseScreen>();
        }
    }
}