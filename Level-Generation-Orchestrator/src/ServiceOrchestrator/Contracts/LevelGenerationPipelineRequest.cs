namespace PatternCipher.Services.Contracts
{
    /// <summary>
    /// Data contract for input to the IGenerationPipeline.
    /// Defines the specific parameters required by the IGenerationPipeline's ExecutePipelineAsync method.
    /// </summary>
    public class LevelGenerationPipelineRequest
    {
        /// <summary>
        /// The concrete parameters to be used for procedural level generation.
        /// </summary>
        public LevelGenerationParameters GenerationParameters { get; set; }

        /// <summary>
        /// The difficulty parameters associated with this generation attempt.
        /// </summary>
        public DifficultyParameters DifficultyParameters { get; set; }

        /// <summary>
        /// The current attempt number for this pipeline execution (e.g., 1 for initial, >1 for retries/fallbacks).
        /// </summary>
        public int AttemptNumber { get; set; }
    }
}