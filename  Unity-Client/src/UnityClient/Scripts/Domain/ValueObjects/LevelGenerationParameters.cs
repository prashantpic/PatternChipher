using System.Collections.Generic;
using PatternCipher.Client.Domain.ValueObjects; // For GridDimensions

namespace PatternCipher.Client.Domain.ValueObjects
{
    public class LevelGenerationParameters
    {
        public GridDimensions Dimensions { get; }
        public List<string> AllowedSymbolIds { get; }
        public int MinMovesToSolve { get; } // Or a difficulty metric
        public int MaxMovesToSolve { get; } // Optional upper bound for complexity
        public int NumberOfColors { get; } // Example of symbol variety constraint
        public Dictionary<string, int> RequiredSpecialTiles { get; } // e.g., {"BOMB_ID", 2}

        // Add other parameters: e.g., objective type hints, blocker density, etc.
        // public ObjectiveTypeHint ObjectiveType { get; }
        // public float BlockerDensity { get; }


        public LevelGenerationParameters(
            GridDimensions dimensions,
            List<string> allowedSymbolIds,
            int minMovesToSolve,
            int numberOfColors,
            int maxMovesToSolve = 0, // 0 could mean no explicit max
            Dictionary<string, int> requiredSpecialTiles = null)
        {
            Dimensions = dimensions;
            AllowedSymbolIds = allowedSymbolIds ?? new List<string>();
            MinMovesToSolve = minMovesToSolve;
            NumberOfColors = numberOfColors;
            MaxMovesToSolve = maxMovesToSolve;
            RequiredSpecialTiles = requiredSpecialTiles ?? new Dictionary<string, int>();

            if (AllowedSymbolIds.Count == 0 && numberOfColors > 0)
            {
                // Potentially generate default symbol IDs based on numberOfColors
                // For now, assume AllowedSymbolIds are provided if specific IDs are needed,
                // or numberOfColors is used by generator to pick from a predefined palette.
            }
        }
    }
}