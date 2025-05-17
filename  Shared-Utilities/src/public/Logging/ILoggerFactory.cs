using System;

namespace PatternCipher.Utilities.Logging
{
    /// <summary>
    /// Defines the public contract for a factory that creates ILogger instances.
    /// </summary>
    /// <remarks>
    /// Abstracts the creation of logger instances, allowing for different underlying logging mechanisms or configurations.
    /// </remarks>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Creates a logger instance for a specific category.
        /// </summary>
        /// <param name="categoryName">The name of the category the logger belongs to (e.g., class name, feature area).</param>
        /// <returns>An ILogger instance.</returns>
        ILogger CreateLogger(string categoryName);
    }
}