using System;

namespace PatternCipher.Utilities.DataProtection
{
    /// <summary>
    /// Defines the public contract for data protection services, including checksum generation/validation and data obfuscation/deobfuscation.
    /// </summary>
    public interface IDataProtectionProvider
    {
        /// <summary>
        /// Generates a checksum for the given data.
        /// </summary>
        /// <param name="data">The data for which to generate the checksum.</param>
        /// <returns>A string representing the generated checksum.</returns>
        string GenerateChecksum(byte[] data);

        /// <summary>
        /// Validates data against an expected checksum.
        /// </summary>
        /// <param name="data">The data to validate.</param>
        /// <param name="expectedChecksum">The expected checksum string.</param>
        /// <returns>True if the generated checksum matches the expected checksum, false otherwise.</returns>
        bool ValidateChecksum(byte[] data, string expectedChecksum);

        /// <summary>
        /// Obfuscates the given data.
        /// </summary>
        /// <param name="data">The data to obfuscate.</param>
        /// <returns>The obfuscated data as a byte array.</returns>
        byte[] Obfuscate(byte[] data);

        /// <summary>
        /// Deobfuscates previously obfuscated data.
        /// </summary>
        /// <param name="obfuscatedData">The obfuscated data as a byte array.</param>
        /// <returns>The original deobfuscated data.</returns>
        byte[] Deobfuscate(byte[] obfuscatedData);
    }
}