using PatternCipher.Services.Contracts;
using System.Threading.Tasks;

namespace PatternCipher.Services.Interfaces
{
    /// <summary>
    /// Interface for managing difficulty progression rules.
    /// Defines the contract for applying difficulty progression rules based on player progress
    /// and remote configurations to influence level generation parameters.
    /// </summary>
    public interface IDifficultyManager
    {
        /// <summary>
        /// Asynchronously gets the current difficulty parameters based on player progression.
        /// </summary>
        /// <param name="playerProgression">The player's current progression state.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the <see cref="DifficultyParameters"/> appropriate for the player's context.
        /// </returns>
        Task<DifficultyParameters> GetCurrentDifficultyParametersAsync(PlayerProgression playerProgression);

        /// <summary>
        /// Asynchronously gets the specific level generation parameters for a given set of difficulty parameters.
        /// This translates abstract difficulty settings into concrete generator inputs.
        /// </summary>
        /// <param name="difficultyParams">The difficulty parameters to translate.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the <see cref="LevelGenerationParameters"/> to be used by the generator.
        /// </returns>
        Task<LevelGenerationParameters> GetGenerationParametersForDifficultyAsync(DifficultyParameters difficultyParams);
    }
}