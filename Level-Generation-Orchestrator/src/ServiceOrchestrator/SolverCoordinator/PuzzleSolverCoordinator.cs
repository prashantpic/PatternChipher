using PatternCipher.Services.Interfaces;
using PatternCipher.Services.Contracts;
using System.Threading.Tasks;
using System; // For ArgumentNullException

namespace PatternCipher.Services.SolverCoordinator
{
    public class PuzzleSolverCoordinator : ISolverCoordinator
    {
        private readonly IPuzzleSolverAdapter _puzzleSolverAdapter;

        public PuzzleSolverCoordinator(IPuzzleSolverAdapter puzzleSolverAdapter)
        {
            _puzzleSolverAdapter = puzzleSolverAdapter ?? throw new ArgumentNullException(nameof(puzzleSolverAdapter));
        }

        public async Task<SolvabilityResult> VerifySolvabilityAsync(object rawLevelData, DifficultyParameters difficultyParameters)
        {
            if (rawLevelData == null) throw new ArgumentNullException(nameof(rawLevelData));
            // difficultyParameters can be null if not applicable for a specific solver call

            // Potentially prepare SolverParameters from DifficultyParameters
            var solverParameters = new SolverParameters
            {
                MaxDepth = difficultyParameters?.MaxSolverDepth ?? 100, // Example mapping
                TimeoutMilliseconds = difficultyParameters?.SolverTimeoutMs ?? 5000 // Example mapping
            };
            
            // TODO: Add any transformation logic for rawLevelData if needed before passing to the adapter

            return await _puzzleSolverAdapter.SolveAsync(rawLevelData, solverParameters);
        }
    }
}