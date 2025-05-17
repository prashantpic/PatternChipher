using System;

namespace PatternCipher.Client.Domain.Exceptions
{
    public class InvalidGridOperationException : Exception
    {
        public InvalidGridOperationException()
            : base("An invalid operation was attempted on the game grid.")
        {
        }

        public InvalidGridOperationException(string message)
            : base(message)
        {
        }

        public InvalidGridOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}