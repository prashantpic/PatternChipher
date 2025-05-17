using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using PatternCipher.Client.Presentation.Views; // For GridView

namespace PatternCipher.Client.Presentation.Screens.GameScreen
{
    public class GameScreenView : MonoBehaviour
    {
        [Header("Grid Display")]
        [SerializeField] private GridView gridView;

        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI movesText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI goalText;

        [Header("Buttons")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button hintButton;
        // Add other buttons as needed (e.g., undo, special ability)

        public GridView GridView => gridView;

        public event Action OnPauseButtonClick;
        public event Action OnHintButtonClick;

        private void Awake()
        {
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(() => OnPauseButtonClick?.Invoke());
            }
            if (hintButton != null)
            {
                hintButton.onClick.AddListener(() => OnHintButtonClick?.Invoke());
            }
        }

        public void UpdateMovesDisplay(int moves)
        {
            if (movesText != null)
            {
                movesText.text = $"Moves: {moves}";
            }
        }

        public void UpdateTimerDisplay(string timeFormatted)
        {
            if (timerText != null)
            {
                timerText.text = $"Time: {timeFormatted}";
            }
        }

        public void UpdateScoreDisplay(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {score}";
            }
        }

        public void UpdateGoalDisplay(string goalDescription)
        {
            if (goalText != null)
            {
                goalText.text = $"Goal: {goalDescription}";
            }
        }
        
        private void OnDestroy()
        {
            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveAllListeners();
            }
            if (hintButton != null)
            {
                hintButton.onClick.RemoveAllListeners();
            }
        }
    }
}