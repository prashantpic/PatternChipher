namespace PatternCipher.Domain.Entities
{
    /// <summary>
    /// Entity representing the 'par' move count and related metrics for a puzzle.
    /// Holds the target 'par' information for a puzzle, such as optimal move count, derived from solver analysis.
    /// </summary>
    public class ParDetails
    {
        public int ParMoveCount { get; }
        public int OptimalSolutionLength { get; }

        public ParDetails(int parMoveCount, int optimalSolutionLength)
        {
            if (parMoveCount < 0)
                throw new System.ArgumentOutOfRangeException(nameof(parMoveCount), "Par move count cannot be negative.");
            if (optimalSolutionLength < 0)
                throw new System.ArgumentOutOfRangeException(nameof(optimalSolutionLength), "Optimal solution length cannot be negative.");
            
            ParMoveCount = parMoveCount;
            OptimalSolutionLength = optimalSolutionLength;
        }
    }
}