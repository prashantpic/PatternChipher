using PatternCipher.Shared.Common;
using System;
using System.Collections.Generic;

namespace PatternCipher.Shared.Gameplay.Symbols
{
    /// <summary>
    /// Represents a unique symbol that a tile can display.
    /// Implemented as a Value Object where equality is based on the symbol's unique ID.
    /// </summary>
    public sealed class Symbol : ValueObject
    {
        public string Id { get; }

        public Symbol(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Symbol ID cannot be null or whitespace.", nameof(id));
            }
            Id = id;
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
        }
    }
}