using System;
using System.Collections.Generic;
using PatternCipher.Domain.Enums;

namespace PatternCipher.Domain.Entities
{
    /// <summary>
    /// Entity representing a single game rule definition.
    /// Models a specific rule applicable to a puzzle, sourced from configuration or procedural generation.
    /// This could be a win condition, a constraint on moves, or a special tile behavior trigger.
    /// </summary>
    public class RuleDefinition
    {
        public Guid Id { get; }
        public RuleScope Scope { get; }
        public string Type { get; } // Corresponds to "RuleExpression (string or specific data structure for RuleEngine)"
                                    // or "Type" in the more direct plan.
        public IReadOnlyDictionary<string, object> Parameters { get; }
        public string DescriptionKey { get; }

        public RuleDefinition(Guid id, RuleScope scope, string type, Dictionary<string, object> parameters, string descriptionKey)
        {
            Id = id;
            Scope = scope;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Parameters = new Dictionary<string, object>(parameters ?? throw new ArgumentNullException(nameof(parameters)));
            DescriptionKey = descriptionKey ?? throw new ArgumentNullException(nameof(descriptionKey));
        }
    }
}