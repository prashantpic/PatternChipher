using PatternCipher.Services.Interfaces;
using PatternCipher.Services.Contracts;
using PatternCipher.Services.Configuration;
using System.Threading.Tasks;
using UnityEngine; // For Debug.Log
using System;

namespace PatternCipher.Services.FallbackStrategies
{
    public class SimplifyParametersStrategy : IFallbackStrategy
    {
        private readonly OrchestratorSettings _orchestratorSettings;
        private readonly IGenerationPipeline _generationPipeline;
        private readonly IDifficultyManager _difficultyManager;

        public SimplifyParametersStrategy(
            OrchestratorSettings orchestratorSettings,
            IGenerationPipeline generationPipeline,
            IDifficultyManager difficultyManager)
        {
            _orchestratorSettings = orchestratorSettings ?? throw new ArgumentNullException(nameof(orchestratorSettings));
            _generationPipeline = generationPipeline ?? throw new ArgumentNullException(nameof(generationPipeline));
            _difficultyManager = difficultyManager ?? throw new ArgumentNullException(nameof(difficultyManager));
        }

        public async Task<GeneratedLevelData> AttemptFallbackGenerationAsync(LevelGenerationPipelineRequest originalPipelineRequest, int overallAttemptCount)
        {
            if (originalPipelineRequest == null) throw new ArgumentNullException(nameof(originalPipelineRequest));
            
            Debug.Log($"[SimplifyParametersStrategy] Attempting generation with simplified parameters. Overall attempt: {overallAttemptCount}");

            // Create a simplified version of difficulty/generation parameters
            // This is a placeholder for actual simplification logic.
            // It might involve asking IDifficultyManager for a "simpler" version based on original context,
            // or applying hardcoded simplifications.

            DifficultyParameters simplifiedDifficultyParams = originalPipelineRequest.DifficultyParameters; // Start with original
            LevelGenerationParameters simplifiedGenerationParams = originalPipelineRequest.GenerationParameters; // Start with original

            // Example simplification: Reduce grid size or complexity if possible
            // This requires knowledge of how LevelGenerationParameters are structured.
            // For instance, if GenerationParameters has MinGridSize, MaxGridSize:
            // simplifiedGenerationParams.MaxGridSize = Math.Max(simplifiedGenerationParams.MinGridSize, simplifiedGenerationParams.MaxGridSize -1);
            
            // A more robust way would be to have IDifficultyManager provide simplified params:
            // simplifiedDifficultyParams = await _difficultyManager.GetSimplifiedDifficultyParametersAsync(originalPipelineRequest.PlayerProgressionContext, originalPipelineRequest.DifficultyParameters);
            // simplifiedGenerationParams = await _difficultyManager.GetGenerationParametersForDifficultyAsync(simplifiedDifficultyParams);
            // For now, we'll just log that simplification would occur.
            Debug.Log("[SimplifyParametersStrategy] Placeholder: Applying simplification logic to parameters.");


            LevelGenerationPipelineRequest simplifiedRequest = new LevelGenerationPipelineRequest
            {
                DifficultyParameters = simplifiedDifficultyParams, // Potentially simplified
                GenerationParameters = simplifiedGenerationParams, // Potentially simplified
                PlayerProgressionContext = originalPipelineRequest.PlayerProgressionContext
            };
            
            try
            {
                GeneratedLevelData result = await _generationPipeline.ExecutePipelineAsync(simplifiedRequest);
                Debug.Log("[SimplifyParametersStrategy] Succeeded with simplified parameters.");
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[SimplifyParametersStrategy] Failed with simplified parameters: {ex.Message}");
                // Similar to RetryStrategy, re-throw to signal failure to LGS.
                throw;
            }
        }
    }
}