using System;
using PatternCipher.Utilities.Logging;

namespace PatternCipher.Utilities.Logging.Internal
{
    /// <summary>
    /// Internal ILogger implementation that performs no logging operations. Used as a default or when logging is disabled.
    /// </summary>
    /// <remarks>
    /// Implements all ILogger methods as empty operations, ensuring no side effects or errors if used.
    /// This is an example of the Null Object pattern.
    /// </remarks>
    internal class NullLogger : ILogger
    {
        /// <summary>
        /// Private constructor to enforce singleton or factory-based instantiation.
        /// </summary>
        internal NullLogger() { }

        /// <summary>
        /// Logs a message (no operation).
        /// </summary>
        public void Log(LogLevel level, string message, Exception exception = null, LogEventInfo eventInfo = null)
        {
            // No operation
        }

        /// <summary>
        /// Logs a debug message (no operation).
        /// </summary>
        public void LogDebug(string message, LogEventInfo eventInfo = null)
        {
            // No operation
        }

        /// <summary>
        /// Logs an informational message (no operation).
        /// </summary>
        public void LogInfo(string message, LogEventInfo eventInfo = null)
        {
            // No operation
        }

        /// <summary>
        /// Logs a warning message (no operation).
        /// </summary>
        public void LogWarning(string message, LogEventInfo eventInfo = null)
        {
            // No operation
        }

        /// <summary>
        /// Logs an error message (no operation).
        /// </summary>
        public void LogError(string message, Exception exception = null, LogEventInfo eventInfo = null)
        {
            // No operation
        }
    }
}