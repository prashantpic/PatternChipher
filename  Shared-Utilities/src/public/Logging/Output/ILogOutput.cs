using System;
using PatternCipher.Utilities.Logging; // Required for LogLevel and LogEventInfo

namespace PatternCipher.Utilities.Logging.Output
{
    /// <summary>
    /// Defines the public contract for a log output destination, such as console, file, or a remote service.
    /// </summary>
    /// <remarks>
    /// Abstracts the actual writing of log messages, enabling different output mechanisms to be plugged into the logging framework.
    /// </remarks>
    public interface ILogOutput
    {
        /// <summary>
        /// Writes a log entry to the output destination.
        /// Implementations should handle formatting and writing based on their specific destination.
        /// </summary>
        /// <param name="level">The severity level of the log entry.</param>
        /// <param name="categoryName">The name of the logger category.</param>
        /// <param name="message">The log message string.</param>
        /// <param name="exception">An optional exception associated with the log entry. Can be null.</param>
        /// <param name="eventInfo">Optional structured properties for the log event. Can be null.</param>
        void WriteLog(LogLevel level, string categoryName, string message, System.Exception? exception, LogEventInfo? eventInfo);
    }
}