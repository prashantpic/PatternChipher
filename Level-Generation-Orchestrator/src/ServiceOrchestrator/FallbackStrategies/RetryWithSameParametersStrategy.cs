using PatternCipher.Services.Interfaces;
using PatternCipher.Services.Contracts;
using PatternCipher.Services.Configuration;
using System.Threading.Tasks;
using UnityEngine; // For Debug.Log
using System;

namespace PatternCipher.Services.FallbackStrategies
{
    public class RetryWithSameParametersStrategy : IFallbackStrategy
    {
        private readonly OrchestratorSettings _orchestratorSettings;
        private readonly IGenerationPipeline _generationPipeline;

        public RetryWithSameParametersStrategy(
            OrchestratorSettings orchestratorSettings,
            IGenerationPipeline generationPipeline)
        {
            _orchestratorSettings = orchestratorSettings ?? throw new ArgumentNullException(nameof(orchestratorSettings));
            _generationPipeline = generationPipeline ?? throw new ArgumentNullException(nameof(generationPipeline));
        }

        public async Task<GeneratedLevelData> AttemptFallbackGenerationAsync(LevelGenerationPipelineRequest originalPipelineRequest, int overallAttemptCount)
        {
            if (originalPipelineRequest == null) throw new ArgumentNullException(nameof(originalPipelineRequest));

            // This strategy might have its own internal retry count, or rely on the global attempt count.
            // For simplicity, let's assume it makes one attempt per call from LevelGenerationService's loop.
            // If it were to loop internally, OrchestratorSettings.MaxRetryAttempts would be its limit.

            // This specific strategy is more about being *one* of the fallbacks.
            // Let's assume it tries ONCE with same params when its turn comes.
            // If LevelGenerationService calls it multiple times, it's fine.
            // The name "RetryWithSameParametersStrategy" suggests it *is* the retry mechanism.
            // Let's make it retry up to _orchestratorSettings.MaxRetryAttempts if it's the *only* fallback strategy.
            // However, LevelGenerationService has its own loop. So, this strategy should just make *one* attempt.
            // The "overallAttemptCount" helps decide if this strategy should even run.

            Debug.Log($"[RetryWithSameParametersStrategy] Attempting generation with original parameters. Overall attempt: {overallAttemptCount}");

            try
            {
                // It re-uses the original request's parameters, which are already set up.
                GeneratedLevelData result = await _generationPipeline.ExecutePipelineAsync(originalPipelineRequest);
                Debug.Log("[RetryWithSameParametersStrategy] Succeeded with original parameters.");
                return result;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[RetryWithSameParametersStrategy] Failed with original parameters: {ex.Message}");
                // Don't re-throw here if LGS is to try other fallbacks. Return null to indicate failure of this strategy.
                // If this strategy is meant to throw to signal LGS to stop, then re-throw.
                // Given IFallbackStrategy[], returning null is safer.
                // LGS will throw LevelGenerationFailedException if all strategies (including this one) return null or throw.
                // For now, let's re-throw the specific exception type it caught from pipeline.
                throw; 
            }
        }
    }
}