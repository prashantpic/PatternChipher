using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using PatternCipher.Presentation.Managers;
using PatternCipher.Application; // Placeholder for Application Layer
using PatternCipher.Presentation.Feedback; // Placeholder

// Placeholder classes
namespace PatternCipher.Presentation.Screens.LevelComplete
{
    public class StarRatingDisplay : MonoBehaviour 
    {
        public Tween AnimateStars(int count) { return transform.DOScale(1,0.1f); }
    }
    public class LevelCompletionData 
    {
        public int FinalScore;
        public int MovesTaken;
        public float TimeTaken;
        public int StarsEarned;
        public int NextLevelId;
    }
}


namespace PatternCipher.Presentation.Screens.LevelComplete
{
    /// <summary>
    /// The view controller for the screen that appears after a player successfully completes a level.
    /// It shows their stats and provides rewarding, "juicy" feedback.
    /// </summary>
    public class LevelCompleteScreen : BaseScreen
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI movesText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private StarRatingDisplay starRatingDisplay;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button levelSelectButton;

        [Header("Animation Settings")]
        [SerializeField] private float scoreCountUpDuration = 1.5f;
        [SerializeField] private float elementFadeInDuration = 0.5f;
        [SerializeField] private float delayBetweenAnims = 0.5f;

        private LevelCompletionData results;

        private void Awake()
        {
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            replayButton.onClick.AddListener(OnReplayClicked);
            levelSelectButton.onClick.AddListener(OnLevelSelectClicked);
        }
        
        private void OnDestroy()
        {
            nextLevelButton.onClick.RemoveListener(OnNextLevelClicked);
            replayButton.onClick.RemoveListener(OnReplayClicked);
            levelSelectButton.onClick.RemoveListener(OnLevelSelectClicked);
        }

        protected override void OnShow(object data)
        {
            base.OnShow(data);
            
            if (data is LevelCompletionData completionData)
            {
                this.results = completionData;
                InitializeUI();
                PlayVictoryAnimation();
            }
            else
            {
                Debug.LogError("[LevelCompleteScreen] Received invalid data. Expected LevelCompletionData.");
            }
        }

        private void InitializeUI()
        {
            scoreText.text = "Score: 0";
            movesText.text = $"Moves: {results.MovesTaken}";
            timeText.text = $"Time: {results.TimeTaken:F2}s";
            
            // Initially hide elements that will be animated in
            movesText.alpha = 0;
            timeText.alpha = 0;
            nextLevelButton.transform.localScale = Vector3.zero;
            replayButton.transform.localScale = Vector3.zero;
            levelSelectButton.transform.localScale = Vector3.zero;

            nextLevelButton.interactable = results.NextLevelId > 0;
        }

        private void PlayVictoryAnimation()
        {
            var sequence = DOTween.Sequence();

            // 1. Animate Stars
            sequence.Append(starRatingDisplay.AnimateStars(results.StarsEarned));
            sequence.AppendInterval(delayBetweenAnims);
            
            // 2. Animate Score Counting Up
            int currentScore = 0;
            sequence.Append(DOTween.To(() => currentScore, s => currentScore = s, results.FinalScore, scoreCountUpDuration)
                .OnUpdate(() => scoreText.text = $"Score: {currentScore}")
                .SetEase(Ease.OutCubic));
            
            // 3. Fade in other stats
            sequence.Join(movesText.DOFade(1, elementFadeInDuration));
            sequence.Join(timeText.DOFade(1, elementFadeInDuration));
            sequence.AppendInterval(delayBetweenAnims);

            // 4. Play global feedback
            sequence.AppendCallback(() => FeedbackManager.Instance.PlayLevelCompleteFeedback());

            // 5. Pop in the navigation buttons
            sequence.Append(nextLevelButton.transform.DOScale(1, elementFadeInDuration).SetEase(Ease.OutBack));
            sequence.Join(replayButton.transform.DOScale(1, elementFadeInDuration).SetEase(Ease.OutBack));
            sequence.Join(levelSelectButton.transform.DOScale(1, elementFadeInDuration).SetEase(Ease.OutBack));
            
            sequence.Play();
        }

        private void OnNextLevelClicked()
        {
            if (results.NextLevelId > 0)
            {
                GameManager.Instance.StartLevel(results.NextLevelId);
            }
        }

        private void OnReplayClicked()
        {
            GameManager.Instance.RestartLevel();
        }

        private void OnLevelSelectClicked()
        {
            GameManager.Instance.GoToLevelSelect();
        }
    }
}