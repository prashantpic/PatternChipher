using System;

namespace PatternCipher.Utilities.Logging
{
    /// <summary>
    /// Provides static methods to obtain ILogger instances and to configure the underlying ILoggerFactory.
    /// Acts as the main entry point for the logging framework.
    /// </summary>
    /// <remarks>
    /// Manages a static ILoggerFactory instance. If no factory is set, it may use a default (e.g., DefaultLoggerFactory creating NullLoggers or basic console loggers).
    /// Allows consumers to easily obtain logger instances and to configure the logging system's behavior (e.g., by providing a custom ILoggerFactory for platform-specific logging).
    /// </remarks>
    public static class LogManager
    {
        private static ILoggerFactory _factory;
        private static readonly object _lock = new object();

        // A default factory that produces NullLoggers if nothing is configured
        // or a simple console logger in debug builds might be useful, but
        // the description implies it might use DefaultLoggerFactory.
        // Let's make it return NullLogger until configured explicitly.
        private static ILoggerFactory DefaultNullFactory => new Internal.DefaultLoggerFactory(new Config.LoggerConfiguration { MinimumLevel = LogLevel.None }); // Create NullLogger by default

        /// <summary>
        /// Gets a logger instance for the specified category name.
        /// </summary>
        /// <param name="categoryName">The name of the category.</param>
        /// <returns>An ILogger instance.</returns>
        public static ILogger GetLogger(string categoryName)
        {
            // Use the configured factory, or the default factory if none is set
            ILoggerFactory currentFactory = _factory ?? DefaultNullFactory;
            return currentFactory.CreateLogger(categoryName);
        }

        /// <summary>
        /// Gets a logger instance for the specified type's full name as category.
        /// </summary>
        /// <param name="type">The type whose full name will be used as the category name.</param>
        /// <returns>An ILogger instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown if type is null.</exception>
        public static ILogger GetLogger(System.Type type)
        {
             if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return GetLogger(type.FullName ?? type.Name); // Use full name if available, otherwise name
        }

        /// <summary>
        /// Sets the logger factory to be used for creating logger instances.
        /// </summary>
        /// <param name="loggerFactory">The logger factory instance to use.</param>
        public static void SetLoggerFactory(ILoggerFactory loggerFactory)
        {
            lock (_lock)
            {
                _factory = loggerFactory;
            }
        }
    }
}