using PatternCipher.Services.Interfaces;
using PatternCipher.Services.Contracts; // For GeneratedLevelData
using System.Threading.Tasks;
using System; // For ArgumentNullException
using UnityEngine; // For Debug.Log

// Assuming PatternCipher.Client.Data and its components exist as per instructions
namespace PatternCipher.Client.Data
{
    // Placeholder for the actual cloud save repository
    public interface ICloudSaveRepository 
    {
        Task StoreLevelAsync(GeneratedLevelData levelData);
    }

    public class CloudSaveRepositoryStub : ICloudSaveRepository // Example Stub
    {
        public Task StoreLevelAsync(GeneratedLevelData levelData)
        {
            Debug.Log($"[Client.Data.ICloudSaveRepository.Stub] Storing level ID: {levelData.LevelID}, Version: {levelData.Version}");
            // Simulate storage
            return Task.CompletedTask;
        }
    }
}


namespace PatternCipher.Services.Adapters
{
    public class CloudSaveRepositoryAdapterImpl : ICloudSaveRepositoryAdapter
    {
        private readonly Client.Data.ICloudSaveRepository _cloudSaveRepository; // As specified

        public CloudSaveRepositoryAdapterImpl(Client.Data.ICloudSaveRepository cloudSaveRepository)
        {
            _cloudSaveRepository = cloudSaveRepository ?? throw new ArgumentNullException(nameof(cloudSaveRepository));
        }

        public async Task StoreGeneratedLevelAsync(GeneratedLevelData generatedLevelData)
        {
            if (generatedLevelData == null) throw new ArgumentNullException(nameof(generatedLevelData));

            Debug.Log("[CloudSaveRepositoryAdapterImpl] Calling external ICloudSaveRepository.StoreLevelAsync");
            try
            {
                // Assuming the client repository has a method like StoreLevelAsync
                await _cloudSaveRepository.StoreLevelAsync(generatedLevelData);
                Debug.Log("[CloudSaveRepositoryAdapterImpl] Level data stored via external repository.");
            }
            catch(Exception ex)
            {
                Debug.LogError($"[CloudSaveRepositoryAdapterImpl] Error calling external cloud save repository: {ex.Message}");
                throw; // Or wrap in a service-specific exception (e.g., DataPersistenceException)
            }
        }
    }
}