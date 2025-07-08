using System;

namespace PatternCipher.Domain.Scoring.Services
{
    /// <summary>
    /// A stateless domain service responsible for calculating player scores,
    /// including bonuses like the efficiency bonus for completing a puzzle under par.
    /// </summary>
    public sealed class ScoringService
    {
        /// <summary>
        /// Calculates an efficiency bonus based on the number of moves taken versus the puzzle's par value.
        /// </summary>
        /// <param name="par">The 'par' or optimal number of moves for the puzzle.</param>
        /// <param name="movesTaken">The number of moves the player took to solve the puzzle.</param>
        /// <returns>A score bonus. Returns 0 if the player exceeded the par.</returns>
        public int CalculateEfficiencyBonus(int par, int movesTaken)
        {
            // In a real application, these values would likely come from a configuration file or a remote config service.
            const int pointsPerMoveSaved = 100;
            const int flatBonusForPar = 50;

            if (movesTaken > par || par <= 0)
            {
                return 0;
            }

            if (movesTaken == par)
            {
                return flatBonusForPar;
            }

            // movesTaken < par
            int movesSaved = par - movesTaken;
            return (movesSaved * pointsPerMoveSaved) + flatBonusForPar;
        }
    }
}