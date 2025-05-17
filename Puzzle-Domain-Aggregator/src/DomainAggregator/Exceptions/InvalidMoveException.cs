using System;

namespace PatternCipher.Domain.Exceptions
{
    /// <summary>
    /// Custom exception for attempts to make an invalid player move.
    /// Indicates that a player attempted a move that violates game rules.
    /// Thrown by PuzzleInstance when a PlayerMove fails validation against InteractionAllowedSpecification or other rules.
    /// </summary>
    [Serializable]
    public class InvalidMoveException : Exception
    {
        public InvalidMoveException() { }
        public InvalidMoveException(string message) : base(message) { }
        public InvalidMoveException(string message, Exception inner) : base(message, inner) { }
        protected InvalidMoveException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}