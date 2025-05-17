using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PatternCipher.UI.Coordinator.Interfaces
{
    /// <summary>
    /// Adapter interface for the GridView component from UI-Component-Library.
    /// Defines a contract for the UICoordinatorService to interact with the GridView component.
    /// </summary>
    public interface IGridViewAdapter
    {
        /// <summary>
        /// Sets up the grid with the provided data.
        /// </summary>
        /// <param name="gridData">Data to configure the grid (e.g., dimensions, initial tile states).</param>
        /// <returns>A task that represents the asynchronous setup operation.</returns>
        Task SetupGridAsync(object gridData);

        /// <summary>
        /// Updates the visuals of one or more tiles.
        /// </summary>
        /// <param name="tileUpdatePayload">Data containing information about which tiles to update and their new visual state.</param>
        void UpdateTileVisuals(object tileUpdatePayload);

        /// <summary>
        /// Highlights specified tiles on the grid.
        /// </summary>
        /// <param name="positions">A collection of tile positions (Vector2Int) to highlight.</param>
        /// <param name="highlightType">A string key or enum defining the type of highlight (e.g., "selected", "path", "error").</param>
        void HighlightTiles(IEnumerable<Vector2Int> positions, string highlightType);

        /// <summary>
        /// Animates the swapping of two tiles.
        /// </summary>
        /// <param name="pos1">The grid position of the first tile.</param>
        /// <param name="pos2">The grid position of the second tile.</param>
        /// <returns>A task that represents the asynchronous animation operation.</returns>
        Task AnimateTileSwap(Vector2Int pos1, Vector2Int pos2);

        /// <summary>
        /// Event triggered when a tile interaction occurs (e.g., tap, drag-release resulting in a swap).
        /// The object parameter should carry details about the interaction (e.g., tile ID, interaction type, positions involved).
        /// </summary>
        event Action<object> OnTileInteraction;
    }
}