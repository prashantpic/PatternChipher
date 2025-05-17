namespace PatternCipher.Services.Contracts
{
    /// <summary>
    /// Data contract for difficulty parameters fetched from Remote Config.
    /// Represents the difficulty settings and progression rules that influence level generation
    /// (e.g., grid size, number of unique symbols, special tile frequency).
    /// </summary>
    public class DifficultyParameters
    {
        /// <summary>
        /// The target width of the game grid.
        /// </summary>
        public int GridWidth { get; set; }

        /// <summary>
        /// The target height of the game grid.
        /// </summary>
        public int GridHeight { get; set; }

        /// <summary>
        /// The number of unique symbols to be used in the level.
        /// </summary>
        public int NumberOfSymbols { get; set; }

        /// <summary>
        /// Rules or configurations for special tiles (e.g., frequency, types).
        /// The specific structure depends on game design.
        /// </summary>
        public object SpecialTileRules { get; set; } // Could be a more specific type or a dictionary
    }
}