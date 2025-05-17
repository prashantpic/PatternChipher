using System.Threading.Tasks;
using PatternCipher.Client.Domain.Repositories; // For IPlayerProgressRepository
using PatternCipher.Client.Domain.Aggregates; // For PlayerProgressAggregate
using PatternCipher.Client.DataPersistence.Models; // For PlayerProfileData
using PatternCipher.Client.DataPersistence.Services; // For SaveLoadService (interface or class)
using UnityEngine; // For Debug.Log

namespace PatternCipher.Client.DataPersistence.Repositories
{
    public class LocalPlayerProgressRepository : IPlayerProgressRepository
    {
        private readonly ISaveLoadService _saveLoadService;
        private const string PlayerProgressFileName = "playerProgress.dat"; // Example filename

        public LocalPlayerProgressRepository(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            if (_saveLoadService == null)
            {
                Debug.LogError("SaveLoadService is null in LocalPlayerProgressRepository constructor.");
            }
        }

        public async Task SaveProgressAsync(PlayerProgressAggregate progress)
        {
            if (progress == null)
            {
                Debug.LogError("PlayerProgressAggregate to save is null.");
                return;
            }
            if (_saveLoadService == null)
            {
                Debug.LogError("SaveLoadService not initialized. Cannot save progress.");
                return;
            }

            // Convert PlayerProgressAggregate to PlayerProfileData DTO
            PlayerProfileData dataToSave = MapAggregateToData(progress);

            try
            {
                await _saveLoadService.SaveAsync(dataToSave, PlayerProgressFileName);
                Debug.Log($"Player progress saved locally to {PlayerProgressFileName}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save player progress: {e.Message}");
                // Potentially re-throw or handle more gracefully
            }
        }

        public async Task<PlayerProgressAggregate> LoadProgressAsync()
        {
            if (_saveLoadService == null)
            {
                Debug.LogError("SaveLoadService not initialized. Cannot load progress.");
                return PlayerProgressAggregate.CreateDefault(); // Return a default/new aggregate
            }

            try
            {
                if (!await _saveLoadService.FileExistsAsync(PlayerProgressFileName))
                {
                    Debug.Log($"No local player progress file found ({PlayerProgressFileName}). Returning new progress.");
                    return PlayerProgressAggregate.CreateDefault(); // Or return null if preferred
                }

                PlayerProfileData loadedData = await _saveLoadService.LoadAsync<PlayerProfileData>(PlayerProgressFileName);

                if (loadedData != null)
                {
                    // Convert PlayerProfileData DTO to PlayerProgressAggregate
                    Debug.Log($"Player progress loaded from {PlayerProgressFileName}");
                    return MapDataToAggregate(loadedData);
                }
                else
                {
                    Debug.LogWarning($"Loaded player progress data is null from {PlayerProgressFileName}. Returning new progress.");
                    return PlayerProgressAggregate.CreateDefault();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load player progress: {e.Message}. Returning new progress.");
                // Potentially delete corrupted file or handle more gracefully
                // await _saveLoadService.DeleteAsync(PlayerProgressFileName);
                return PlayerProgressAggregate.CreateDefault();
            }
        }

        // --- Mappers ---
        // These would ideally be in separate Mapper classes or handled by a library like AutoMapper
        // For simplicity, defined inline here.

        private PlayerProfileData MapAggregateToData(PlayerProgressAggregate aggregate)
        {
            if (aggregate == null) return null;
            // This mapping depends heavily on the structure of PlayerProgressAggregate and PlayerProfileData
            return new PlayerProfileData
            {
                SchemaVersion = PlayerProfileData.CurrentSchemaVersion, // Assuming PlayerProfileData has this
                UserId = aggregate.UserId,
                TotalStars = aggregate.TotalStars,
                // Example: LevelRecords mapping
                // LevelRecords = aggregate.GetLevelRecords().ToDictionary(
                //    kvp => kvp.Key, 
                //    kvp => new LevelRecord { Stars = kvp.Value.Stars, BestScore = kvp.Value.BestScore }
                // ),
                // Settings = new GameSettings { Volume = aggregate.Settings.Volume, /* ... */ }
            };
        }

        private PlayerProgressAggregate MapDataToAggregate(PlayerProfileData data)
        {
            if (data == null) return PlayerProgressAggregate.CreateDefault();
            // This mapping also depends on the structures
            // Ensure to handle schema version migration here or ensure SaveLoadService does it.
            // For this example, assume SaveLoadService has already migrated 'data' to current schema.

            // var settings = new Domain.ValueObjects.PlayerSettings(data.Settings.Volume, /* ... */);
            // var aggregate = new PlayerProgressAggregate(data.UserId, data.TotalStars, settings);
            
            // Example: Hydrating LevelRecords
            // if (data.LevelRecords != null)
            // {
            //     foreach (var kvp in data.LevelRecords)
            //     {
            //         aggregate.UpdateLevelRecord(kvp.Key, new Domain.Entities.PlayerLevelRecord(kvp.Value.Stars, kvp.Value.BestScore));
            //     }
            // }
            // For now, returning a simplified aggregate:
             return new PlayerProgressAggregate(data.UserId, data.TotalStars /*, other mapped fields */);
        }
    }

    // STUB for ISaveLoadService - this should be in DataPersistence.Services
    public interface ISaveLoadService
    {
        Task SaveAsync<T>(T data, string filename);
        Task<T> LoadAsync<T>(string filename);
        Task<bool> FileExistsAsync(string filename);
        Task DeleteAsync(string filename);
    }
}