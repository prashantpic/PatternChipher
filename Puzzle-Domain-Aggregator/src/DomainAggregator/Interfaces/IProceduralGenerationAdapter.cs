using System.Threading.Tasks;
using PatternCipher.Models; // Assuming this namespace contains GeneratedPuzzleData and GenerationParameters

namespace PatternCipher.Domain.Interfaces
{
    /// <summary>
    /// Abstracts communication with the procedural level generation logic.
    /// Implementation likely in REPO-UNITY-CLIENT.
    /// </summary>
    public interface IProceduralGenerationAdapter
    {
        /// <summary>
        /// Generates a new puzzle based on the provided parameters.
        /// </summary>
        /// <param name="parameters">Parameters for puzzle generation.</param>
        /// <returns>Data structure representing the generated puzzle.</returns>
        Task<GeneratedPuzzleData> GeneratePuzzleAsync(GenerationParameters parameters);
    }
}