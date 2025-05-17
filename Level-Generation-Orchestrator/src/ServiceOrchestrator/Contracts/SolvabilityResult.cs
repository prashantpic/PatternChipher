namespace PatternCipher.Services.Contracts
{
    /// <summary>
    /// Data contract for the result of a solvability check.
    /// Contains information about whether a level is solvable, the solution path if found,
    /// and any metrics from the solver.
    /// </summary>
    public class SolvabilityResult
    {
        /// <summary>
        /// True if the level is solvable, false otherwise.
        /// </summary>
        public bool IsSolvable { get; set; }

        /// <summary>
        /// Data representing the solution path (e.g., a JSON string of moves).
        /// Null or empty if not solvable or not applicable.
        /// </summary>
        public string SolutionPathData { get; set; }

        /// <summary>
        /// The number of moves in the found solution.
        /// 0 or a specific value if not solvable.
        /// </summary>
        public int MovesInSolution { get; set; }

        /// <summary>
        /// Additional metrics from the solver (e.g., time taken, nodes explored).
        /// The specific structure depends on the solver.
        /// </summary>
        public object SolverMetrics { get; set; } // Could be a more specific type or Dictionary<string, object>
    }
}