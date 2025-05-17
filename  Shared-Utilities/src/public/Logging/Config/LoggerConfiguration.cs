using System;
using System.Collections.Generic;
using PatternCipher.Utilities.Logging.Output; // Required for ILogOutput

namespace PatternCipher.Utilities.Logging.Config
{
    /// <summary>
    /// Data Transfer Object (DTO) for configuring a logger instance, including its minimum log level and output targets.
    /// </summary>
    /// <remarks>
    /// Defines the behavior of a specific logger, such as which messages to log and where to send them. Supports disabling or reducing logging levels.
    /// </remarks>
    public class LoggerConfiguration
    {
        /// <summary>
        /// The minimum log level this logger will process. Messages with a level below this will be ignored.
        /// Default is LogLevel.Info.
        /// </summary>
        public LogLevel MinimumLevel { get; set; } = LogLevel.Info;

        /// <summary>
        /// A list of log outputs to which messages will be sent if they meet the minimum level.
        /// Initialized to an empty list.
        /// </summary>
        public List<ILogOutput> Outputs { get; set; } = new List<ILogOutput>();

        /// <summary>
        /// Initializes a new instance of the LoggerConfiguration class with default values.
        /// </summary>
        public LoggerConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LoggerConfiguration class with a specific minimum log level.
        /// </summary>
        /// <param name="minimumLevel">The minimum log level for this configuration.</param>
        public LoggerConfiguration(LogLevel minimumLevel)
        {
            MinimumLevel = minimumLevel;
        }

        /// <summary>
        /// Initializes a new instance of the LoggerConfiguration class with a specific minimum log level and a list of outputs.
        /// </summary>
        /// <param name="minimumLevel">The minimum log level for this configuration.</param>
        /// <param name="outputs">The list of log outputs for this configuration.</param>
        public LoggerConfiguration(LogLevel minimumLevel, List<ILogOutput> outputs)
        {
            MinimumLevel = minimumLevel;
            Outputs = outputs ?? new List<ILogOutput>();
        }
    }
}