using PatternCipher.Services.Contracts;
using System.Threading.Tasks;

namespace PatternCipher.Services.Interfaces
{
    /// <summary>
    /// Adapter interface for the PuzzleSolver from REPO-UNITY-CLIENT.
    /// Abstracts the actual PuzzleSolver component residing in REPO-UNITY-CLIENT,
    /// used for solvability checks.
    /// </summary>
    public interface IPuzzleSolverAdapter
    {
        /// <summary>
        /// Asynchronously attempts to solve a puzzle given its raw level data and solver parameters.
        /// </summary>
        /// <param name="rawLevelData">The raw data of the level to be solved.</param>
        /// <param name="solverParams">Parameters to guide the solving process (e.g., search depth, timeout).</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains a <see cref="SolvabilityResult"/> indicating whether the level is solvable,
        /// the solution path, and other solver metrics.
        /// </returns>
        Task<SolvabilityResult> SolveAsync(object rawLevelData, SolverParameters solverParams);
    }
}