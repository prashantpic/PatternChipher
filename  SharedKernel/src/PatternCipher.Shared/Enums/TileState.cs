namespace PatternCipher.Shared.Enums
{
    /// <summary>
    /// Defines the possible behavioral states for a Tile.
    /// </summary>
    public enum TileState
    {
        /// <summary>Standard interactive tile.</summary>
        Default,
        /// <summary>Tile cannot be moved or changed directly.</summary>
        Locked,
        /// <summary>Tile can match any symbol or color.</summary>
        Wildcard,
        /// <summary>Tile that blocks interaction or movement.</summary>
        Obstacle
    }
}