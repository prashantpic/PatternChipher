using System;

namespace PatternCipher.Utilities.DataProtection
{
    /// <summary>
    /// Data Transfer Object (DTO) for configuring data protection settings, such as obfuscation keys.
    /// </summary>
    public class DataProtectionSettings
    {
        /// <summary>
        /// Key used for XOR obfuscation.
        /// </summary>
        public byte[] ObfuscationKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the DataProtectionSettings class.
        /// </summary>
        public DataProtectionSettings()
        {
            // Initialize with a default empty key or null, consumers should set it.
            ObfuscationKey = Array.Empty<byte>();
        }

        /// <summary>
        /// Initializes a new instance of the DataProtectionSettings class with a specific obfuscation key.
        /// </summary>
        /// <param name="obfuscationKey">The key to use for obfuscation.</param>
        public DataProtectionSettings(byte[] obfuscationKey)
        {
            ObfuscationKey = obfuscationKey ?? throw new ArgumentNullException(nameof(obfuscationKey));
        }
    }
}