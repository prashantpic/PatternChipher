namespace PatternCipher.UI.Components.GridView.Enums
{
    /// <summary>
    /// Defines the different visual states a game tile can be in.
    /// These states are used by GridTileView to apply appropriate styling and visual feedback.
    /// Implements REQ-UIX-015.
    /// </summary>
    public enum TileVisualState
    {
        /// <summary>
        /// The default, interactive state of the tile.
        /// </summary>
        Normal,

        /// <summary>
        /// The tile is currently selected by the player.
        /// </summary>
        Selected,

        /// <summary>
        /// The tile is visually highlighted, perhaps as part of a hint or a successful match.
        /// </summary>
        Highlighted,

        /// <summary>
        /// The tile is locked and cannot be interacted with until a condition is met.
        /// </summary>
        Locked,

        /// <summary>
        /// The tile represents an obstacle or an unmovable/unmatchable element.
        /// </summary>
        Obstacle,

        /// <summary>
        /// The tile is a special 'key' tile, possibly part of an objective.
        /// </summary>
        Key
    }
}