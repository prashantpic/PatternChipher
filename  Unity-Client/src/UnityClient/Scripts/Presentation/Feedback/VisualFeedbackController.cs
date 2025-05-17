using UnityEngine;
using DG.Tweening;
using PatternCipher.Client.Core.Events;
using PatternCipher.Client.Domain.Events;
using PatternCipher.Client.Presentation.Feedback.Configuration;
using PatternCipher.Client.Presentation.Views; // For TileView if needed
using PatternCipher.Client.Domain.ValueObjects; // For GridPosition

namespace PatternCipher.Client.Presentation.Feedback
{
    public class VisualFeedbackController : MonoBehaviour
    {
        [SerializeField] private FeedbackConfigSO feedbackConfig;
        [SerializeField] private Camera mainCamera; // For screen shake

        // Example particle prefabs - can be part of FeedbackConfigSO or direct references
        [SerializeField] private GameObject tileMatchParticlePrefab;
        [SerializeField] private GameObject tileTapSuccessParticlePrefab;

        // Assuming GridView reference is obtained elsewhere or passed in
        private GridView _gridView;

        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void OnEnable()
        {
            GlobalEventBus.Instance?.Subscribe<TileInteractionFeedbackEvent>(HandleTileInteractionFeedback);
            GlobalEventBus.Instance?.Subscribe<LevelCompletedEvent>(HandleLevelCompletedFeedback);
        }

        private void OnDisable()
        {
            GlobalEventBus.Instance?.Unsubscribe<TileInteractionFeedbackEvent>(HandleTileInteractionFeedback);
            GlobalEventBus.Instance?.Unsubscribe<LevelCompletedEvent>(HandleLevelCompletedFeedback);
        }

        public void Initialize(GridView gridView)
        {
            _gridView = gridView;
        }

        private void HandleTileInteractionFeedback(TileInteractionFeedbackEvent eventData)
        {
            if (feedbackConfig == null) return;

            TileView tileView = _gridView?.GetTileViewAt(eventData.Position);

            switch (eventData.RequestedFeedback)
            {
                case FeedbackType.TapSuccess:
                    if (tileView != null)
                    {
                        tileView.transform.DOPunchScale(feedbackConfig.TapSuccessPunch, feedbackConfig.TapAnimationDuration, 10, 1)
                            .SetEase(feedbackConfig.TapEase);
                    }
                    SpawnParticleEffect(eventData.Position, tileTapSuccessParticlePrefab, feedbackConfig.DefaultParticleDuration);
                    break;
                case FeedbackType.TapInvalid:
                     if (tileView != null)
                    {
                        // Example: Shake or color flash
                        tileView.transform.DOShakePosition(feedbackConfig.InvalidMoveShakeDuration, feedbackConfig.InvalidMoveShakeStrength, 10, 90)
                            .SetEase(feedbackConfig.InvalidMoveEase);
                    }
                    break;
                case FeedbackType.SwapSuccess:
                    // Could animate two tiles
                    // For now, let's assume individual tile animations are handled by TileView after move
                    break;
                case FeedbackType.SwapInvalid:
                    TileView tileView2 = _gridView?.GetTileViewAt(eventData.Position2); // Assuming Position2 exists in event for swap
                    if (tileView != null) tileView.transform.DOShakePosition(feedbackConfig.InvalidMoveShakeDuration, feedbackConfig.InvalidMoveShakeStrength).SetEase(feedbackConfig.InvalidMoveEase);
                    if (tileView2 != null) tileView2.transform.DOShakePosition(feedbackConfig.InvalidMoveShakeDuration, feedbackConfig.InvalidMoveShakeStrength).SetEase(feedbackConfig.InvalidMoveEase);
                    break;
                case FeedbackType.MatchFound:
                    if (tileView != null)
                    {
                        tileView.AnimateMatch(feedbackConfig.MatchAnimationDuration); // TileView should handle its own match animation
                    }
                    SpawnParticleEffect(eventData.Position, tileMatchParticlePrefab, feedbackConfig.DefaultParticleDuration);
                    // Potentially screen shake for bigger matches
                    if (eventData.MatchSize >= feedbackConfig.ScreenShakeMinMatchSize) // Assuming MatchSize in event
                    {
                        TriggerScreenShake(feedbackConfig.ScreenShakeDuration, feedbackConfig.ScreenShakeStrength);
                    }
                    break;
                case FeedbackType.TileCleared:
                     if (tileView != null)
                    {
                        tileView.transform.DOScale(Vector3.zero, feedbackConfig.ClearAnimationDuration)
                            .SetEase(feedbackConfig.ClearEase).OnComplete(() => {
                                // Potentially disable/pool tileView here or let GridView handle it
                            });
                    }
                    break;
                // Add more cases for other FeedbackType enums
            }
        }

        private void HandleLevelCompletedFeedback(LevelCompletedEvent eventData)
        {
            if (feedbackConfig == null) return;

            // Example: Play a celebratory animation or particle effect
            Debug.Log($"Level Completed Visual Feedback: Stars {eventData.StarsAwarded}");
            if (eventData.StarsAwarded >= feedbackConfig.LevelCompleteMinStarsForCelebration)
            {
                // Instantiate a full-screen particle effect or trigger a UI animation
                if (feedbackConfig.LevelCompleteCelebrationEffect != null)
                {
                    Instantiate(feedbackConfig.LevelCompleteCelebrationEffect, Vector3.zero, Quaternion.identity);
                }
                TriggerScreenShake(feedbackConfig.LevelCompleteScreenShakeDuration, feedbackConfig.LevelCompleteScreenShakeStrength);
            }
        }


        private void SpawnParticleEffect(GridPosition gridPos, GameObject particlePrefab, float duration)
        {
            if (particlePrefab == null || _gridView == null) return;

            Vector3 worldPos = _gridView.GetWorldPositionFromGrid(gridPos);
            GameObject particleInstance = Instantiate(particlePrefab, worldPos, Quaternion.identity);
            Destroy(particleInstance, duration); // Simple destruction, consider pooling
        }

        private void TriggerScreenShake(float duration, float strength)
        {
            if (mainCamera == null || feedbackConfig == null) return;
            mainCamera.DOShakePosition(duration, strength, feedbackConfig.ScreenShakeVibrato, feedbackConfig.ScreenShakeRandomness)
                .SetEase(feedbackConfig.ScreenShakeEase);
        }
    }
}