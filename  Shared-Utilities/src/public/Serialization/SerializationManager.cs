namespace PatternCipher.Utilities.Serialization
{
    /// <summary>
    /// Provides a static factory method to obtain instances of ISerializationService. Configured to provide JsonSerializationService by default.
    /// </summary>
    public static class SerializationManager
    {
        private static ISerializationService _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Gets the default ISerializationService instance.
        /// </summary>
        /// <remarks>
        /// Initialized lazily to a singleton instance of JsonSerializationService.
        /// </remarks>
        public static ISerializationService Instance
        {
            get
            {
                // Implement thread-safe lazy initialization
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Internal.JsonSerializationService();
                        }
                    }
                }
                return _instance;
            }
        }

        // Although not strictly required by the JSON, adding a method to allow
        // setting a custom instance makes the manager more flexible and aligns
        // with typical factory/service locator patterns.
        /// <summary>
        /// Sets a custom ISerializationService instance to be used by the Manager.
        /// </summary>
        /// <param name="customInstance">The custom service instance to use.</param>
        public static void SetInstance(ISerializationService customInstance)
        {
            lock (_lock)
            {
                _instance = customInstance;
            }
        }
    }
}