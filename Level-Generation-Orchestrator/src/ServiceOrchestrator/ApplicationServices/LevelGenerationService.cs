using PatternCipher.Services.Interfaces;
using PatternCipher.Services.Contracts;
using PatternCipher.Services.Configuration;
using PatternCipher.Services.Exceptions;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace PatternCipher.Services.ApplicationServices
{
    public class LevelGenerationService : ILevelGenerationService
    {
        private readonly IGenerationPipeline _generationPipeline;
        private readonly IEnumerable<IFallbackStrategy> _fallbackStrategies;
        private readonly IFirebaseValidationServiceAdapter _firebaseValidationServiceAdapter;
        private readonly ILevelDataMigrator _levelDataMigrator;
        private readonly IDifficultyManager _difficultyManager;
        private readonly ICloudSaveRepositoryAdapter _cloudSaveRepositoryAdapter;
        private readonly OrchestratorSettings _orchestratorSettings;

        public LevelGenerationService(
            IGenerationPipeline generationPipeline,
            IEnumerable<IFallbackStrategy> fallbackStrategies,
            IFirebaseValidationServiceAdapter firebaseValidationServiceAdapter,
            ILevelDataMigrator levelDataMigrator,
            IDifficultyManager difficultyManager,
            ICloudSaveRepositoryAdapter cloudSaveRepositoryAdapter,
            OrchestratorSettings orchestratorSettings)
        {
            _generationPipeline = generationPipeline ?? throw new ArgumentNullException(nameof(generationPipeline));
            _fallbackStrategies = fallbackStrategies ?? throw new ArgumentNullException(nameof(fallbackStrategies));
            _firebaseValidationServiceAdapter = firebaseValidationServiceAdapter ?? throw new ArgumentNullException(nameof(firebaseValidationServiceAdapter));
            _levelDataMigrator = levelDataMigrator ?? throw new ArgumentNullException(nameof(levelDataMigrator));
            _difficultyManager = difficultyManager ?? throw new ArgumentNullException(nameof(difficultyManager));
            _cloudSaveRepositoryAdapter = cloudSaveRepositoryAdapter ?? throw new ArgumentNullException(nameof(cloudSaveRepositoryAdapter));
            _orchestratorSettings = orchestratorSettings ?? throw new ArgumentNullException(nameof(orchestratorSettings));
        }

        public async Task<GeneratedLevelData> GenerateLevelAsync(LevelGenerationRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            Debug.Log($"[LevelGenerationService] Starting level generation for request: {request.DifficultyKey}");

            DifficultyParameters difficultyParams = await _difficultyManager.GetCurrentDifficultyParametersAsync(request.PlayerProgressionContext, request.DifficultyKey);
            LevelGenerationParameters generationParams = await _difficultyManager.GetGenerationParametersForDifficultyAsync(difficultyParams);

            GeneratedLevelData generatedLevelData = null;
            int currentAttempt = 0;
            int maxAttempts = 1 + (_fallbackStrategies.Any() ? _orchestratorSettings.MaxRetryAttempts : 0); // Initial + Fallbacks configured retries

            LevelGenerationPipelineRequest pipelineRequest = new LevelGenerationPipelineRequest
            {
                DifficultyParameters = difficultyParams,
                GenerationParameters = generationParams,
                PlayerProgressionContext = request.PlayerProgressionContext
            };

            // Initial Attempt
            try
            {
                Debug.Log($"[LevelGenerationService] Attempt {++currentAttempt}/{maxAttempts} (Primary Pipeline)");
                generatedLevelData = await _generationPipeline.ExecutePipelineAsync(pipelineRequest);
                Debug.Log($"[LevelGenerationService] Primary pipeline attempt {currentAttempt} successful.");
            }
            catch (Exception ex) when (ex is SolvabilityCheckFailedException || ex is ParCalculationFailedException)
            {
                Debug.LogWarning($"[LevelGenerationService] Primary pipeline attempt {currentAttempt} failed: {ex.Message}");
                if (currentAttempt >= maxAttempts || !_fallbackStrategies.Any())
                {
                    Debug.LogError($"[LevelGenerationService] All generation attempts failed. Last error: {ex.Message}");
                    throw new LevelGenerationFailedException("All level generation attempts failed.", ex);
                }
            }
            catch (Exception ex)
            {
                 Debug.LogError($"[LevelGenerationService] Unexpected error during primary pipeline attempt {currentAttempt}: {ex.Message}");
                 throw new LevelGenerationFailedException("Unexpected error during primary level generation.", ex);
            }


            // Fallback Attempts
            if (generatedLevelData == null && _fallbackStrategies.Any())
            {
                foreach (var strategy in _fallbackStrategies)
                {
                    if (currentAttempt >= maxAttempts) break;
                    
                    Debug.Log($"[LevelGenerationService] Attempt {++currentAttempt}/{maxAttempts} (Fallback: {strategy.GetType().Name})");
                    try
                    {
                        generatedLevelData = await strategy.AttemptFallbackGenerationAsync(pipelineRequest, currentAttempt); // Pass pipelineRequest
                        if (generatedLevelData != null)
                        {
                            Debug.Log($"[LevelGenerationService] Fallback strategy {strategy.GetType().Name} successful.");
                            break; 
                        }
                        Debug.LogWarning($"[LevelGenerationService] Fallback strategy {strategy.GetType().Name} did not yield a level.");
                    }
                    catch (Exception ex) when (ex is SolvabilityCheckFailedException || ex is ParCalculationFailedException)
                    {
                        Debug.LogWarning($"[LevelGenerationService] Fallback strategy {strategy.GetType().Name} failed: {ex.Message}");
                         if (currentAttempt >= maxAttempts)
                        {
                            Debug.LogError($"[LevelGenerationService] All generation attempts failed including fallbacks. Last error: {ex.Message}");
                            throw new LevelGenerationFailedException("All level generation attempts failed, including fallbacks.", ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[LevelGenerationService] Unexpected error during fallback {strategy.GetType().Name}: {ex.Message}");
                        throw new LevelGenerationFailedException($"Unexpected error during fallback strategy {strategy.GetType().Name}.", ex);
                    }
                }
            }

            if (generatedLevelData == null)
            {
                Debug.LogError("[LevelGenerationService] All generation attempts exhausted without success.");
                throw new LevelGenerationFailedException("Failed to generate a level after all primary and fallback attempts.");
            }

            Debug.Log("[LevelGenerationService] Level generated successfully, proceeding to migration.");

            // Migration
            // The ILevelDataMigrator returns a shell with RawLayoutData and Version. We update the existing GDL.
            var migrationShell = await _levelDataMigrator.MigrateToLatestAsync(generatedLevelData.RawLayoutData, generatedLevelData.Version);
            generatedLevelData.RawLayoutData = migrationShell.RawLayoutData;
            generatedLevelData.Version = migrationShell.Version;
            Debug.Log($"[LevelGenerationService] Level data migrated to version {generatedLevelData.Version}.");

            // Server-side Validation
            LevelValidationRequest validationRequest = new LevelValidationRequest
            {
                GeneratedLevel = generatedLevelData, // Send the fully populated and migrated level
                DifficultyKey = request.DifficultyKey
            };
            Debug.Log("[LevelGenerationService] Performing server-side validation.");
            LevelValidationResult validationResult = await _firebaseValidationServiceAdapter.ValidateLevelOnServerAsync(validationRequest);
            if (!validationResult.IsValid)
            {
                Debug.LogError($"[LevelGenerationService] Server-side validation failed: {validationResult.Reason}");
                throw new ServerValidationFailedException($"Server-side validation failed: {validationResult.Reason}");
            }
            Debug.Log("[LevelGenerationService] Server-side validation successful.");

            // Persist Data
            Debug.Log("[LevelGenerationService] Storing generated level.");
            await _cloudSaveRepositoryAdapter.StoreGeneratedLevelAsync(generatedLevelData);
            Debug.Log("[LevelGenerationService] Level stored successfully.");

            return generatedLevelData;
        }
    }
}