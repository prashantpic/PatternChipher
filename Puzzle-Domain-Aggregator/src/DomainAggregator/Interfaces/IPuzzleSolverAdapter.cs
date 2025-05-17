using System.Threading.Tasks;
using PatternCipher.Domain.Aggregates.PuzzleInstance; // For PuzzleInstance
using PatternCipher.Models; // Assuming this namespace contains SolverResult

namespace PatternCipher.Domain.Interfaces
{
    /// <summary>
    /// Abstracts communication with puzzle solving and solvability verification logic.
    /// Implementation likely in REPO-UNITY-CLIENT.
    /// </summary>
    public interface IPuzzleSolverAdapter
    {
        /// <summary>
        /// Checks if a given puzzle instance is solvable.
        /// </summary>
        /// <param name="puzzle">The puzzle instance to check.</param>
        /// <returns>True if the puzzle is solvable, false otherwise.</returns>
        Task<bool> IsSolvableAsync(PuzzleInstance puzzle);

        /// <summary>
        /// Gets an optimal solution for the puzzle.
        /// </summary>
        /// <param name="puzzle">The puzzle instance.</param>
        /// <returns>A SolverResult containing information about the optimal solution.</returns>
        Task<SolverResult> GetOptimalSolutionAsync(PuzzleInstance puzzle);
    }
}