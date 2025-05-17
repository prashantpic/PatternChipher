using PatternCipher.Services.Contracts;
using System.Threading.Tasks;

namespace PatternCipher.Services.Interfaces
{
    /// <summary>
    /// Interface for coordinating with the puzzle solver.
    /// Defines the contract for interacting with the puzzle solver component
    /// to verify level solvability and retrieve solution metadata.
    /// </summary>
    public interface ISolverCoordinator
    {
        /// <summary>
        /// Asynchronously verifies the solvability of a given raw level data.
        /// </summary>
        /// <param name="rawLevelData">The raw data representing the level layout to be solved.</param>
        /// <param name="difficultyParameters">The difficulty parameters that might influence solving (e.g., move limits, specific rules).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="SolvabilityResult"/>.</returns>
        Task<SolvabilityResult> VerifySolvabilityAsync(object rawLevelData, DifficultyParameters difficultyParameters);
    }
}