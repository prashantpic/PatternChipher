using System;
using PatternCipher.Domain.Aggregates.PuzzleInstance;
using PatternCipher.Domain.Interfaces;

namespace PatternCipher.Domain.Specifications
{
    /// <summary>
    /// Specification to check if a puzzle configuration is solvable.
    /// Encapsulates the logic to determine if a puzzle is solvable, often by querying a solver service.
    /// </summary>
    public class PuzzleSolvableSpecification : ISpecification<IPuzzleInstance> // Assuming IPuzzleInstance for broader applicability
    // Or use concrete PatternCipher.Domain.Aggregates.PuzzleInstance.PuzzleInstance if preferred by overall design
    {
        private readonly IPuzzleSolverAdapter _solverAdapter;

        public PuzzleSolvableSpecification(IPuzzleSolverAdapter solverAdapter)
        {
            _solverAdapter = solverAdapter ?? throw new ArgumentNullException(nameof(solverAdapter));
        }

        public bool IsSatisfiedBy(IPuzzleInstance puzzle)
        {
            if (puzzle == null)
            {
                throw new ArgumentNullException(nameof(puzzle));
            }
            
            // The IPuzzleSolverAdapter.IsSolvableAsync returns Task<bool>.
            // ISpecification.IsSatisfiedBy returns bool.
            // Blocking here is not ideal but follows the synchronous ISpecification contract.
            // Consider an IAsyncSpecification or making ISpecification async in a future iteration.
            // Also, the IPuzzleSolverAdapter takes PuzzleInstance concrete type.
            // This might require a cast or adjustment in IPuzzleSolverAdapter interface if IPuzzleInstance is used here.
            // For now, assuming PuzzleInstance is the type expected by the adapter and it is or can be cast from IPuzzleInstance.
            if (puzzle is PatternCipher.Domain.Aggregates.PuzzleInstance.PuzzleInstance concretePuzzle)
            {
                try
                {
                    return _solverAdapter.IsSolvableAsync(concretePuzzle).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    // Log exception, and decide on behavior (e.g., return false)
                    // For now, rethrow or let it propagate if that's the desired behavior for specification failures
                    // Or, a specification could return false on error.
                    Console.Error.WriteLine($"Error checking puzzle solvability: {ex.Message}");
                    return false; // Default to not solvable on error, or rethrow
                }
            }
            else
            {
                // Handle cases where the puzzle instance is not the expected concrete type, if necessary.
                // This might indicate a design mismatch or require a more flexible adapter.
                throw new InvalidOperationException("Puzzle instance is not of the expected type for solver adapter.");
            }
        }
    }
}