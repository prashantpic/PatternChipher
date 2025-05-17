using PatternCipher.Client.Domain.Aggregates; // For GridAggregate, LevelProfileAggregate
// Potentially ValueObjects if target patterns use GridPosition etc.
// using PatternCipher.Client.Domain.ValueObjects; 

namespace PatternCipher.Client.Domain.Entities
{
    public enum ObjectiveType
    {
        ClearAllTiles,
        CollectSpecificSymbols,
        AchieveScore,
        BreakBlockers,
        MatchPattern
        // Add other objective types
    }

    public class LevelObjective
    {
        public ObjectiveType Type { get; private set; }
        public string TargetSymbolId { get; private set; } // For CollectSpecificSymbols
        public int RequiredCount { get; private set; }    // For CollectSpecificSymbols, BreakBlockers
        public int TargetScore { get; private set; }      // For AchieveScore
        // public GridPattern TargetPattern { get; private set; } // For MatchPattern (GridPattern would be a custom VO)

        // Example constructor, can be overloaded or use a factory
        public LevelObjective(ObjectiveType type, int targetScore = 0, string targetSymbolId = null, int requiredCount = 0)
        {
            Type = type;
            TargetScore = targetScore;
            TargetSymbolId = targetSymbolId;
            RequiredCount = requiredCount;
        }

        public bool IsCompleted(GridAggregate currentGrid, LevelProfileAggregate levelProfile)
        {
            if (currentGrid == null || levelProfile == null)
            {
                // Consider logging an error or throwing an ArgumentNullException
                return false;
            }

            switch (Type)
            {
                case ObjectiveType.AchieveScore:
                    return levelProfile.CurrentScore >= TargetScore;
                
                case ObjectiveType.CollectSpecificSymbols:
                    // This would require levelProfile to track collected symbols or query grid for specific symbols that are "collected"
                    // For simplicity, let's assume levelProfile tracks collected counts.
                    // return levelProfile.GetCollectedSymbolCount(TargetSymbolId) >= RequiredCount;
                    return false; // Placeholder: Requires more state in LevelProfileAggregate or different logic

                case ObjectiveType.ClearAllTiles:
                    // This would typically mean no "matchable" or "blocker" tiles remain.
                    // Requires GridAggregate to provide information about remaining clearable tiles.
                    // return currentGrid.GetRemainingClearableTilesCount() == 0;
                    return false; // Placeholder

                case ObjectiveType.BreakBlockers:
                    // Similar to ClearAllTiles, but specific to blocker-type tiles.
                    // return currentGrid.GetRemainingBlockerTilesCount() == 0;
                     return false; // Placeholder
                
                case ObjectiveType.MatchPattern:
                    // This would involve checking if a specific pattern of symbols exists on the grid.
                    // return currentGrid.CheckForPattern(TargetPattern);
                    return false; // Placeholder

                default:
                    return false;
            }
        }
    }
}