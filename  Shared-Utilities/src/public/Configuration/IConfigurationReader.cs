using System;

namespace PatternCipher.Utilities.Configuration
{
    /// <summary>
    /// Defines the public contract for reading configuration data from various sources into a typed object.
    /// </summary>
    public interface IConfigurationReader
    {
        /// <summary>
        /// Reads configuration from a string.
        /// </summary>
        /// <typeparam name="T">The type of the configuration object.</typeparam>
        /// <param name="configData">The string containing the configuration data.</param>
        /// <returns>An object instance of type T containing the configuration.</returns>
        T ReadConfigurationFromString<T>(string configData);

        /// <summary>
        /// Reads configuration from a file path.
        /// </summary>
        /// <typeparam name="T">The type of the configuration object.</typeparam>
        /// <param name="filePath">The path to the configuration file.</param>
        /// <returns>An object instance of type T containing the configuration.</returns>
        T ReadConfigurationFromFile<T>(string filePath);
    }
}