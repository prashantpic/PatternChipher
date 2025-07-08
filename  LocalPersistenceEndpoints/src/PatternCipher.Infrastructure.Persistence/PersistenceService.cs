using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PatternCipher.Infrastructure.Persistence.Exceptions;
using PatternCipher.Infrastructure.Persistence.Interfaces;
using PatternCipher.Infrastructure.Persistence.Migrations;
using PatternCipher.Infrastructure.Persistence.Models;
using PatternCipher.Infrastructure.Persistence.Repositories;
using PatternCipher.Infrastructure.Persistence.Security;
using PatternCipher.Infrastructure.Persistence.Serialization;

// Assuming a shared constants class
// using PatternCipher.Shared;

namespace PatternCipher.Infrastructure.Persistence
{
    /// <summary>
    /// Implements IPersistenceService. Orchestrates the complex process of loading and saving data
    /// by coordinating the file repository, data protector, serializer, and migration services.
    /// </summary>
    public class PersistenceService : IPersistenceService
    {
        private const string PLAYER_PROFILE_FILENAME = "player_profile.dat";
        private const string IN_PROGRESS_STATE_FILENAME = "gamestate.tmp";

        private readonly IFileRepository _repository;
        private readonly IDataProtector _dataProtector;
        private readonly IJsonSerializer _serializer;
        private readonly IMigrationService _migrationService;

        public PersistenceService(
            IFileRepository repository,
            IDataProtector dataProtector,
            IJsonSerializer serializer,
            IMigrationService migrationService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _dataProtector = dataProtector ?? throw new ArgumentNullException(nameof(dataProtector));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _migrationService = migrationService ?? throw new ArgumentNullException(nameof(migrationService));
        }

        public bool HasPlayerProfile()
        {
            return _repository.FileExists(PLAYER_PROFILE_FILENAME);
        }

        public async Task<PlayerProfile> LoadPlayerProfileAsync()
        {
            if (!HasPlayerProfile())
            {
                // No save file exists, return a fresh profile.
                return new PlayerProfile();
            }

            try
            {
                // 1. Read the raw, protected text from the file.
                var finalJson = await _repository.ReadAllTextAsync(PLAYER_PROFILE_FILENAME);

                // 2. Deserialize the outer wrapper object.
                var wrapper = _serializer.Deserialize<SaveDataWrapper>(finalJson);
                if (wrapper == null || string.IsNullOrEmpty(wrapper.Payload))
                {
                    throw new SaveDataCorruptionException("Save data wrapper is null or payload is empty.");
                }

                // 3. Unprotect the payload to get the clean, inner JSON. This also validates integrity.
                var playerProfileJson = _dataProtector.Unprotect(wrapper.Payload);

                // 4. Parse the clean JSON to a JObject for migration.
                var playerProfileJObject = JObject.Parse(playerProfileJson);

                // 5. Migrate the JObject to the current schema version.
                var migratedJObject = _migrationService.MigrateToCurrentVersion(playerProfileJObject);

                // 6. Deserialize the final, migrated JObject into the PlayerProfile model.
                // It's cleaner to convert the JObject back to a string for the final deserialization,
                // or use JObject.ToObject(Type), which the serializer can encapsulate.
                var playerProfile = migratedJObject.ToObject<PlayerProfile>() ?? new PlayerProfile();
                
                return playerProfile;
            }
            catch (Exception ex) when (ex is JsonException || ex is SaveDataCorruptionException || ex is InvalidOperationException)
            {
                // Log the specific error. In a real app, use a proper logging framework.
                Console.WriteLine($"[ERROR] Failed to load player profile: {ex.Message}. Returning a new profile.");
                // Optionally, back up the corrupted file here.
                return new PlayerProfile();
            }
        }

        public async Task SavePlayerProfileAsync(PlayerProfile playerProfile)
        {
            try
            {
                // 1. Serialize the core PlayerProfile object to a JSON string.
                var payloadJson = _serializer.Serialize(playerProfile);

                // 2. Protect the JSON payload. This obfuscates it and embeds an integrity check.
                var protectedPayload = _dataProtector.Protect(payloadJson);

                // 3. Create the wrapper DTO.
                var wrapper = new SaveDataWrapper
                {
                    SchemaVersion = SharedConstants.CURRENT_SCHEMA_VERSION,
                    Payload = protectedPayload,
                    Checksum = null // Not used by SimpleXorDataProtector, as checksum is embedded.
                };

                // 4. Serialize the final wrapper object.
                var finalJson = _serializer.Serialize(wrapper);

                // 5. Write the final string to the file.
                await _repository.WriteAllTextAsync(PLAYER_PROFILE_FILENAME, finalJson);
            }
            catch(Exception ex)
            {
                // In a real app, use a proper logging framework.
                 Console.WriteLine($"[ERROR] Failed to save player profile: {ex.Message}");
                 // Depending on the game, you might want to re-throw or handle this to notify the user.
            }
        }

        public bool HasInProgressState()
        {
            return _repository.FileExists(IN_PROGRESS_STATE_FILENAME);
        }

        public void SaveInProgressState(GameStateInProgress state)
        {
            try
            {
                var stateJson = _serializer.Serialize(state);
                // Note: WriteAllTextAsync is used here via .Wait() to fit the sync interface.
                // In a pure async context, this method would also be async.
                // For a game, synchronous fast saves are often acceptable.
                _repository.WriteAllTextAsync(IN_PROGRESS_STATE_FILENAME, stateJson).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to save in-progress state: {ex.Message}");
            }
        }

        public GameStateInProgress? LoadInProgressState()
        {
            if (!HasInProgressState()) return null;

            try
            {
                var stateJson = _repository.ReadAllTextAsync(IN_PROGRESS_STATE_FILENAME).GetAwaiter().GetResult();
                return _serializer.Deserialize<GameStateInProgress>(stateJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load in-progress state: {ex.Message}");
                // In case of corruption, clear the invalid state.
                ClearInProgressState();
                return null;
            }
        }

        public void ClearInProgressState()
        {
            _repository.DeleteFile(IN_PROGRESS_STATE_FILENAME);
        }
    }
}