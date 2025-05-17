using PatternCipher.Services.Contracts;
using System.Threading.Tasks;

namespace PatternCipher.Services.Interfaces
{
    /// <summary>
    /// Main interface for the Level Generation Orchestrator service.
    /// Defines the primary contract for requesting and orchestrating level generation.
    /// </summary>
    public interface ILevelGenerationService
    {
        /// <summary>
        /// Asynchronously generates a new level based on the provided request parameters.
        /// This method orchestrates the entire workflow including fetching difficulty,
        /// executing the generation pipeline, handling fallbacks, server validation,
        /// data migration, and persistence.
        /// </summary>
        /// <param name="request">The request parameters for level generation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated level data.</returns>
        Task<GeneratedLevelData> GenerateLevelAsync(LevelGenerationRequest request);
    }
}