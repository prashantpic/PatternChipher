namespace PatternCipher.UI.Coordinator.VisualClarity
{
    /// <summary>
    /// Interface for coordinating the visual clarity of critical game information
    /// displayed through various UI components like HUD, Grid, etc.
    /// </summary>
    public interface IClarityCoordinator
    {
        /// <summary>
        /// Updates the UI display related to puzzle goals.
        /// </summary>
        /// <param name="goalData">Data object containing information about the current puzzle goals.</param>
        void UpdatePuzzleGoalDisplay(object goalData);

        /// <summary>
        /// Updates the visual representation of tiles on the game grid based on their state.
        /// </summary>
        /// <param name="tileStateData">Data object containing information about tile states.</param>
        void UpdateTileStateVisuals(object tileStateData);

        /// <summary>
        /// Updates or manages visual cues for special tiles on the grid.
        /// </summary>
        /// <param name="specialTileData">Data object containing information about special tiles that need cues.</param>
        void UpdateSpecialTileCues(object specialTileData);
    }
}