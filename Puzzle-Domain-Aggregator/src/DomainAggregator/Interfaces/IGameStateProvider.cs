using System.Threading.Tasks;
using PatternCipher.Models; // Assuming this namespace contains LocalGameState and CloudGameState

namespace PatternCipher.Domain.Interfaces
{
    /// <summary>
    /// Abstracts the retrieval of local and cloud game state data.
    /// </summary>
    public interface IGameStateProvider
    {
        /// <summary>
        /// Gets the current local game state snapshot.
        /// </summary>
        /// <returns>The local game state.</returns>
        LocalGameState GetLocalState();

        /// <summary>
        /// Gets the latest cloud game state snapshot.
        /// </summary>
        /// <returns>The cloud game state.</returns>
        Task<CloudGameState> GetCloudStateAsync();
    }
}