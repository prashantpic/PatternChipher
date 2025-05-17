using System;
using System.Collections.Generic;
using PatternCipher.Utilities.Logging;
using PatternCipher.Utilities.Logging.Config;
using PatternCipher.Utilities.Logging.Output; // For ConsoleLogOutput
using PatternCipher.Utilities.Logging.Output.Internal; // For ConsoleLogOutput concrete type if namespace is Internal

namespace PatternCipher.Utilities.Logging.Internal
{
    /// <summary>
    /// Internal default implementation of ILoggerFactory. Creates ConfigurableLogger instances.
    /// </summary>
    /// <remarks>
    /// Provides a concrete factory for creating logger instances, typically ConfigurableLoggers, based on provided configurations.
    /// Instantiates ConfigurableLogger with a specific LoggerConfiguration, possibly merging category-specific settings with global defaults.
    /// </remarks>
    internal class DefaultLoggerFactory : ILoggerFactory
    {
        private readonly LoggerConfiguration _globalConfig;
        private readonly NullLogger _nullLoggerInstance = new NullLogger(); // Cache a single NullLogger instance

        /// <summary>
        /// Initializes with an optional global logger configuration.
        /// If no configuration is provided, it defaults to logging Info level and above to the Console.
        /// </summary>
        /// <param name="globalConfig">The global configuration to apply to loggers created by this factory.
        /// If null, a default configuration (Info level, Console output) is used.</param>
        public DefaultLoggerFactory(LoggerConfiguration? globalConfig = null)
        {
            if (globalConfig == null)
            {
                _globalConfig = new LoggerConfiguration
                {
                    MinimumLevel = LogLevel.Info,
                    Outputs = new List<ILogOutput> { new ConsoleLogOutput() } // Assuming ConsoleLogOutput is in Output.Internal
                };
            }
            else
            {
                _globalConfig = globalConfig;
            }
        }

        /// <summary>
        /// Creates a logger instance for a specific category.
        /// </summary>
        /// <param name="categoryName">The name of the category the logger belongs to (e.g., class name, feature area).</param>
        /// <returns>An ILogger instance. Returns a NullLogger if the global configuration is set to LogLevel.None or has no outputs.</returns>
        /// <exception cref="ArgumentNullException">Thrown if categoryName is null or empty.</exception>
        public ILogger CreateLogger(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                throw new ArgumentNullException(nameof(categoryName), "Logger category name cannot be null or empty.");
            }

            // If global config effectively disables logging, return a NullLogger to avoid overhead.
            if (_globalConfig.MinimumLevel == LogLevel.None || _globalConfig.Outputs == null || _globalConfig.Outputs.Count == 0)
            {
                return _nullLoggerInstance;
            }

            // In a more advanced system, categoryName could be used to fetch category-specific configurations
            // which would then be merged with _globalConfig. For this implementation, all loggers share _globalConfig.
            return new ConfigurableLogger(categoryName, _globalConfig);
        }
    }
}