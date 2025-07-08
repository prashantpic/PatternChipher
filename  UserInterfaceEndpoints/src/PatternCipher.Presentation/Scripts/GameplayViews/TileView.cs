using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using PatternCipher.Domain; // Placeholder for domain models
using PatternCipher.Presentation.Accessibility; // For checking accessibility settings

namespace PatternCipher.Presentation.GameplayViews
{
    /// <summary>
    /// Represents a single interactive tile on the game grid. It updates its visuals
    /// based on model data and communicates user input to its controller.
    /// This is the primary interactive element for the player.
    /// </summary>
    /// <remarks>
    /// Adheres to NFR-US-005 by ensuring its RectTransform provides a large enough tap target.
    /// Implements input interfaces to capture taps and drags.
    /// Uses DOTween for 'juicy' feedback animations (NFR-V-003).
    /// </remarks>
    [RequireComponent(typeof(RectTransform))]
    public class TileView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("UI References")]
        [SerializeField] private Image symbolImage;
        [SerializeField] private Image background;
        [SerializeField] private GameObject lockedOverlay;
        [SerializeField] private GameObject selectionHighlight;

        [Header("Animation Settings")]
        [SerializeField] private float selectionPunchScale = 0.1f;
        [SerializeField] private float selectionAnimDuration = 0.2f;
        [SerializeField] private float errorShakeDuration = 0.5f;
        [SerializeField] private float errorShakeStrength = 15f;

        /// <summary>
        /// The tile's current position in the grid.
        /// </summary>
        public Vector2Int GridPosition { get; set; }

        // --- Static Events for Input Handling ---
        /// <summary>
        /// Fired when a tile is tapped down on.
        /// </summary>
        public static event Action<TileView> OnTilePointerDown;
        /// <summary>
        /// Fired when a pointer is released over a tile.
        /// </summary>
        public static event Action<TileView> OnTilePointerUp;
         /// <summary>
        /// Fired when a drag is detected from one tile to another.
        /// </summary>
        public static event Action<TileView, Vector2> OnTileDrag;

        /// <summary>
        /// Sets the tile's initial visual state based on its data model.
        /// </summary>
        /// <param name="tileData">The data model for this tile.</param>
        public void Initialize(TileData tileData)
        {
            this.GridPosition = tileData.Position;
            UpdateVisuals(tileData);
            SetSelected(false);
        }

        /// <summary>
        /// Updates the tile's visuals (sprite, color, locked state) to match new data.
        /// </summary>
        /// <param name="tileData">The new data for the tile.</param>
        public void UpdateVisuals(TileData tileData)
        {
            // TODO: Implement a mapping from TileData.SymbolType to an actual Sprite
            // symbolImage.sprite = SymbolManager.Instance.GetSpriteFor(tileData.SymbolType);
            
            // TODO: Implement a color mapping that respects accessibility settings
            // background.color = ColorManager.Instance.GetColorFor(tileData.ColorType, AccessibilityManager.Instance.CurrentColorblindMode);

            lockedOverlay.SetActive(tileData.IsLocked);
        }

        /// <summary>
        /// Plays an animation to indicate the tile has been selected or deselected.
        /// </summary>
        /// <param name="isSelected">True if the tile is now selected, false otherwise.</param>
        public void SetSelected(bool isSelected)
        {
            selectionHighlight.SetActive(isSelected);
            if (isSelected)
            {
                transform.DOKill();
                transform.DOPunchScale(Vector3.one * selectionPunchScale, selectionAnimDuration, 1, 0.5f);
            }
        }
        
        /// <summary>
        /// Plays a "shake" animation to indicate an invalid action.
        /// </summary>
        public void AnimateError()
        {
            if (AccessibilityManager.Instance != null && AccessibilityManager.Instance.IsReducedMotionEnabled) return;

            transform.DOKill();
            transform.DOShakePosition(errorShakeDuration, errorShakeStrength);
        }


        /// <summary>
        /// Plays an animation to visualize a state change, e.g., a tile being cleared.
        /// </summary>
        public void PlayStateChangeAnimation(TileData newData)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(Vector3.one * 1.2f, 0.15f));
            sequence.AppendCallback(() => UpdateVisuals(newData));
            sequence.Append(transform.DOScale(Vector3.one, 0.15f));
        }

        #region Input Handlers
        public void OnPointerDown(PointerEventData eventData)
        {
            OnTilePointerDown?.Invoke(this);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            OnTilePointerUp?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnTileDrag?.Invoke(this, eventData.delta);
        }
        #endregion
    }
}