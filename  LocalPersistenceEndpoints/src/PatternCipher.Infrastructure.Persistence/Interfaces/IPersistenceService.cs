using System.Threading.Tasks;
// Assuming these models exist in a shared project
// using PatternCipher.Shared.Models; 

// Placeholder classes until the shared project is available
public class PlayerProfile { }
public class GameStateInProgress { }


namespace PatternCipher.Infrastructure.Persistence.Interfaces
{
    /// <summary>
    /// Defines the public contract for all persistence operations.
    /// This is the primary interface used by the Application layer to save and load all player data,
    /// abstracting away the underlying implementation details of file I/O, serialization, and security.
    /// </summary>
    public interface IPersistenceService
    {
        /// <summary>
        /// Asynchronously loads, migrates, and deserializes the main player profile from local storage.
        /// </summary>
        /// <returns>
        /// A Task that represents the asynchronous operation. The task result contains the loaded PlayerProfile.
        /// If no profile exists or if the data is corrupt and unrecoverable, it must return a new, default PlayerProfile object.
        /// </returns>
        Task<PlayerProfile> LoadPlayerProfileAsync();

        /// <summary>
        /// Asynchronously serializes, protects, and saves the provided PlayerProfile object to local storage.
        /// </summary>
        /// <param name="playerProfile">The player profile data to save.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        Task SavePlayerProfileAsync(PlayerProfile playerProfile);

        /// <summary>
        /// Synchronously checks if a player profile save file exists.
        /// </summary>
        /// <returns>True if the file exists, otherwise false.</returns>
        bool HasPlayerProfile();

        /// <summary>
        /// Synchronously saves the state of an in-progress level for interruption recovery.
        /// This operation must be fast and is not expected to use the full protection/migration pipeline of the main profile save.
        /// </summary>
        /// <param name="state">The in-progress game state to save.</param>
        void SaveInProgressState(GameStateInProgress state);

        /// <summary>
        /// Synchronously loads the in-progress level state.
        /// </summary>
        /// <returns>The deserialized GameStateInProgress object, or null if no state exists or it's invalid.</returns>
        GameStateInProgress? LoadInProgressState();

        /// <summary>
        /// Deletes the in-progress level state file.
        /// </summary>
        void ClearInProgressState();

        /// <summary>
        /// Synchronously checks if an in-progress level state file exists.
        /// </summary>
        /// <returns>True if the file exists, otherwise false.</returns>
        bool HasInProgressState();
    }
}