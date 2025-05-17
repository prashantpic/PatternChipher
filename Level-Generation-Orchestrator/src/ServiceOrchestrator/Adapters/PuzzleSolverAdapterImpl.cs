using PatternCipher.Services.Interfaces;
using PatternCipher.Services.Contracts; // For SolvabilityResult, SolverParameters
using System.Threading.Tasks;
using System; // For ArgumentNullException
using UnityEngine; // For Debug.Log

// Assuming PatternCipher.Client and its components exist as per instructions
namespace PatternCipher.Client
{
    // Placeholder for the actual solver from REPO-UNITY-CLIENT
    public class PuzzleSolver
    {
        public Task<SolvabilityResult> SolvePuzzleAsync(object rawLevelData, SolverParameters parameters)
        {
            Debug.Log($"[Client.PuzzleSolver.Stub] Solving puzzle. MaxDepth={parameters.MaxDepth}, Timeout={parameters.TimeoutMilliseconds}ms");
            // Simulate solving
            bool isSolvable = UnityEngine.Random.value > 0.1f; // 90% solvable for stub
            return Task.FromResult(new SolvabilityResult
            {
                IsSolvable = isSolvable,
                SolutionPathData = isSolvable ? "solution_path_placeholder" : null,
                MovesInSolution = isSolvable ? UnityEngine.Random.Range(5, 20) : 0,
                SolverMetrics = "metrics_placeholder",
                FailureReason = isSolvable ? null : "StubSolver_UnsolvableRoll"
            });
        }
    }
}

namespace PatternCipher.Services.Adapters
{
    public class PuzzleSolverAdapterImpl : IPuzzleSolverAdapter
    {
        private readonly Client.PuzzleSolver _solver; // As specified: "PuzzleSolver (from REPO-UNITY-CLIENT)"

        public PuzzleSolverAdapterImpl(Client.PuzzleSolver solver)
        {
            _solver = solver ?? throw new ArgumentNullException(nameof(solver));
        }

        public async Task<SolvabilityResult> SolveAsync(object rawLevelData, SolverParameters solverParameters)
        {
            if (rawLevelData == null) throw new ArgumentNullException(nameof(rawLevelData));
            if (solverParameters == null) throw new ArgumentNullException(nameof(solverParameters));

            Debug.Log("[PuzzleSolverAdapterImpl] Calling external PuzzleSolver.SolvePuzzleAsync");
            try
            {
                // Assuming the client solver has a method like SolvePuzzleAsync
                SolvabilityResult result = await _solver.SolvePuzzleAsync(rawLevelData, solverParameters);
                Debug.Log($"[PuzzleSolverAdapterImpl] Puzzle solved. Solvable: {result.IsSolvable}");
                return result;
            }
            catch(Exception ex)
            {
                Debug.LogError($"[PuzzleSolverAdapterImpl] Error calling external solver: {ex.Message}");
                throw; // Or wrap in a service-specific exception
            }
        }
    }
}