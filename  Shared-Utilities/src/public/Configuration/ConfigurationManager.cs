using PatternCipher.Utilities.Serialization;

namespace PatternCipher.Utilities.Configuration
{
    /// <summary>
    /// Provides a static factory method to obtain instances of IConfigurationReader, typically for JSON configuration.
    /// </summary>
    /// <remarks>
    /// Provides an instance of JsonConfigurationReader, specialized for JSON format, cast to IConfigurationReader.
    /// </remarks>
    public static class ConfigurationManager
    {
        private static IConfigurationReader _jsonReaderInstance;
        private static readonly object _jsonReaderLock = new object();

        /// <summary>
        /// Gets an IConfigurationReader instance for JSON files.
        /// </summary>
        /// <remarks>
        /// Uses the default ISerializationService instance from SerializationManager internally.
        /// </remarks>
        public static IConfigurationReader JsonReaderInstance
        {
            get
            {
                // Implement thread-safe lazy initialization
                if (_jsonReaderInstance == null)
                {
                    lock (_jsonReaderLock)
                    {
                        if (_jsonReaderInstance == null)
                        {
                            // The JsonConfigurationReader needs an ISerializationService.
                            // It should use the default instance from SerializationManager.
                            _jsonReaderInstance = new Internal.JsonConfigurationReader(SerializationManager.Instance);
                        }
                    }
                }
                return _jsonReaderInstance;
            }
        }

        // Could add methods here to set a custom JSON reader instance,
        // or get readers for other formats if implemented.
    }
}