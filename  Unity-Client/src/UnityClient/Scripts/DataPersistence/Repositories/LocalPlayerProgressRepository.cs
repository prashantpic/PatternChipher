using System.Threading.Tasks;
using PatternCipher.Client.Domain.Repositories;
using PatternCipher.Client.DataPersistence.Models; // For PlayerProfileData
using PatternCipher.Client.DataPersistence.Services; // For SaveLoadService
// Assuming PlayerProgressAggregate is a class defined in the Domain layer
// using PatternCipher.Client.Domain.Aggregates; 

namespace PatternCipher.Client.DataPersistence.Repositories
{
    // Placeholder for PlayerProgressAggregate if not generated yet
    // This would ideally be in PatternCipher.Client.Domain.Aggregates
    public class PlayerProgressAggregate
    {
        public string UserId { get; set; }
        public int TotalStars { get; set; }
        // Add other relevant fields like unlocked levels, settings, etc.
        // This class should map to/from PlayerProfileData

        public PlayerProgressAggregate() { } // Default constructor for deserialization/creation

        // Example mapping from DTO
        public static PlayerProgressAggregate FromData(PlayerProfileData data)
        {
            if (data == null) return new PlayerProgressAggregate(); // Or null, depending on error handling
            return new PlayerProgressAggregate
            {
                UserId = data.UserId,
                TotalStars = data.TotalStars,
                // Map other fields from data.LevelRecords, data.Settings etc.
            };
        }

        // Example mapping to DTO
        public PlayerProfileData ToData()
        {
            return new PlayerProfileData
            {
                UserId = this.UserId,
                TotalStars = this.TotalStars,
                SchemaVersion = PlayerProfileData.CurrentSchemaVersion // Assuming PlayerProfileData has a current version
                // Map other fields to data.LevelRecords, data.Settings etc.
            };
        }
    }


    public class LocalPlayerProgressRepository : IPlayerProgressRepository
    {
        private readonly SaveLoadService _saveLoadService;
        private const string PlayerProgressFilename = "player_progress.dat";

        public LocalPlayerProgressRepository(SaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }

        public async Task SaveProgressAsync(PlayerProgressAggregate progress)
        {
            if (progress == null)
            {
                // Handle null progress data, perhaps log an error or skip saving
                UnityEngine.Debug.LogError("Attempted to save null player progress.");
                return;
            }

            PlayerProfileData dataToSave = progress.ToData(); // Convert aggregate to DTO
            await _saveLoadService.SaveAsync(dataToSave, PlayerProgressFilename);
        }

        public async Task<PlayerProgressAggregate> LoadProgressAsync()
        {
            if (!_saveLoadService.FileExists(PlayerProgressFilename))
            {
                UnityEngine.Debug.Log("No local player progress file found. Returning new progress.");
                return new PlayerProgressAggregate(); // Return a new/default aggregate
            }

            PlayerProfileData loadedData = await _saveLoadService.LoadAsync<PlayerProfileData>(PlayerProgressFilename);

            if (loadedData == null)
            {
                // Handle cases where loading failed or returned null (e.g., corrupted file, migration issue)
                UnityEngine.Debug.LogWarning("Failed to load player progress data or data was null. Returning new progress.");
                return new PlayerProgressAggregate(); // Return a new/default aggregate
            }
            
            return PlayerProgressAggregate.FromData(loadedData); // Convert DTO to aggregate
        }
    }
}