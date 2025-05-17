namespace PatternCipher.Utilities.Logging
{
    /// <summary>
    /// Defines the available logging levels (e.g., Debug, Info, Warning, Error) to categorize log message severity.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Verbose debugging information.
        /// </summary>
        Debug,
        /// <summary>
        /// General informational messages about application progress.
        /// </summary>
        Info,
        /// <summary>
        /// Potential issues or non-critical problems.
        /// </summary>
        Warning,
        /// <summary>
        /// Errors that prevent normal operation but might be recoverable.
        /// </summary>
        Error,
        /// <summary>
        /// No logging messages (used for filtering).
        /// </summary>
        None
    }
}