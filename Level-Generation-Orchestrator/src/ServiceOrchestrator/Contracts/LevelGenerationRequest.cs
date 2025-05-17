namespace PatternCipher.Services.Contracts
{
    /// <summary>
    /// Data contract for requesting level generation.
    /// Defines the input parameters for a level generation request, such as desired difficulty,
    /// player progression context, or specific generation seeds.
    /// </summary>
    public class LevelGenerationRequest
    {
        /// <summary>
        /// Key identifying the desired difficulty or type of level to generate.
        /// </summary>
        public string DifficultyKey { get; set; }

        /// <summary>
        /// Context of the player's current progression in the game.
        /// </summary>
        public PlayerProgression PlayerProgressionContext { get; set; }

        /// <summary>
        /// Optional seed for deterministic level generation. If null, a random seed may be used.
        /// </summary>
        public int? GenerationSeed { get; set; }

        /// <summary>
        /// Flag indicating whether to force regeneration even if a suitable cached level exists (not typically handled by this service directly, but could influence downstream).
        /// </summary>
        public bool ForceRegenerate { get; set; }
    }
}