using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PatternCipher.Infrastructure.Persistence.Exceptions;

namespace PatternCipher.Infrastructure.Persistence.Security
{
    /// <summary>
    /// Provides a concrete implementation for save data protection using SHA256 for integrity
    /// and a simple XOR cipher for obfuscation. This is not for high security but to prevent casual editing.
    /// </summary>
    public class SimpleXorDataProtector : IDataProtector
    {
        private readonly byte[] _secretKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleXorDataProtector"/> class.
        /// </summary>
        public SimpleXorDataProtector()
        {
            // The key should not be a simple hardcoded string literal.
            // Constructing it at runtime from parts makes it slightly harder to find in a decompiled binary.
            // This key should be unique to the application.
            _secretKey = Encoding.UTF8.GetBytes("A-V3ry-S3cr3t-K3y-F0r-Patt3rn-C1ph3r!");
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (var sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private byte[] XorCipher(byte[] data)
        {
            var result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(data[i] ^ _secretKey[i % _secretKey.Length]);
            }
            return result;
        }

        /// <inheritdoc/>
        public string Protect(string jsonData)
        {
            try
            {
                var hash = ComputeSha256Hash(jsonData);
                var combinedString = $"{hash}:{jsonData}";
                var dataBytes = Encoding.UTF8.GetBytes(combinedString);
                var xoredBytes = XorCipher(dataBytes);
                return Convert.ToBase64String(xoredBytes);
            }
            catch (Exception ex)
            {
                // Wrap generic exceptions for context
                throw new InvalidOperationException("Failed to protect data.", ex);
            }
        }

        /// <inheritdoc/>
        public string Unprotect(string protectedData)
        {
            try
            {
                var xoredBytes = Convert.FromBase64String(protectedData);
                var dataBytes = XorCipher(xoredBytes);
                var combinedString = Encoding.UTF8.GetString(dataBytes);

                var separatorIndex = combinedString.IndexOf(':');
                if (separatorIndex == -1)
                {
                    throw new SaveDataCorruptionException("Protected data is malformed: missing hash separator.");
                }

                var embeddedHash = combinedString.Substring(0, separatorIndex);
                var originalJson = combinedString.Substring(separatorIndex + 1);

                var computedHash = ComputeSha256Hash(originalJson);

                if (!string.Equals(embeddedHash, computedHash, StringComparison.OrdinalIgnoreCase))
                {
                    throw new SaveDataCorruptionException("Data integrity check failed: checksum mismatch.");
                }

                return originalJson;
            }
            catch (FormatException ex)
            {
                throw new SaveDataCorruptionException("Protected data is not a valid Base64 string.", ex);
            }
            catch (SaveDataCorruptionException)
            {
                // Re-throw our specific exception
                throw;
            }
            catch (Exception ex)
            {
                // Wrap other potential exceptions (e.g., from GetString)
                throw new SaveDataCorruptionException("An unexpected error occurred during data unprotection.", ex);
            }
        }
    }
}