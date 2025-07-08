namespace PatternCipher.Infrastructure.Persistence.Models
{
    /// <summary>
    /// A data transfer object (DTO) that represents the on-disk file structure.
    /// It wraps the actual player profile data with persistence-specific metadata.
    /// </summary>
    public class SaveDataWrapper
    {
        /// <summary>
        /// The schema version of the wrapped Payload.
        /// </summary>
        public int SchemaVersion { get; set; }

        /// <summary>
        /// A checksum for the payload to verify integrity.
        /// Note: Depending on the IDataProtector implementation, this might be redundant if the
        /// checksum is embedded within the payload itself. It is included here for flexibility.
        /// </summary>
        public string? Checksum { get; set; }

        /// <summary>
        /// The protected data payload. This is typically a Base64-encoded,
        /// obfuscated/encrypted string containing the serialized game data.
        /// </summary>
        public string Payload { get; set; } = string.Empty;
    }
}