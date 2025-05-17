namespace PatternCipher.Services.Contracts
{
    /// <summary>
    /// Data contract for requesting server-side level validation.
    /// Defines input for validating generated level parameters with the server,
    /// typically involving the raw level data or its hash.
    /// </summary>
    public class LevelValidationRequest
    {
        /// <summary>
        /// A hash of the level data, used for quick verification or integrity checks.
        /// </summary>
        public string LevelDataHash { get; set; }

        /// <summary>
        /// The raw data of the level to be validated.
        /// </summary>
        public object RawLevelData { get; set; }

        /// <summary>
        /// The difficulty parameters associated with the level being validated.
        /// </summary>
        public DifficultyParameters DifficultyParameters { get; set; }
    }
}