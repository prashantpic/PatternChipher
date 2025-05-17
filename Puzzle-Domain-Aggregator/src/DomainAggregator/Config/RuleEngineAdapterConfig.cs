using System.Collections.Generic;

namespace PatternCipher.Domain.Config
{
    /// <summary>
    /// Configuration settings for adapting or interacting with the RuleEngine library.
    /// Holds configuration parameters for the RuleEngine integration, such as paths to rule files, 
    /// default rule sets, or RuleEngine specific settings.
    /// </summary>
    public class RuleEngineAdapterConfig
    {
        /// <summary>
        /// Gets or sets the list of file paths where rule definitions (e.g., JSON, XML) can be found.
        /// These paths might be relative to a base configuration directory or absolute.
        /// </summary>
        public List<string> RuleDefinitionFilePaths { get; set; }

        /// <summary>
        /// Gets or sets the name of the default rule set to be used if no specific rule set is requested.
        /// </summary>
        public string DefaultRuleSetName { get; set; }

        // Add any other RuleEngine specific settings here.
        // For example:
        // public bool EnableRuleCaching { get; set; } = true;
        // public int RuleCacheExpirationMinutes { get; set; } = 60;

        public RuleEngineAdapterConfig()
        {
            RuleDefinitionFilePaths = new List<string>();
            DefaultRuleSetName = "Default"; // sensible default
        }
    }
}