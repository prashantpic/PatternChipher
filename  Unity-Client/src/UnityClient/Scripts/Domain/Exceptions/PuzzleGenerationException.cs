using System;

namespace PatternCipher.Client.Domain.Exceptions
{
    public class PuzzleGenerationException : Exception
    {
        public PuzzleGenerationException()
            : base("An error occurred during puzzle generation.")
        {
        }

        public PuzzleGenerationException(string message)
            : base(message)
        {
        }

        public PuzzleGenerationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}