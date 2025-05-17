namespace PatternCipher.Services.VersionManagement.Models
{
    /// <summary>
    /// Represents version 2 of the level data format.
    /// Used as a target for data migration from V1 or as a source for migration to CurrentLevelData.
    /// </summary>
    public class LevelDataV2
    {
        public string LevelID { get; set; }
        public byte[] GridData { get; set; } // More efficient binary storage for layout
        public string DifficultyKey { get; set; } // e.g., "easy_01", "medium_15"
        public int ParValue { get; set; }
        public string SolutionPathData { get; set; } // e.g., serialized list of moves
        public int Version { get; set; } = 2;
    }
}