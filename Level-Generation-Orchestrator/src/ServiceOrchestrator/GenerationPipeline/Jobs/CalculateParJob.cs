using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace PatternCipher.Services.GenerationPipeline.Jobs
{
    [BurstCompile]
    public struct CalculateParJob : IJob
    {
        // Input: Number of moves in the optimal or found solution
        [ReadOnly]
        public NativeArray<int> MovesInSolution;

        // Input: Other metrics from the solver or level that might influence par
        // [ReadOnly]
        // public NativeArray<float> SolverMetrics;

        // Output: The calculated par value
        public NativeArray<int> ParValueResult;

        public void Execute()
        {
            // Placeholder logic for par calculation.
            // This job applies game-specific rules to determine par based on solver output.
            
            if (MovesInSolution.Length > 0)
            {
                int solutionMoves = MovesInSolution[0];
                int calculatedPar = solutionMoves; // Simplest: par is moves in solution

                // Example of a more complex rule:
                // if (solutionMoves <= 5)
                // {
                //     calculatedPar = solutionMoves + 2;
                // }
                // else
                // {
                //     calculatedPar = solutionMoves + (int)(solutionMoves * 0.1f); // par is 10% more than solution
                // }
                
                ParValueResult[0] = calculatedPar;
            }
            else
            {
                ParValueResult[0] = -1; // Indicate error or undefined
            }

            // The IParCalculator would prepare the input (MovesInSolution)
            // from the SolvabilityResult, schedule this job, and retrieve the ParValueResult.
        }
    }
}