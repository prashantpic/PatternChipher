using System;

namespace PatternCipher.Services.Contracts
{
    /// <summary>
    /// Data contract for the output of level generation.
    /// Defines the structure of a successfully generated level's data, including its layout,
    /// solution path metadata, par value, and version.
    /// </summary>
    public class GeneratedLevelData
    {
        /// <summary>
        /// Unique identifier for the generated level.
        /// </summary>
        public string LevelID { get; set; }

        /// <summary>
        /// The raw data representing the level's layout. The specific type depends on the generator.
        /// </summary>
        public object RawLayoutData { get; set; }

        /// <summary>
        /// A representation of the solution path (e.g., a sequence of moves in JSON format).
        /// </summary>
        public string SolutionPath { get; set; }

        /// <summary>
        /// The target "par" score (e.g., number of moves) for the level.
        /// </summary>
        public int ParValue { get; set; }

        /// <summary>
        /// A numerical or descriptive rating of the level's difficulty.
        /// </summary>
        public float DifficultyRating { get; set; }

        /// <summary>
        /// The version number of this level data format.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Indicates whether the level has been verified as solvable.
        /// </summary>
        public bool IsSolvable { get; set; }

        /// <summary>
        /// Timestamp of when the level was generated.
        /// </summary>
        public DateTime GenerationTimestamp { get; set; }

        public GeneratedLevelData() { }

        public GeneratedLevelData(string levelID, object rawLayoutData, string solutionPath, int parValue, float difficultyRating, int version, bool isSolvable, DateTime generationTimestamp)
        {
            LevelID = levelID;
            RawLayoutData = rawLayoutData;
            SolutionPath = solutionPath;
            ParValue = parValue;
            DifficultyRating = difficultyRating;
            Version = version;
            IsSolvable = isSolvable;
            GenerationTimestamp = generationTimestamp;
        }
    }
}