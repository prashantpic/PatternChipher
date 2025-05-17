using System;
using PatternCipher.Utilities.Common.Exceptions;

namespace PatternCipher.Utilities.DataProtection
{
    /// <summary>
    /// Provides a static factory method and configuration for IDataProtectionProvider instances.
    /// </summary>
    /// <remarks>
    /// Initializes and returns a ConcreteDataProtectionProvider instance, configured with settings like an obfuscation key.
    /// Requires configuration before accessing the Instance.
    /// </remarks>
    public static class DataProtectionManager
    {
        private static IDataProtectionProvider _instance;
        private static readonly object _lock = new object();
        private static bool _isConfigured = false;

        /// <summary>
        /// Configures the data protection provider with specified settings.
        /// Must be called before accessing the Instance property.
        /// </summary>
        /// <param name="settings">The settings to configure the data protection provider.</param>
        /// <exception cref="ArgumentNullException">Thrown if settings is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the manager is already configured.</exception>
        public static void Configure(DataProtectionSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings), "Data protection settings cannot be null.");
            }

            lock (_lock)
            {
                if (_isConfigured)
                {
                    throw new InvalidOperationException("DataProtectionManager is already configured.");
                }

                _instance = new Internal.ConcreteDataProtectionProvider(settings);
                _isConfigured = true;
            }
        }

        /// <summary>
        /// Gets the configured IDataProtectionProvider instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the manager has not been configured.</exception>
        public static IDataProtectionProvider Instance
        {
            get
            {
                lock (_lock)
                {
                    if (!_isConfigured || _instance == null)
                    {
                        throw new InvalidOperationException("DataProtectionManager is not configured. Call Configure() first.");
                    }
                    return _instance;
                }
            }
        }
    }
}