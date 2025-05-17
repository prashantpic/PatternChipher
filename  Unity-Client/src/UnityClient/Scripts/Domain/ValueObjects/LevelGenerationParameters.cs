using System.Collections.Generic;
using PatternCipher.Client.Domain.ValueObjects; // For GridDimensions

namespace PatternCipher.Client.Domain.ValueObjects
{
    // Define a simple enum for difficulty or use a more complex struct/class if needed
    public enum DifficultySetting
    {
        Easy,
        Medium,
        Hard,
        Custom
    }

    public class LevelGenerationParameters
    {
        public GridDimensions Dimensions { get; }
        public List<string> AllowedSymbolIds { get; }
        public int MinMovesToSolve { get; } // Or target par moves
        public DifficultySetting Difficulty { get; }
        
        // Example of special tile parameters
        public int MinSpecialTiles { get; }
        public int MaxSpecialTiles { get; }
        public List<string> AllowedSpecialSymbolIds { get; } // If special symbols have their own IDs

        // Example of objective configurations
        // This could be a more complex structure or a factory method to create LevelObjective
        public string ObjectiveTypeKey { get; } // e.g., "CollectSymbol", "ClearTiles"
        public Dictionary<string, object> ObjectiveParameters { get; }

        public LevelGenerationParameters(
            GridDimensions dimensions,
            List<string> allowedSymbolIds,
            int minMovesToSolve,
            DifficultySetting difficulty,
            int minSpecialTiles = 0,
            int maxSpecialTiles = 0,
            List<string> allowedSpecialSymbolIds = null,
            string objectiveTypeKey = null,
            Dictionary<string, object> objectiveParameters = null)
        {
            Dimensions = dimensions;
            AllowedSymbolIds = allowedSymbolIds ?? new List<string>();
            MinMovesToSolve = minMovesToSolve;
            Difficulty = difficulty;
            MinSpecialTiles = minSpecialTiles;
            MaxSpecialTiles = maxSpecialTiles;
            AllowedSpecialSymbolIds = allowedSpecialSymbolIds ?? new List<string>();
            ObjectiveTypeKey = objectiveTypeKey;
            ObjectiveParameters = objectiveParameters ?? new Dictionary<string, object>();
        }

        // Consider adding validation or factory methods for robust creation
    }
}