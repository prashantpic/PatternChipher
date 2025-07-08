namespace PatternCipher.Infrastructure.Persistence.Security
{
    /// <summary>
    /// Defines the contract for protecting and unprotecting save data.
    /// This abstracts the specific methods of obfuscation and checksum validation.
    /// </summary>
    public interface IDataProtector
    {
        /// <summary>
        /// Applies security measures to raw data, such as obfuscation and checksum calculation.
        /// </summary>
        /// <param name="jsonData">The clean JSON data to protect.</param>
        /// <returns>The protected (e.g., obfuscated, encrypted, Base64 encoded) data string.</returns>
        string Protect(string jsonData);

        /// <summary>
        /// Reverses the protection process, validates data integrity, and returns the original data.
        /// </summary>
        /// <param name="protectedData">The protected data string to unprotect.</param>
        /// <returns>The original, clean JSON data string.</returns>
        /// <exception cref="Exceptions.SaveDataCorruptionException">Thrown if the data fails an integrity check (e.g., checksum mismatch).</exception>
        string Unprotect(string protectedData);
    }
}