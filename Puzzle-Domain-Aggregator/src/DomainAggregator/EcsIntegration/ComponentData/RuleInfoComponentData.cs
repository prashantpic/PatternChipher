using System;

namespace PatternCipher.Domain.EcsIntegration.ComponentData
{
    /// <summary>
    /// Data-only struct representing rule information for potential ECS integration.
    /// </summary>
    public readonly struct RuleInfoComponentData
    {
        public Guid RuleId { get; }
        public bool IsSatisfied { get; }
        public string DescriptionKey { get; } // For localization

        public RuleInfoComponentData(Guid ruleId, bool isSatisfied, string descriptionKey)
        {
            RuleId = ruleId;
            IsSatisfied = isSatisfied;
            DescriptionKey = descriptionKey ?? string.Empty;
        }
    }
}