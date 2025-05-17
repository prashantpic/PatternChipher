using PatternCipher.Client.Infrastructure.Firebase;
using PatternCipher.Client.Domain.Repositories;
using PatternCipher.Client.DataPersistence.Models;
using System.Threading.Tasks;
using UnityEngine; // For Debug.Log, can be replaced with a proper logger

namespace PatternCipher.Client.DataPersistence.Services
{
    public class CloudSyncService
    {
        private readonly FirebaseAuthenticationService _authService;
        private readonly FirebaseFirestoreService _firestoreService;
        private readonly IPlayerProgressRepository _localProgressRepository; // For local data access

        private const string PLAYER_DATA_COLLECTION = "playerProfiles"; // Firestore collection name

        public CloudSyncService(
            FirebaseAuthenticationService authService,
            FirebaseFirestoreService firestoreService,
            IPlayerProgressRepository localProgressRepository)
        {
            _authService = authService;
            _firestoreService = firestoreService;
            _localProgressRepository = localProgressRepository;
        }

        public async Task SyncAsync()
        {
            if (!_authService.IsSignedIn())
            {
                Debug.LogWarning("CloudSyncService: User not signed in. Cannot sync.");
                return;
            }

            string userId = _authService.GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                Debug.LogError("CloudSyncService: Signed in user has no ID. Cannot sync.");
                return;
            }

            Debug.Log("CloudSyncService: Starting sync process...");

            PlayerProfileData localData = await _localProgressRepository.LoadProgressAsync(); // Assumes PlayerProgressAggregate can be converted or repository returns DTO
            PlayerProfileData cloudData = await DownloadDataAsync();

            if (cloudData == null && localData != null) // No cloud data, local data exists -> Upload local
            {
                Debug.Log("CloudSyncService: No cloud data found. Uploading local data.");
                await UploadDataAsync(localData);
            }
            else if (cloudData != null && localData == null) // Cloud data exists, no local data -> Download cloud
            {
                Debug.Log("CloudSyncService: No local data found. Applying cloud data.");
                // Assuming PlayerProgressAggregate can be created from PlayerProfileData
                // var aggregate = ConvertToAggregate(cloudData); // Placeholder conversion
                // await _localProgressRepository.SaveProgressAsync(aggregate);
                await SaveLocalDataAsync(cloudData);
            }
            else if (cloudData != null && localData != null) // Both exist -> Resolve conflict
            {
                Debug.Log("CloudSyncService: Both local and cloud data exist. Resolving conflict.");
                PlayerProfileData resolvedData = ResolveConflict(localData, cloudData);
                await SaveLocalDataAsync(resolvedData); // Save resolved to local
                await UploadDataAsync(resolvedData);    // Upload resolved to cloud
            }
            else // Neither exists
            {
                Debug.Log("CloudSyncService: No local or cloud data to sync.");
                 // Optionally create and save a new default profile
                var defaultProfile = PlayerProfileData.CreateDefault(userId);
                await SaveLocalDataAsync(defaultProfile);
                await UploadDataAsync(defaultProfile);
            }
            Debug.Log("CloudSyncService: Sync process completed.");
        }

        public async Task UploadDataAsync(PlayerProfileData data)
        {
            if (!_authService.IsSignedIn() || data == null)
            {
                Debug.LogWarning("CloudSyncService: User not signed in or data is null. Cannot upload.");
                return;
            }
            string userId = _authService.GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return;

            data.UserId = userId; // Ensure UserId is set
            data.LastCloudSyncTimestamp = System.DateTime.UtcNow.Ticks; // Update sync timestamp

            try
            {
                string documentPath = $"{PLAYER_DATA_COLLECTION}/{userId}";
                await _firestoreService.SetDocumentAsync(documentPath, data);
                Debug.Log($"CloudSyncService: Data uploaded successfully for user {userId}.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"CloudSyncService: Error uploading data - {ex.Message}");
            }
        }

        public async Task<PlayerProfileData> DownloadDataAsync()
        {
            if (!_authService.IsSignedIn())
            {
                Debug.LogWarning("CloudSyncService: User not signed in. Cannot download.");
                return null;
            }
            string userId = _authService.GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return null;

            try
            {
                string documentPath = $"{PLAYER_DATA_COLLECTION}/{userId}";
                var data = await _firestoreService.GetDocumentAsync<PlayerProfileData>(documentPath);
                if(data != null) Debug.Log($"CloudSyncService: Data downloaded successfully for user {userId}.");
                else Debug.Log($"CloudSyncService: No cloud data found for user {userId}.");
                return data;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"CloudSyncService: Error downloading data - {ex.Message}");
                return null;
            }
        }

        private PlayerProfileData ResolveConflict(PlayerProfileData local, PlayerProfileData cloud)
        {
            // Basic conflict resolution: prefer data with the most recent 'SchemaVersion',
            // then 'LastModifiedTimestamp' or 'LastCloudSyncTimestamp'.
            // More sophisticated merging logic might be needed based on game design
            // (e.g., merge unlocked levels, take highest score per level).

            Debug.Log($"CloudSyncService: Resolving conflict. Local schema: {local.SchemaVersion}, Cloud schema: {cloud.SchemaVersion}");
            Debug.Log($"CloudSyncService: Local last modified: {new System.DateTime(local.LastModifiedTimestamp)}, Cloud last cloud sync: {new System.DateTime(cloud.LastCloudSyncTimestamp)}");


            if (local.SchemaVersion > cloud.SchemaVersion) return local;
            if (cloud.SchemaVersion > local.SchemaVersion) return cloud;

            // If schemas are same, compare timestamps.
            // Here, LastModifiedTimestamp refers to local changes, LastCloudSyncTimestamp to when cloud was last updated.
            // A more robust approach would be a dedicated "server_timestamp" field updated by Firestore on write.
            // For simplicity, we'll use LastModifiedTimestamp for local and LastCloudSyncTimestamp for cloud as a proxy.
            if (local.LastModifiedTimestamp > cloud.LastCloudSyncTimestamp)
            {
                Debug.Log("CloudSyncService: Conflict resolved favoring local data (newer modification).");
                return local;
            }
            else
            {
                Debug.Log("CloudSyncService: Conflict resolved favoring cloud data (newer or equal cloud sync).");
                return cloud;
            }
        }
        
        // Helper to save DTO to local repository (might involve conversion to aggregate)
        private async Task SaveLocalDataAsync(PlayerProfileData dataToSave)
        {
            if (dataToSave == null) return;
            // This assumes IPlayerProgressRepository can handle PlayerProfileData directly
            // or that PlayerProfileData can be easily converted to the aggregate it expects.
            // For now, let's assume SaveProgressAsync can take a DTO, or we have a conversion step.
            
            // Placeholder for converting DTO to Aggregate if repository expects an aggregate
            // PlayerProgressAggregate aggregateToSave = ConvertDtoToAggregate(dataToSave);
            // await _localProgressRepository.SaveProgressAsync(aggregateToSave);

            // If IPlayerProgressRepository is designed to work with DTOs for simplicity in this layer:
             await _localProgressRepository.SaveProgressAsync(dataToSave); // This is a simplification.
                                                                        // In a strict DDD, repository saves aggregates.
                                                                        // Application service would convert DTO -> Aggregate.
            Debug.Log("CloudSyncService: Resolved data saved locally.");
        }
    }
}