using System;
using System.Threading.Tasks;
using PatternCipher.Domain.Interfaces;
using PatternCipher.Domain.Aggregates.PuzzleInstance; // For PuzzleInstance type
using PatternCipher.Domain.Entities;
using PatternCipher.Models; // For SolverResult

namespace PatternCipher.Domain.Services
{
    /// <summary>
    /// Orchestrates the process of obtaining the 'par' move count for a puzzle,
    /// likely by interacting with a solver service.
    /// </summary>
    public class ParDeterminationService
    {
        private readonly IPuzzleSolverAdapter _solverAdapter;

        public ParDeterminationService(IPuzzleSolverAdapter solverAdapter)
        {
            _solverAdapter = solverAdapter ?? throw new ArgumentNullException(nameof(solverAdapter));
        }

        /// <summary>
        /// Determines the par details for a given puzzle instance.
        /// Calls the puzzle solver adapter to find the optimal solution length and
        /// calculates par based on this length and potentially other configurations.
        /// </summary>
        /// <param name="puzzle">The puzzle instance for which to determine par.</param>
        /// <returns>The calculated par details for the puzzle.</returns>
        public async Task<ParDetails> DetermineParAsync(PuzzleInstance puzzle)
        {
            if (puzzle == null)
            {
                throw new ArgumentNullException(nameof(puzzle));
            }

            // The IPuzzleSolverAdapter.GetOptimalSolutionAsync expects PuzzleInstance itself according to point 4 of full plan.
            SolverResult solverResult = await _solverAdapter.GetOptimalSolutionAsync(puzzle);

            if (solverResult == null) // Or check solverResult.IsSolvable / solverResult.Success
            {
                // Handle case where solution cannot be found or puzzle is unsolvable
                // This might mean returning a default ParDetails or throwing an exception
                // For now, assume SolverResult contains OptimalPathLength, possibly 0 or -1 if not solved.
                // throw new InvalidOperationException($"Could not determine optimal solution for puzzle {puzzle.Id}.");
                // Return a ParDetails indicating failure or default.
                return new ParDetails(0, 0); // Example: 0 par if unsolvable or error
            }
            
            // OptimalSolutionLength is part of ParDetails entity definition.
            // Assuming SolverResult has a property like OptimalPathLength.
            // The SDS for ParDetails has ParMoveCount and OptimalSolutionLength.
            int optimalLength = solverResult.OptimalPathLength; // Assuming this property exists on SolverResult

            // ParMoveCount could be the optimal length, or optimal length + some buffer,
            // or based on a difficulty curve. For simplicity, let's assume par is optimal length.
            int parMoveCount = optimalLength; 
            // Example: parMoveCount = (int)Math.Ceiling(optimalLength * 1.2); // e.g., optimal + 20%

            return new ParDetails(parMoveCount, optimalLength);
        }
    }
}