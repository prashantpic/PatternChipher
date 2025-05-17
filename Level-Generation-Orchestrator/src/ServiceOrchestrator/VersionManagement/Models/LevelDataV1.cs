namespace PatternCipher.Services.VersionManagement.Models
{
    /// <summary>
    /// Represents an older version (V1) of the level data format.
    /// Used as a source for data migration.
    /// </summary>
    public class LevelDataV1
    {
        public string OldLevelIdentifier { get; set; }
        public string SerializedGridV1 { get; set; } // e.g., a JSON string or custom format
        public int DifficultyNumber { get; set; }
        public int ParMovesV1 { get; set; }
        public string SolutionV1 { get; set; } // e.g., a string representation of moves
        // Assume version is implicitly 1 or not stored
    }
}