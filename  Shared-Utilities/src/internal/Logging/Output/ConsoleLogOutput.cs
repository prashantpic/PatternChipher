using System;
using PatternCipher.Utilities.Logging;
using PatternCipher.Utilities.Logging.Output;

namespace PatternCipher.Utilities.Logging.Output.Internal
{
    /// <summary>
    /// Internal implementation of ILogOutput that writes log messages to the system console.
    /// </summary>
    /// <remarks>
    /// Directs formatted log messages to the standard console output, useful for development and server environments.
    /// Formats the log message, level, category, exception, and event info into a string and writes it to System.Console.
    /// </remarks>
    internal class ConsoleLogOutput : ILogOutput
    {
        /// <summary>
        /// Writes a log entry to System.Console.
        /// </summary>
        /// <param name="level">The severity level.</param>
        /// <param name="categoryName">The logger category name.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">An optional exception.</param>
        /// <param name="eventInfo">Optional structured properties.</param>
        public void WriteLog(LogLevel level, string categoryName, string message, Exception exception, LogEventInfo eventInfo)
        {
            // Simple formatting for console output
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff UTC");
            string formattedMessage = $"{timestamp} [{level.ToString().ToUpper()}] ({categoryName}) - {message}";

            // Append exception details if present
            if (exception != null)
            {
                formattedMessage += Environment.NewLine + $"Exception: {exception.GetType().FullName}: {exception.Message}";
                formattedMessage += Environment.NewLine + $"StackTrace: {exception.StackTrace}";
                // Optionally include inner exceptions
                if (exception.InnerException != null)
                {
                     formattedMessage += Environment.NewLine + $"Inner Exception: {exception.InnerException.GetType().FullName}: {exception.InnerException.Message}";
                     // Inner exception stack trace might be useful too
                }
            }

            // Append event info properties if present
            if (eventInfo != null && eventInfo.Properties != null && eventInfo.Properties.Count > 0)
            {
                 formattedMessage += Environment.NewLine + "Properties:";
                 foreach(var pair in eventInfo.Properties)
                 {
                     formattedMessage += Environment.NewLine + $"  {pair.Key}: {pair.Value}";
                 }
            }

            // Use appropriate Console method based on level for potential color coding etc.
            // System.Console doesn't have built-in colors per WriteLine, but we could manually set ForegroundColor.
            // For simplicity, just use WriteLine here.
            System.Console.WriteLine(formattedMessage);
        }
    }
}