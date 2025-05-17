using PatternCipher.Services.Interfaces;
using PatternCipher.Services.Contracts;
using PatternCipher.Services.Configuration; // For RemoteConfigKeys
using PatternCipher.Services.Exceptions;
using System.Threading.Tasks;
using Newtonsoft.Json; // For deserializing config strings
using System; // For ArgumentNullException
using UnityEngine; // For Debug.Log

namespace PatternCipher.Services.Difficulty
{
    public class DifficultyProgressionManager : IDifficultyManager
    {
        private readonly IRemoteConfigServiceAdapter _remoteConfigServiceAdapter;

        public DifficultyProgressionManager(IRemoteConfigServiceAdapter remoteConfigServiceAdapter)
        {
            _remoteConfigServiceAdapter = remoteConfigServiceAdapter ?? throw new ArgumentNullException(nameof(remoteConfigServiceAdapter));
        }

        public async Task<DifficultyParameters> GetCurrentDifficultyParametersAsync(PlayerProgression playerProgression, string difficultyKey)
        {
            if (playerProgression == null) throw new ArgumentNullException(nameof(playerProgression));
            if (string.IsNullOrEmpty(difficultyKey)) throw new ArgumentNullException(nameof(difficultyKey));

            // Example: difficultyKey might be "Easy", "Medium_Tier1", etc.
            // Or it could be derived from playerProgression.CurrentLevel
            string configKey = $"{RemoteConfigKeys.DifficultyRulesPrefix}{difficultyKey}";
            Debug.Log($"[DifficultyProgressionManager] Fetching difficulty parameters from Remote Config key: {configKey}");

            try
            {
                string configJson = await _remoteConfigServiceAdapter.GetConfigAsync(configKey);
                if (string.IsNullOrEmpty(configJson) || configJson == "{}") // Check for empty or default JSON
                {
                    Debug.LogWarning($"[DifficultyProgressionManager] No difficulty parameters found or empty config for key: {configKey}. Using default.");
                    // Return default parameters or throw an exception
                    return new DifficultyParameters { Rating = difficultyKey, MaxSolverDepth = 50, SolverTimeoutMs = 3000 }; // Example default
                }
                
                DifficultyParameters parameters = JsonConvert.DeserializeObject<DifficultyParameters>(configJson);
                Debug.Log($"[DifficultyProgressionManager] Successfully fetched and parsed difficulty parameters for key: {configKey}");
                return parameters;
            }
            catch (JsonException ex)
            {
                Debug.LogError($"[DifficultyProgressionManager] Failed to parse difficulty parameters JSON for key {configKey}: {ex.Message}");
                throw new RemoteConfigFetchException($"Failed to parse Remote Config data for {configKey}.", ex);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DifficultyProgressionManager] Failed to fetch difficulty parameters for key {configKey}: {ex.Message}");
                throw new RemoteConfigFetchException($"Failed to fetch Remote Config data for {configKey}.", ex);
            }
        }

        public async Task<LevelGenerationParameters> GetGenerationParametersForDifficultyAsync(DifficultyParameters difficultyParams)
        {
            if (difficultyParams == null) throw new ArgumentNullException(nameof(difficultyParams));

            // Example: generation parameters might be directly part of DifficultyParameters from RemoteConfig,
            // or another RemoteConfig key is derived from difficultyParams.Rating
            string generationRulesKey = $"{RemoteConfigKeys.GenerationRulesPrefix}{difficultyParams.Rating}";
             Debug.Log($"[DifficultyProgressionManager] Fetching generation parameters from Remote Config key: {generationRulesKey}");
            
            try
            {
                string configJson = await _remoteConfigServiceAdapter.GetConfigAsync(generationRulesKey);
                 if (string.IsNullOrEmpty(configJson) || configJson == "{}")
                {
                    Debug.LogWarning($"[DifficultyProgressionManager] No generation parameters found or empty config for key: {generationRulesKey}. Using default.");
                    // Return default parameters or throw an exception
                    return new LevelGenerationParameters { GridMinSize = 5, GridMaxSize = 7, SymbolCount = 3 }; // Example default
                }

                LevelGenerationParameters parameters = JsonConvert.DeserializeObject<LevelGenerationParameters>(configJson);
                Debug.Log($"[DifficultyProgressionManager] Successfully fetched and parsed generation parameters for key: {generationRulesKey}");
                return parameters;
            }
            catch (JsonException ex)
            {
                Debug.LogError($"[DifficultyProgressionManager] Failed to parse generation parameters JSON for key {generationRulesKey}: {ex.Message}");
                throw new RemoteConfigFetchException($"Failed to parse Remote Config data for {generationRulesKey}.", ex);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DifficultyProgressionManager] Failed to fetch generation parameters for key {generationRulesKey}: {ex.Message}");
                throw new RemoteConfigFetchException($"Failed to fetch Remote Config data for {generationRulesKey}.", ex);
            }
        }
    }
}