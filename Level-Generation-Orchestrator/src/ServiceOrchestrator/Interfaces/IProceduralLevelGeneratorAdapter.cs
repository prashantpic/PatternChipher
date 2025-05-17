using PatternCipher.Services.Contracts;
using System.Threading.Tasks;

namespace PatternCipher.Services.Interfaces
{
    /// <summary>
    /// Adapter interface for the ProceduralLevelGenerator from REPO-UNITY-CLIENT.
    /// Abstracts the actual ProceduralLevelGenerator component residing in REPO-UNITY-CLIENT,
    /// allowing this service to request raw level generation.
    /// </summary>
    public interface IProceduralLevelGeneratorAdapter
    {
        /// <summary>
        /// Asynchronously generates a raw level layout based on the provided generation parameters.
        /// </summary>
        /// <param name="generationParams">The parameters to guide the level generation process.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains an object representing the raw generated level data.
        /// The specific type of this object will depend on the underlying generator's implementation.
        /// </returns>
        Task<object> GenerateRawLevelAsync(LevelGenerationParameters generationParams);
    }
}