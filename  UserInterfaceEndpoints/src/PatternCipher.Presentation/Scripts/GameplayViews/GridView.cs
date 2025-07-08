using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PatternCipher.Domain; // Placeholder for domain models

namespace PatternCipher.Presentation.GameplayViews
{
    /// <summary>
    /// This view component is responsible for translating the abstract Grid model
    /// into a visible, interactive grid of tiles on the screen.
    /// It manages the lifecycle of TileView objects.
    /// </summary>
    /// <remarks>
    /// Renders the interactive game board and its tiles, and orchestrates animations
    /// for tile swaps and state changes.
    /// </remarks>
    public class GridView : MonoBehaviour
    {
        [Tooltip("The prefab for a single tile view.")]
        [SerializeField] private TileView tileViewPrefab;
        [Tooltip("The transform container that will hold all tile view objects.")]
        [SerializeField] private Transform gridContainer;

        private Dictionary<Vector2Int, TileView> tileViews = new Dictionary<Vector2Int, TileView>();
        
        /// <summary>
        /// Clears the existing grid and builds a new one based on the provided data.
        /// </summary>
        /// <param name="gridData">The model data representing the grid to be created.</param>
        public void CreateGrid(GridData gridData)
        {
            foreach (var tileView in tileViews.Values)
            {
                Destroy(tileView.gameObject);
            }
            tileViews.Clear();

            if (gridData == null || gridData.Tiles == null) return;

            for (int x = 0; x < gridData.Width; x++)
            {
                for (int y = 0; y < gridData.Height; y++)
                {
                    TileData tileData = gridData.GetTileAt(x, y);
                    if (tileData != null)
                    {
                        var newTileView = Instantiate(tileViewPrefab, gridContainer);
                        newTileView.Initialize(tileData);
                        tileViews[new Vector2Int(x, y)] = newTileView;
                    }
                }
            }
        }

        /// <summary>
        /// Finds a specific tile view and triggers an update of its visual state.
        /// </summary>
        /// <param name="position">The grid position of the tile to update.</param>
        /// <param name="tileData">The new data for the tile.</param>
        public void UpdateTileView(Vector2Int position, TileData tileData)
        {
            if (tileViews.TryGetValue(position, out TileView view))
            {
                view.UpdateVisuals(tileData);
            }
        }
        
        /// <summary>
        /// Smoothly animates the swapping of two tile views.
        /// </summary>
        /// <param name="posA">The grid position of the first tile.</param>
        /// <param name="posB">The grid position of the second tile.</param>
        /// <param name="onComplete">An action to invoke when the animation is finished.</param>
        public void AnimateSwap(Vector2Int posA, Vector2Int posB, Action onComplete)
        {
            if (tileViews.TryGetValue(posA, out TileView tileA) && tileViews.TryGetValue(posB, out TileView tileB))
            {
                Vector3 posA_world = tileA.transform.position;
                Vector3 posB_world = tileB.transform.position;

                // Update dictionary first
                tileViews[posA] = tileB;
                tileViews[posB] = tileA;
                tileB.GridPosition = posA;
                tileA.GridPosition = posB;

                var sequence = DOTween.Sequence();
                sequence.Append(tileA.transform.DOMove(posB_world, 0.3f).SetEase(Ease.OutQuad));
                sequence.Join(tileB.transform.DOMove(posA_world, 0.3f).SetEase(Ease.OutQuad));
                sequence.OnComplete(() => onComplete?.Invoke());
            }
            else
            {
                onComplete?.Invoke();
            }
        }
        
        /// <summary>
        /// Triggers a state change animation on a specific tile.
        /// </summary>
        /// <param name="position">The grid position of the tile.</param>
        /// <param name="newData">The new data for the tile.</param>
        public void AnimateTileStateChange(Vector2Int position, TileData newData)
        {
            if (tileViews.TryGetValue(position, out TileView view))
            {
                view.PlayStateChangeAnimation(newData);
            }
        }

        /// <summary>
        /// Returns the TileView instance at a given grid coordinate.
        /// </summary>
        /// <param name="position">The grid coordinate.</param>
        /// <returns>The TileView at that position, or null if not found.</returns>
        public TileView GetTileViewAt(Vector2Int position)
        {
            tileViews.TryGetValue(position, out TileView view);
            return view;
        }
    }
}