using PatternCipher.Services.Contracts;
using System.Threading.Tasks;

namespace PatternCipher.Services.Interfaces
{
    /// <summary>
    /// Adapter interface for fetching configurations from Remote Config (likely via REPO-UNITY-CLIENT).
    /// Abstracts fetching remote configuration values, particularly difficulty progression rules
    /// and generation parameters.
    /// </summary>
    public interface IRemoteConfigServiceAdapter
    {
        /// <summary>
        /// Asynchronously fetches difficulty parameters from remote configuration.
        /// </summary>
        /// <param name="difficultyKey">The key identifying the specific difficulty parameters to fetch.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the <see cref="DifficultyParameters"/> fetched from remote config.
        /// </returns>
        Task<DifficultyParameters> GetDifficultyParametersAsync(string difficultyKey);

        /// <summary>
        /// Asynchronously fetches level generation rules from remote configuration.
        /// </summary>
        /// <param name="ruleSetKey">The key identifying the specific set of generation rules to fetch.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the <see cref="LevelGenerationParameters"/> (or a similar structure representing rules)
        /// fetched from remote config.
        /// </returns>
        Task<LevelGenerationParameters> GetGenerationRulesAsync(string ruleSetKey);
    }
}