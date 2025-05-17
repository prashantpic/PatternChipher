using System;
using System.Security.Cryptography;
using System.Text;
using PatternCipher.Utilities.DataProtection;
using PatternCipher.Utilities.Common.Exceptions;

namespace PatternCipher.Utilities.DataProtection.Internal
{
    /// <summary>
    /// Internal implementation of IDataProtectionProvider providing SHA256 checksum and XOR/Base64 obfuscation.
    /// </summary>
    /// <remarks>
    /// Provides concrete implementations for data integrity checks (SHA256) and basic obfuscation (XOR with a key, Base64 encoding) to deter casual tampering.
    /// Implements checksum generation using SHA256. Obfuscation involves XORing data with a configured key, followed by Base64 encoding. Deobfuscation reverses this process.
    /// </remarks>
    internal class ConcreteDataProtectionProvider : IDataProtectionProvider
    {
        private readonly byte[] _obfuscationKey;
        private const int Sha256HashByteLength = 32; // SHA256 produces a 256-bit (32-byte) hash.

        /// <summary>
        /// Initializes with data protection settings.
        /// </summary>
        /// <param name="settings">The settings containing the obfuscation key.</param>
        /// <exception cref="ArgumentNullException">Thrown if settings is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the obfuscation key in settings is null or empty.</exception>
        public ConcreteDataProtectionProvider(DataProtectionSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (settings.ObfuscationKey == null || settings.ObfuscationKey.Length == 0)
            {
                throw new ArgumentException("Obfuscation key must be provided and cannot be empty.", nameof(settings.ObfuscationKey));
            }
            _obfuscationKey = settings.ObfuscationKey;
        }

        /// <summary>
        /// Generates a SHA256 checksum for the given data.
        /// </summary>
        /// <param name="data">The data for which to generate the checksum.</param>
        /// <returns>A hex string representation of the SHA256 hash.</returns>
        /// <exception cref="ArgumentNullException">Thrown if data is null.</exception>
        /// <exception cref="DataProtectionException">Thrown if checksum generation fails.</exception>
        public string GenerateChecksum(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            try
            {
                using (var sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(data);
                    // Convert byte array to a hex string
                    StringBuilder hex = new StringBuilder(hashBytes.Length * 2);
                    foreach (byte b in hashBytes)
                    {
                        hex.AppendFormat("{0:x2}", b);
                    }
                    return hex.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new DataProtectionException("Failed to generate SHA256 checksum.", ex);
            }
        }

        /// <summary>
        /// Validates data against an expected checksum (hex encoded SHA256 hash).
        /// </summary>
        /// <param name="data">The data to validate.</param>
        /// <param name="expectedChecksum">The expected hex checksum string.</param>
        /// <returns>True if the generated checksum matches the expected checksum (case-insensitive), false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if data or expectedChecksum is null.</exception>
        /// <exception cref="DataProtectionException">Thrown if checksum validation fails due to format errors or other issues.</exception>
        public bool ValidateChecksum(byte[] data, string expectedChecksum)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (string.IsNullOrEmpty(expectedChecksum))
            {
                throw new ArgumentNullException(nameof(expectedChecksum), "Expected checksum cannot be null or empty.");
            }

            try
            {
                string actualChecksum = GenerateChecksum(data);
                // Case-insensitive comparison for hex strings
                return string.Equals(actualChecksum, expectedChecksum, StringComparison.OrdinalIgnoreCase);
            }
            // GenerateChecksum can throw DataProtectionException, let it propagate.
            // No specific format exception for hex string comparison here.
            catch (DataProtectionException)
            {
                throw; // Re-throw if GenerateChecksum failed
            }
            catch (Exception ex)
            {
                 // Catch any other unexpected errors during comparison
                throw new DataProtectionException("An unexpected error occurred during checksum validation.", ex);
            }
        }
        
        /// <summary>
        /// Performs a byte-wise XOR operation between data and a key.
        /// The key is reused (cycled) if it's shorter than the data.
        /// </summary>
        private byte[] XorBytes(byte[] data, byte[] key)
        {
            // ArgumentNullException for data is handled by callers (Obfuscate/Deobfuscate)
            // Key validity (non-null, non-empty) is ensured by constructor
            byte[] result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(data[i] ^ key[i % key.Length]);
            }
            return result;
        }


        /// <summary>
        /// Obfuscates the given data using XOR with the configured key, then encodes the result to Base64.
        /// The output byte array is the UTF-8 representation of the Base64 string.
        /// </summary>
        /// <param name="data">The data to obfuscate.</param>
        /// <returns>The obfuscated data as a UTF-8 byte array of the Base64 string.</returns>
        /// <exception cref="ArgumentNullException">Thrown if data is null.</exception>
        /// <exception cref="DataProtectionException">Thrown if obfuscation fails.</exception>
        public byte[] Obfuscate(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            try
            {
                byte[] xoredData = XorBytes(data, _obfuscationKey);
                string base64EncodedData = Convert.ToBase64String(xoredData);
                return Encoding.UTF8.GetBytes(base64EncodedData);
            }
            catch (Exception ex)
            {
                throw new DataProtectionException("Failed to obfuscate data.", ex);
            }
        }

        /// <summary>
        /// Deobfuscates previously obfuscated data.
        /// Input byte array is expected to be a UTF-8 representation of a Base64 string.
        /// This string is Base64 decoded, then XORed with the configured key.
        /// </summary>
        /// <param name="obfuscatedData">The obfuscated data as a UTF-8 byte array of a Base64 string.</param>
        /// <returns>The original deobfuscated data.</returns>
        /// <exception cref="ArgumentNullException">Thrown if obfuscatedData is null or empty.</exception>
        /// <exception cref="DataProtectionException">Thrown if deobfuscation fails (e.g., invalid format, key issues).</exception>
        public byte[] Deobfuscate(byte[] obfuscatedData)
        {
            if (obfuscatedData == null || obfuscatedData.Length == 0)
            {
                throw new ArgumentNullException(nameof(obfuscatedData), "Obfuscated data cannot be null or empty.");
            }

            try
            {
                string base64EncodedData = Encoding.UTF8.GetString(obfuscatedData);
                byte[] xoredData = Convert.FromBase64String(base64EncodedData);
                return XorBytes(xoredData, _obfuscationKey);
            }
            catch (DecoderFallbackException dfex) // UTF-8 decoding failed
            {
                throw new DataProtectionException("Failed to deobfuscate data: Input is not valid UTF-8 encoded Base64 string.", dfex);
            }
            catch (FormatException fex) // Base64 decoding failed
            {
                throw new DataProtectionException("Failed to deobfuscate data: Input is not a valid Base64 string.", fex);
            }
            catch (Exception ex)
            {
                throw new DataProtectionException("Failed to deobfuscate data.", ex);
            }
        }
    }
}