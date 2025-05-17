using System;
using System.IO;
using PatternCipher.Utilities.Configuration;
using PatternCipher.Utilities.Serialization;
using PatternCipher.Utilities.Common.Exceptions;

namespace PatternCipher.Utilities.Configuration.Internal
{
    /// <summary>
    /// Internal implementation of IConfigurationReader for reading JSON formatted configuration data.
    /// </summary>
    /// <remarks>
    /// Uses an ISerializationService (typically configured for JSON) to parse JSON data from a file or string into a strongly-typed configuration object. Handles file I/O operations.
    /// </remarks>
    internal class JsonConfigurationReader : IConfigurationReader
    {
        private readonly ISerializationService _serializationService;

        /// <summary>
        /// Initializes a new instance of the JsonConfigurationReader.
        /// </summary>
        /// <param name="serializationService">The serialization service to use for JSON parsing. It's expected this service handles JSON.</param>
        /// <exception cref="ArgumentNullException">Thrown if serializationService is null.</exception>
        public JsonConfigurationReader(ISerializationService serializationService)
        {
            _serializationService = serializationService ?? throw new ArgumentNullException(nameof(serializationService));
        }

        /// <summary>
        /// Reads configuration from a JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the configuration object.</typeparam>
        /// <param name="configData">The string containing the JSON configuration data.</param>
        /// <returns>An object instance of type T containing the configuration.</returns>
        /// <exception cref="ArgumentNullException">Thrown if configData is null or empty.</exception>
        /// <exception cref="ConfigurationException">Thrown if deserialization or parsing fails, wrapping the underlying SerializationException.</exception>
        public T ReadConfigurationFromString<T>(string configData)
        {
            if (string.IsNullOrEmpty(configData))
            {
                 throw new ArgumentNullException(nameof(configData), $"Configuration data string cannot be null or empty when reading for type {typeof(T).FullName}.");
            }

            try
            {
                return _serializationService.Deserialize<T>(configData);
            }
            catch (SerializationException sex)
            {
                 throw new ConfigurationException($"Failed to parse configuration string for type {typeof(T).FullName}. See inner exception for details.", sex);
            }
            catch (Exception ex) // Catch any other unexpected errors
            {
                 throw new ConfigurationException($"An unexpected error occurred while reading configuration from string for type {typeof(T).FullName}.", ex);
            }
        }

        /// <summary>
        /// Reads configuration from a JSON file path.
        /// </summary>
        /// <typeparam name="T">The type of the configuration object.</typeparam>
        /// <param name="filePath">The path to the JSON configuration file.</param>
        /// <returns>An object instance of type T containing the configuration.</returns>
        /// <exception cref="ArgumentNullException">Thrown if filePath is null or empty.</exception>
        /// <exception cref="ConfigurationException">Thrown if the file is not found, cannot be read, or parsing fails (wraps underlying exceptions like FileNotFoundException, IOException, or SerializationException).</exception>
        public T ReadConfigurationFromFile<T>(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                 throw new ArgumentNullException(nameof(filePath), "File path cannot be null or empty.");
            }

            try
            {
                string configJson = File.ReadAllText(filePath);
                // Now use the string reading method
                return ReadConfigurationFromString<T>(configJson);
            }
            catch (FileNotFoundException fnfex)
            {
                throw new ConfigurationException($"Configuration file not found at path: {filePath}", fnfex);
            }
            catch (DirectoryNotFoundException dnfex)
            {
                throw new ConfigurationException($"Directory for configuration file not found at path: {filePath}", dnfex);
            }
            catch (IOException ioex)
            {
                // Covers various I/O errors like access denied, sharing violation etc.
                throw new ConfigurationException($"Error reading configuration file at path: {filePath}. IO Error: {ioex.Message}", ioex);
            }
            catch (UnauthorizedAccessException uaex)
            {
                throw new ConfigurationException($"Access denied while reading configuration file at path: {filePath}", uaex);
            }
            catch (System.Security.SecurityException secEx)
            {
                 throw new ConfigurationException($"Security error while reading configuration file at path: {filePath}", secEx);
            }
            catch (ConfigurationException) // Re-throw ConfigurationException from ReadConfigurationFromString
            {
                throw;
            }
            catch (Exception ex) // Catch any other unexpected errors from File.ReadAllText or other issues
            {
                 throw new ConfigurationException($"An unexpected error occurred while reading configuration from file at path: {filePath}", ex);
            }
        }
    }
}