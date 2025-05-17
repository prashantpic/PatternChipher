using PatternCipher.Services.Contracts;
using System.Threading.Tasks;

namespace PatternCipher.Services.Interfaces
{
    /// <summary>
    /// Interface for fallback strategies during level generation.
    /// Defines a contract for fallback generation strategies to be invoked
    /// when primary generation attempts fail.
    /// </summary>
    public interface IFallbackStrategy
    {
        /// <summary>
        /// Asynchronously attempts to generate a level using a fallback strategy.
        /// </summary>
        /// <param name="originalRequest">The original request that led to the need for a fallback.</param>
        /// <param name="attemptCount">The current attempt number for this fallback strategy or overall retries.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the <see cref="GeneratedLevelData"/> if the fallback was successful,
        /// or null/throws an exception if this strategy also failed or is not applicable.
        /// </returns>
        Task<GeneratedLevelData> AttemptFallbackGenerationAsync(LevelGenerationRequest originalRequest, int attemptCount);
    }
}