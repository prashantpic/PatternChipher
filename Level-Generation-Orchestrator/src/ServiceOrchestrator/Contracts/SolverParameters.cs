namespace PatternCipher.Services.Contracts
{
    /// <summary>
    /// Data contract for parameters passed to the puzzle solver adapter.
    /// Represents parameters for the puzzle solver, such as search depth or timeout.
    /// </summary>
    public class SolverParameters
    {
        /// <summary>
        /// Maximum depth for the solver's search algorithm.
        /// </summary>
        public int MaxSearchDepth { get; set; }

        /// <summary>
        /// Timeout duration for the solving process, in milliseconds.
        /// </summary>
        public int TimeoutMilliseconds { get; set; }
    }
}