namespace PatternCipher.Services.Configuration
{
    /// <summary>
    /// Constants for Remote Config keys used by the orchestrator.
    /// Provides a centralized, static list of keys used for accessing specific values
    /// from Firebase Remote Config related to level generation and difficulty.
    /// </summary>
    public static class RemoteConfigKeys
    {
        /// <summary>
        /// Prefix for Remote Config keys that hold difficulty parameter structures.
        /// Actual keys might be like "Difficulty_Easy", "Difficulty_World1_Level5".
        /// </summary>
        public const string DifficultyRulesPrefix = "Difficulty_";

        /// <summary>
        /// Key for fetching global generation parameters or constraints from Remote Config.
        /// </summary>
        public const string GenerationGlobalParamsKey = "Generation_GlobalParams";
    }
}