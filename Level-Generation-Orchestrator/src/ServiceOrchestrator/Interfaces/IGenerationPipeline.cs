using PatternCipher.Services.Contracts;
using System.Threading.Tasks;

namespace PatternCipher.Services.Interfaces
{
    /// <summary>
    /// Interface for the level generation pipeline.
    /// Defines the contract for executing a level generation pipeline, which includes steps
    /// for layout generation, solvability verification, and par calculation.
    /// </summary>
    public interface IGenerationPipeline
    {
        /// <summary>
        /// Asynchronously executes the level generation pipeline.
        /// This involves generating the raw level layout, verifying its solvability,
        /// and calculating its par value.
        /// </summary>
        /// <param name="pipelineRequest">The request parameters for the generation pipeline execution.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the generated level data if successful.</returns>
        Task<GeneratedLevelData> ExecutePipelineAsync(LevelGenerationPipelineRequest pipelineRequest);
    }
}