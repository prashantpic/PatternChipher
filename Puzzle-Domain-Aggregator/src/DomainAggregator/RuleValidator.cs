using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PatternCipher.Domain.Aggregates.PuzzleInstance;
using PatternCipher.Domain.Entities;
using PatternCipher.Domain.Interfaces;
using PatternCipher.Domain.Specifications;
using PatternCipher.Models; // For ValidationResult
// Assuming RuleEngine is the correct namespace for the RuleEngine library
using RuleEngine = global::RuleEngine.RuleEngine; // Alias to avoid conflict if local RuleEngine type exists

namespace PatternCipher.Domain.Services
{
    /// <summary>
    /// Domain service responsible for validating puzzle rules, tile interactions, and overall puzzle integrity.
    /// Enforces business rules for tile interactions and puzzle integrity using the RuleEngine library and defined specifications.
    /// </summary>
    public class RuleValidator
    {
        private readonly IPuzzleSolverAdapter _solverAdapter;
        private readonly RuleApplicabilitySpecification _ruleApplicabilitySpec; // Assuming this is injected or constructed
        private readonly RuleEngine _ruleEngine; // The actual RuleEngine instance

        public RuleValidator(
            IPuzzleSolverAdapter solverAdapter,
            RuleApplicabilitySpecification ruleApplicabilitySpec,
            RuleEngine ruleEngine)
        {
            _solverAdapter = solverAdapter ?? throw new ArgumentNullException(nameof(solverAdapter));
            _ruleApplicabilitySpec = ruleApplicabilitySpec ?? throw new ArgumentNullException(nameof(ruleApplicabilitySpec));
            _ruleEngine = ruleEngine ?? throw new ArgumentNullException(nameof(ruleEngine));
        }

        /// <summary>
        /// Validates the overall integrity of a puzzle instance.
        /// This may include checking solvability and adherence to fundamental construction rules.
        /// </summary>
        /// <param name="puzzle">The puzzle instance to validate.</param>
        /// <returns>A ValidationResult indicating if the puzzle is valid and any issues found.</returns>
        public async Task<ValidationResult> ValidatePuzzleIntegrityAsync(PuzzleInstance puzzle)
        {
            if (puzzle == null) throw new ArgumentNullException(nameof(puzzle));

            var issues = new List<string>();
            bool isSolvable = await _solverAdapter.IsSolvableAsync(puzzle);

            if (!isSolvable)
            {
                issues.Add("Puzzle is not solvable.");
            }

            // Example: Using a PuzzleSolvableSpecification that encapsulates the solver call
            // var solvableSpec = new PuzzleSolvableSpecification(_solverAdapter); // This spec might be injected too
            // if (!solvableSpec.IsSatisfiedBy(puzzle))
            // {
            //     issues.Add("Puzzle failed solvability specification.");
            // }

            // Additional integrity checks can be added here, potentially using other specifications
            // or direct logic. For example, checking if the grid has a valid configuration.

            return new ValidationResult
            {
                IsValid = !issues.Any(),
                Issues = issues
            };
        }

        /// <summary>
        /// Checks if a specific rule is satisfied for the given puzzle instance.
        /// </summary>
        /// <param name="rule">The rule definition to check.</param>
        /// <param name="puzzle">The current puzzle instance state.</param>
        /// <returns>True if the rule is applicable and satisfied, false otherwise.</returns>
        public bool IsRuleSatisfied(RuleDefinition rule, PuzzleInstance puzzle)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            if (puzzle == null) throw new ArgumentNullException(nameof(puzzle));

            var ruleContext = new RuleContext(puzzle, rule); // Assuming RuleContext is defined
            if (!_ruleApplicabilitySpec.IsSatisfiedBy(ruleContext))
            {
                return false; // Rule is not applicable in the current context
            }

            // Placeholder for RuleEngine evaluation.
            // The actual inputs and rule format will depend on the RuleEngine library's API
            // and how RuleDefinition.RuleExpression (or similar) is structured.
            // For example, rules might be defined in JSON or a DSL.
            // Inputs would be derived from the 'puzzle' state.
            var inputs = new[] { puzzle, puzzle.Grid, puzzle.Rules /* ... other relevant parts of puzzle state */ };
            
            // This is a conceptual representation. The actual API of RuleEngine 4.2.1 needs to be used.
            // List<CompiledRule> compiledRules = _ruleEngine.CompileRule(rule.RuleExpression); // Or however rules are prepared
            // var result = _ruleEngine.Execute(compiledRules, inputs);
            // return result.IsSuccess; // Or interpret result based on rule type

            // For now, returning a placeholder.
            // The 'rule.Parameters' and 'rule.Type' would guide how to use the RuleEngine.
            // Example: if rule.Type is "MatchCount" and Parameters["TargetSymbol"] and Parameters["TargetCount"]
            // The RuleEngine would evaluate if current puzzle state meets this.
            Console.WriteLine($"Evaluating rule '{rule.Id}' of type '{rule.Type}'. Implementation pending RuleEngine integration.");
            return true; // Placeholder
        }

        /// <summary>
        /// Checks if the puzzle's completion/win conditions are met.
        /// </summary>
        /// <param name="puzzle">The puzzle instance to check.</param>
        /// <returns>True if the puzzle is completed, false otherwise.</returns>
        public bool CheckCompletion(PuzzleInstance puzzle)
        {
            if (puzzle == null) throw new ArgumentNullException(nameof(puzzle));

            // Iterate through rules that define completion criteria (e.g., Scope = RuleScope.LevelCompletion)
            var completionRules = puzzle.Rules.Where(r => r.Scope == Domain.Enums.RuleScope.LevelCompletion);

            if (!completionRules.Any())
            {
                // No completion rules defined, perhaps a different mechanism or default state?
                // For now, consider it not completed if no rules define completion.
                return false;
            }

            foreach (var rule in completionRules)
            {
                if (!IsRuleSatisfied(rule, puzzle))
                {
                    return false; // If any completion rule is not met, puzzle is not completed.
                }
            }

            return true; // All completion rules are satisfied.
        }
    }
}