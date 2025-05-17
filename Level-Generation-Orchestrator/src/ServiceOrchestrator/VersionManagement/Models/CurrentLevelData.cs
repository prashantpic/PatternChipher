using System;

namespace PatternCipher.Services.VersionManagement.Models
{
    /// <summary>
    /// Represents the most up-to-date schema for generated level data content
    /// that is subject to versioning and migration.
    /// The LevelDataMigrator aims to convert older versions to this format.
    /// This structure should align with the core persisted fields of GeneratedLevelData.
    /// </summary>
    public class CurrentLevelData
    {
        /// <summary>
        /// The latest version number for the level data format.
        /// The LevelDataMigrator uses this to determine the target version.
        /// </summary>
        public const int LatestVersion = 2; // Assuming LevelDataV2 is the current latest structure

        public string LevelID { get; set; }
        
        /// <summary>
        /// The raw layout data of the level, typically in a serialized format (e.g., byte array).
        /// This field is what's primarily versioned and migrated.
        /// </summary>
        public byte[] RawLayoutData { get; set; }
        
        public string SolutionPath { get; set; }
        public int ParValue { get; set; }
        public string DifficultyRating { get; set; } // e.g., "Easy", "Hard_Tier2"
        
        /// <summary>
        /// The version number of this specific level data instance.
        /// </summary>
        public int Version { get; set; }
        
        // Note: Fields like IsSolvable and GenerationTimestamp from GeneratedLevelData
        // are often runtime-determined or context-specific rather than part of the
        // core migrated data schema itself, but could be included if they are versioned.
        // For this model, we focus on fields that are typically stored and migrated.
    }
}