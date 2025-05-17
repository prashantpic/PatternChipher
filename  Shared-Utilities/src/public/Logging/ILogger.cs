using System;

namespace PatternCipher.Utilities.Logging
{
    /// <summary>
    /// Defines the public contract for a logging service, allowing for different levels of logging and structured data.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message with a specified level, optional exception, and structured event information.
        /// </summary>
        /// <param name="level">The severity level of the log entry.</param>
        /// <param name="message">The log message string.</param>
        /// <param name="exception">An optional exception associated with the log entry.</param>
        /// <param name="eventInfo">Optional structured properties for the log event.</param>
        void Log(LogLevel level, string message, System.Exception? exception = null, LogEventInfo? eventInfo = null);

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The debug message string.</param>
        /// <param name="eventInfo">Optional structured properties for the log event.</param>
        void LogDebug(string message, LogEventInfo? eventInfo = null);

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The informational message string.</param>
        /// <param name="eventInfo">Optional structured properties for the log event.</param>
        void LogInfo(string message, LogEventInfo? eventInfo = null);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The warning message string.</param>
        /// <param name="eventInfo">Optional structured properties for the log event.</param>
        void LogWarning(string message, LogEventInfo? eventInfo = null);

        /// <summary>
        /// Logs an error message, optionally with an exception.
        /// </summary>
        /// <param name="message">The error message string.</param>
        /// <param name="exception">An optional exception associated with the error.</param>
        /// <param name="eventInfo">Optional structured properties for the log event.</param>
        void LogError(string message, System.Exception? exception = null, LogEventInfo? eventInfo = null);
    }
}