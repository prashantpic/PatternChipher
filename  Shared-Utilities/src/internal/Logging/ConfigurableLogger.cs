using System;
using System.Collections.Generic;
using System.Text;
using PatternCipher.Utilities.Logging;
using PatternCipher.Utilities.Logging.Config;
using PatternCipher.Utilities.Logging.Output;

namespace PatternCipher.Utilities.Logging.Internal
{
    /// <summary>
    /// Internal core ILogger implementation that filters messages based on LoggerConfiguration and dispatches to configured ILogOutput instances.
    /// </summary>
    /// <remarks>
    /// Acts as the primary logger implementation, providing level filtering and routing messages to various outputs as per its configuration. Supports performance-friendly logging options.
    /// When a log method is called, it checks if the message's log level meets the configured minimum. If so, it iterates through all configured ILogOutput instances and calls their WriteLog method.
    /// </remarks>
    internal class ConfigurableLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly LoggerConfiguration _configuration;

        /// <summary>
        /// Initializes with a category name and logger configuration.
        /// </summary>
        /// <param name="categoryName">The name of the logger category.</param>
        /// <param name="configuration">The configuration for this logger.</param>
        /// <exception cref="ArgumentNullException">Thrown if categoryName or configuration is null.</exception>
        /// <exception cref="ArgumentException">Thrown if categoryName is empty.</exception>
        public ConfigurableLogger(string categoryName, LoggerConfiguration configuration)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                throw new ArgumentException("Category name cannot be null or empty.", nameof(categoryName));
            }
            _categoryName = categoryName;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Determines if logging is enabled for the specified level.
        /// </summary>
        /// <param name="level">The log level to check.</param>
        /// <returns>True if logging is enabled for the level, false otherwise.</returns>
        private bool IsEnabled(LogLevel level)
        {
            return level >= _configuration.MinimumLevel && _configuration.MinimumLevel != LogLevel.None;
        }

        /// <summary>
        /// Logs a message with a specified level, optional exception, and structured event information.
        /// </summary>
        /// <param name="level">The severity level of the log entry.</param>
        /// <param name="message">The log message string. Can be null or empty if an exception is provided.</param>
        /// <param name="exception">An optional exception associated with the log entry.</param>
        /// <param name="eventInfo">Optional structured properties for the log event.</param>
        public void Log(LogLevel level, string message, Exception? exception = null, LogEventInfo? eventInfo = null)
        {
            if (!IsEnabled(level) || _configuration.Outputs == null || _configuration.Outputs.Count == 0)
            {
                return; // Logging disabled for this level or no outputs configured
            }

            // Ensure there's a message, even if it's just from the exception
            string finalMessage = message ?? exception?.Message ?? string.Empty;

            foreach (var output in _configuration.Outputs)
            {
                try
                {
                    output.WriteLog(level, _categoryName, finalMessage, exception!, eventInfo!);
                }
                catch (Exception ex)
                {
                    // An error occurred in the log output itself.
                    // Avoid re-throwing to prevent application crash due to logging failure.
                    // Consider a fallback mechanism, e.g., writing to System.Diagnostics.Trace or Console.Error.
                    // For now, we'll write a simplified error to the console.
                    Console.Error.WriteLine($"--- Logging Output Error ({output.GetType().FullName}) ---");
                    Console.Error.WriteLine($"Failed to write log: Level={level}, Category={_categoryName}, Message='{finalMessage}'");
                    Console.Error.WriteLine($"Output Exception: {ex.GetType().FullName}: {ex.Message}");
                    Console.Error.WriteLine(ex.StackTrace);
                    Console.Error.WriteLine($"--- End Logging Output Error ---");
                }
            }
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The debug message string.</param>
        /// <param name="eventInfo">Optional structured properties for the log event.</param>
        public void LogDebug(string message, LogEventInfo? eventInfo = null)
        {
            if (IsEnabled(LogLevel.Debug)) // Quick check to avoid unnecessary Log call if disabled
            {
                Log(LogLevel.Debug, message, null, eventInfo);
            }
        }

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The informational message string.</param>
        /// <param name="eventInfo">Optional structured properties for the log event.</param>
        public void LogInfo(string message, LogEventInfo? eventInfo = null)
        {
             if (IsEnabled(LogLevel.Info))
            {
                Log(LogLevel.Info, message, null, eventInfo);
            }
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The warning message string.</param>
        /// <param name="eventInfo">Optional structured properties for the log event.</param>
        public void LogWarning(string message, LogEventInfo? eventInfo = null)
        {
            if (IsEnabled(LogLevel.Warning))
            {
                Log(LogLevel.Warning, message, null, eventInfo);
            }
        }

        /// <summary>
        /// Logs an error message, optionally with an exception.
        /// </summary>
        /// <param name="message">The error message string.</param>
        /// <param name="exception">An optional exception associated with the error.</param>
        /// <param name="eventInfo">Optional structured properties for the log event.</param>
        public void LogError(string message, Exception? exception = null, LogEventInfo? eventInfo = null)
        {
            if (IsEnabled(LogLevel.Error))
            {
                Log(LogLevel.Error, message, exception, eventInfo);
            }
        }
    }
}