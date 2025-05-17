using PatternCipher.Client.Domain.Entities; // For Tile, Symbol (domain entities)
using PatternCipher.Client.Domain.ValueObjects; // For GridPosition
using UnityEngine;
using UnityEngine.UI; // If using UI.Image
using DG.Tweening; // For DOTween animations

namespace PatternCipher.Client.Presentation.Views
{
    public class TileView : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer symbolSpriteRenderer; // Use if rendering with SpriteRenderers

        [SerializeField]
        private Image symbolImage; // Use if rendering with UI Images on a Canvas

        [SerializeField]
        private SpriteRenderer backgroundSpriteRenderer;

        [SerializeField]
        private Image backgroundImage;
        
        [SerializeField]
        private GameObject selectionVisual; // e.g., a highlight border

        [SerializeField]
        private GameObject lockVisual; // e.g., a lock icon

        public GridPosition GridPosition { get; private set; }

        private const float DEFAULT_ANIMATION_DURATION = 0.3f;

        public void Setup(GridPosition position)
        {
            GridPosition = position;
            transform.localPosition = new Vector3(position.Column, position.Row, 0); // Example positioning
            if (selectionVisual != null) selectionVisual.SetActive(false);
            if (lockVisual != null) lockVisual.SetActive(false);
        }

        /// <summary>
        /// Updates the visual appearance of the tile based on its domain data.
        /// </summary>
        /// <param name="tileData">The domain Tile entity data.</param>
        /// <param name="symbolSprite">The sprite for the tile's symbol.</param>
        public void UpdateAppearance(Tile tileData, Sprite symbolSprite)
        {
            if (tileData == null)
            {
                Debug.LogError($"TileView at {GridPosition} received null tileData.");
                // Optionally hide or show an empty state
                if (symbolSpriteRenderer != null) symbolSpriteRenderer.sprite = null;
                if (symbolImage != null) symbolImage.sprite = null;
                return;
            }

            this.GridPosition = tileData.Position; // Ensure GridPosition is up-to-date

            if (symbolSpriteRenderer != null)
            {
                symbolSpriteRenderer.sprite = symbolSprite;
            }
            if (symbolImage != null)
            {
                symbolImage.sprite = symbolSprite;
                symbolImage.enabled = (symbolSprite != null);
            }

            UpdateStateVisuals(tileData.State);
        }

        private void UpdateStateVisuals(TileState state)
        {
            if (selectionVisual != null)
            {
                selectionVisual.SetActive(state == TileState.Selected);
            }

            if (lockVisual != null)
            {
                lockVisual.SetActive(state == TileState.Locked);
            }

            // Handle other states like Matched, Clearing, etc.
            // For example, if Matched, you might start a fade-out animation.
            if (state == TileState.Matched)
            {
                AnimateMatch(DEFAULT_ANIMATION_DURATION);
            }
        }

        public void AnimateMove(Vector3 targetPosition, float duration, Ease easeType = Ease.OutQuad)
        {
            transform.DOLocalMove(targetPosition, duration).SetEase(easeType);
        }

        public void AnimateSelection(bool selected)
        {
            if (selectionVisual != null)
            {
                selectionVisual.SetActive(selected); // Simple toggle, or animate scale/fade
                if (selected)
                {
                    selectionVisual.transform.localScale = Vector3.one * 0.8f;
                    selectionVisual.transform.DOScale(Vector3.one, DEFAULT_ANIMATION_DURATION).SetEase(Ease.OutBack);
                }
            }
        }

        public void AnimateMatch(float duration)
        {
            // Example: Scale down and fade out
            Sequence matchSequence = DOTween.Sequence();
            if (symbolImage != null) matchSequence.Join(symbolImage.DOFade(0, duration));
            if (symbolSpriteRenderer != null) matchSequence.Join(symbolSpriteRenderer.DOFade(0, duration));
            matchSequence.Join(transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));
            matchSequence.OnComplete(() => {
                 // gameObject.SetActive(false); // Or return to pool
                 OnUnpooled(); // If part of an object pool
            });
        }

        public void AnimateLock(bool locked)
        {
             if (lockVisual != null)
            {
                lockVisual.SetActive(locked);
                if (locked)
                {
                    lockVisual.transform.localScale = Vector3.zero;
                    lockVisual.transform.DOScale(Vector3.one, DEFAULT_ANIMATION_DURATION).SetEase(Ease.OutBounce);
                }
            }
        }
        
        public void AnimateSpawn(float duration, Ease easeType = Ease.OutBack)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, duration).SetEase(easeType);

            if (symbolImage != null)
            {
                Color originalColor = symbolImage.color;
                symbolImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
                symbolImage.DOFade(originalColor.a, duration);
            }
            if (symbolSpriteRenderer != null)
            {
                Color originalColor = symbolSpriteRenderer.color;
                symbolSpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
                symbolSpriteRenderer.DOFade(originalColor.a, duration);
            }
        }


        // Object Pool Participant Methods
        public void OnPooled()
        {
            // Reset state when retrieved from pool
            gameObject.SetActive(true);
            transform.localScale = Vector3.one;
            if (symbolImage != null) symbolImage.color = Color.white; // Reset fade
            if (symbolSpriteRenderer != null) symbolSpriteRenderer.color = Color.white; // Reset fade
            if (selectionVisual != null) selectionVisual.SetActive(false);
            if (lockVisual != null) lockVisual.SetActive(false);
        }

        public void OnUnpooled()
        {
            // Prepare for returning to pool
            // DOTween.Kill(transform); // Kill any active tweens on this object
            gameObject.SetActive(false);
        }
    }
}