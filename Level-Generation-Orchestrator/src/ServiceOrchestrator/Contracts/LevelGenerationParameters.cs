using System.Collections.Generic;

namespace PatternCipher.Services.Contracts
{
    /// <summary>
    /// Data contract for detailed level generation parameters.
    /// Represents the concrete parameters fed into the procedural level generator adapter,
    /// derived from difficulty and remote config.
    /// </summary>
    public class LevelGenerationParameters
    {
        /// <summary>
        /// The seed value for the random number generator used in level creation.
        /// </summary>
        public int Seed { get; set; }

        /// <summary>
        /// The horizontal size (number of columns) of the game grid.
        /// </summary>
        public int GridSizeX { get; set; }

        /// <summary>
        /// The vertical size (number of rows) of the game grid.
        /// </summary>
        public int GridSizeY { get; set; }

        /// <summary>
        /// The number of distinct symbols or elements to use in the level.
        /// </summary>
        public int SymbolCount { get; set; }

        /// <summary>
        /// A list of constraints or rules to apply during generation (e.g., "no_match_at_start", "min_chain_length:3").
        /// </summary>
        public List<string> Constraints { get; set; }

        public LevelGenerationParameters()
        {
            Constraints = new List<string>();
        }
    }
}