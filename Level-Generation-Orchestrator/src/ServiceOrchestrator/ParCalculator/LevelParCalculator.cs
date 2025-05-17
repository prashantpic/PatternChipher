using PatternCipher.Services.Interfaces;
using PatternCipher.Services.Contracts;
using System.Threading.Tasks;
using System; // For ArgumentNullException
// using UnityEngine; // For Mathf if complex calculations were needed

namespace PatternCipher.Services.ParCalculator
{
    public class LevelParCalculator : IParCalculator
    {
        public Task<int> CalculateParAsync(object solvedLevelData, SolvabilityResult solvabilityResult)
        {
            if (solvabilityResult == null) throw new ArgumentNullException(nameof(solvabilityResult));
            if (!solvabilityResult.IsSolvable)
            {
                // Or throw an exception, or return a specific value like -1
                return Task.FromResult(0); 
            }

            // Simple par calculation: number of moves in the solution.
            // More complex logic could be:
            // - MovesInSolution + some_buffer
            // - MovesInSolution * difficulty_multiplier
            // - Based on solvedLevelData metrics + MovesInSolution
            int parValue = solvabilityResult.MovesInSolution;

            // Ensure par is not negative, for instance
            if (parValue < 0) parValue = 0;

            return Task.FromResult(parValue);
        }
    }
}