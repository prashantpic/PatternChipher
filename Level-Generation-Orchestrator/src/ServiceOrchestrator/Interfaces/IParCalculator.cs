using PatternCipher.Services.Contracts;
using System.Threading.Tasks;

namespace PatternCipher.Services.Interfaces
{
    /// <summary>
    /// Interface for the par calculator.
    /// Defines the contract for calculating the par (target move count)
    /// of a generated and solved level.
    /// </summary>
    public interface IParCalculator
    {
        /// <summary>
        /// Asynchronously calculates the par value for a level.
        /// </summary>
        /// <param name="solvedLevelData">The data of the level that has been solved. This might include metrics beyond the solution itself.</param>
        /// <param name="solvabilityResult">The result from the solver, including the solution path and moves.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the calculated par value (integer).</returns>
        Task<int> CalculateParAsync(object solvedLevelData, SolvabilityResult solvabilityResult);
    }
}