namespace PatternCipher.Services.Contracts
{
    /// <summary>
    /// Data contract for the result of server-side level validation.
    /// Defines the output from a server-side level validation, indicating success
    /// or failure and any relevant feedback.
    /// </summary>
    public class LevelValidationResult
    {
        /// <summary>
        /// Indicates whether the level is considered valid by the server.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// A message from the server providing details about the validation outcome, especially in case of failure.
        /// </summary>
        public string ValidationMessage { get; set; }
    }
}