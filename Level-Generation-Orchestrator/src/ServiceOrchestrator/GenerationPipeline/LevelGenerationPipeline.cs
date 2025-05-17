using PatternCipher.Services.Interfaces;
using PatternCipher.Services.Contracts;
using PatternCipher.Services.Configuration;
using PatternCipher.Services.Exceptions;
using PatternCipher.Services.Utilities;
using PatternCipher.Services.VersionManagement.Models; // For CurrentLevelData
using System.Threading.Tasks;
using UnityEngine; // For Debug.Log
using System;

namespace PatternCipher.Services.GenerationPipeline
{
    public class LevelGenerationPipeline : IGenerationPipeline
    {
        private readonly IProceduralLevelGeneratorAdapter _proceduralGeneratorAdapter;
        private readonly ISolverCoordinator _solverCoordinator;
        private readonly IParCalculator _parCalculator;
        private readonly OrchestratorSettings _orchestratorSettings;
        // private readonly BurstDispatchUtility _burstDispatchUtility; // Dependency specified but not used in this async-adapter flow

        public LevelGenerationPipeline(
            IProceduralLevelGeneratorAdapter proceduralGeneratorAdapter,
            ISolverCoordinator solverCoordinator,
            IParCalculator parCalculator,
            OrchestratorSettings orchestratorSettings,
            BurstDispatchUtility burstDispatchUtility) // Keep BurstDispatchUtility if future job integration is direct
        {
            _proceduralGeneratorAdapter = proceduralGeneratorAdapter ?? throw new ArgumentNullException(nameof(proceduralGeneratorAdapter));
            _solverCoordinator = solverCoordinator ?? throw new ArgumentNullException(nameof(solverCoordinator));
            _parCalculator = parCalculator ?? throw new ArgumentNullException(nameof(parCalculator));
            _orchestratorSettings = orchestratorSettings ?? throw new ArgumentNullException(nameof(orchestratorSettings));
            // _burstDispatchUtility = burstDispatchUtility; // Not directly used if adapters handle their own potential job dispatches
        }

        public async Task<GeneratedLevelData> ExecutePipelineAsync(LevelGenerationPipelineRequest pipelineRequest)
        {
            if (pipelineRequest == null) throw new ArgumentNullException(nameof(pipelineRequest));

            Debug.Log("[LevelGenerationPipeline] Starting pipeline execution.");

            // 1. Generate Raw Level Layout
            Debug.Log("[LevelGenerationPipeline] Generating raw level layout.");
            object rawLayoutData;
            try
            {
                rawLayoutData = await _proceduralGeneratorAdapter.GenerateRawLevelAsync(pipelineRequest.GenerationParameters);
                if (rawLayoutData == null)
                {
                    throw new Exception("Procedural level generator returned null layout data.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LevelGenerationPipeline] Failed to generate raw level layout: {ex.Message}");
                // Depending on policy, this could be a more specific exception
                throw new Exception("Failed during raw level generation.", ex);
            }
            Debug.Log("[LevelGenerationPipeline] Raw level layout generated.");

            // 2. Verify Solvability
            Debug.Log("[LevelGenerationPipeline] Verifying solvability.");
            SolvabilityResult solvabilityResult;
            try
            {
                // Note: The `rawLayoutData` is passed directly. If jobs are used,
                // the adapter/coordinator or this pipeline would marshal data into NativeArrays.
                // For this design, we assume adapters manage their internal job use.
                solvabilityResult = await _solverCoordinator.VerifySolvabilityAsync(rawLayoutData, pipelineRequest.DifficultyParameters);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LevelGenerationPipeline] Error during solvability verification: {ex.Message}");
                throw; // Rethrow specific or wrapped exception
            }

            if (!solvabilityResult.IsSolvable)
            {
                Debug.LogWarning("[LevelGenerationPipeline] Level is not solvable.");
                throw new SolvabilityCheckFailedException("Generated level is not solvable.", solvabilityResult.FailureReason);
            }
            Debug.Log("[LevelGenerationPipeline] Level is solvable.");

            // 3. Calculate Par
            Debug.Log("[LevelGenerationPipeline] Calculating par value.");
            int parValue;
            try
            {
                parValue = await _parCalculator.CalculateParAsync(rawLayoutData, solvabilityResult); // Pass rawLayoutData if calculator needs it
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LevelGenerationPipeline] Error during par calculation: {ex.Message}");
                throw new ParCalculationFailedException("Failed to calculate par value.", ex);
            }
            Debug.Log($"[LevelGenerationPipeline] Par value calculated: {parValue}");

            // 4. Construct GeneratedLevelData
            var generatedLevelData = new GeneratedLevelData(
                levelId: Guid.NewGuid().ToString(),
                rawLayoutData: rawLayoutData,
                solutionPath: solvabilityResult.SolutionPathData, // Assuming SolutionPathData is string or suitable type
                parValue: parValue,
                difficultyRating: pipelineRequest.DifficultyParameters?.Rating ?? "Default", // Example
                version: CurrentLevelData.LatestVersion, // Use the constant for the latest storable version
                isSolvable: true,
                generationTimestamp: DateTime.UtcNow
            )
            {
                // Additional properties can be set here if GDL is a class and they are not in constructor
                // e.g., solverMetrics if they are part of GeneratedLevelData DTO
            };
            
            Debug.Log("[LevelGenerationPipeline] Pipeline execution completed successfully.");
            return generatedLevelData;
        }
    }
}