using System;
using System.Threading.Tasks;
using PatternCipher.Domain.Aggregates.PuzzleInstance; // For PuzzleInstance

namespace PatternCipher.Domain.Interfaces
{
    /// <summary>
    /// Defines the contract for storing and retrieving PuzzleInstance aggregate roots.
    /// Implementation would be in an infrastructure layer.
    /// </summary>
    public interface IPuzzleInstanceRepository
    {
        /// <summary>
        /// Retrieves a puzzle instance by its ID.
        /// </summary>
        /// <param name="id">The ID of the puzzle instance.</param>
        /// <returns>The puzzle instance, or null if not found.</returns>
        Task<PuzzleInstance> GetByIdAsync(Guid id);

        /// <summary>
        /// Saves a puzzle instance.
        /// </summary>
        /// <param name="puzzle">The puzzle instance to save.</param>
        Task SaveAsync(PuzzleInstance puzzle);

        /// <summary>
        /// Deletes a puzzle instance by its ID.
        /// </summary>
        /// <param name="id">The ID of the puzzle instance to delete.</param>
        Task DeleteAsync(Guid id);
    }
}