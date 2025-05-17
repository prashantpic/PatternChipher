using System;
using PatternCipher.Domain.Aggregates.PuzzleInstance;
using PatternCipher.Domain.Entities;

namespace PatternCipher.Domain.Specifications
{
    /// <summary>
    /// Context for evaluating if a rule is applicable.
    /// </summary>
    public readonly struct RuleContext
    {
        public IPuzzleInstance Puzzle { get; } // Using IPuzzleInstance
        public RuleDefinition Rule { get; }

        public RuleContext(IPuzzleInstance puzzle, RuleDefinition rule)
        {
            Puzzle = puzzle ?? throw new ArgumentNullException(nameof(puzzle));
            Rule = rule ?? throw new ArgumentNullException(nameof(rule));
        }
    }

    /// <summary>
    /// Specification to check if a rule is applicable in the current puzzle context.
    /// Encapsulates the logic to determine if a specific RuleDefinition should be evaluated or applied.
    /// </summary>
    public class RuleApplicabilitySpecification : ISpecification<RuleContext>
    {
        // Dependencies can be injected here if complex applicability checks are needed
        // For example: IGameStateQueryService gameStateQueryService;

        public RuleApplicabilitySpecification(/* IGameStateQueryService gameStateQueryService */)
        {
            // this.gameStateQueryService = gameStateQueryService;
        }

        public bool IsSatisfiedBy(RuleContext context)
        {
            if (context.Puzzle == null || context.Rule == null)
            {
                // Or throw ArgumentNullException if context or its members being null is invalid state
                return false; 
            }

            var puzzleInstance = context.Puzzle;
            var rule = context.Rule;

            // Example: Check if rule scope matches a general puzzle state or type
            // This is a placeholder logic. Real logic would depend on RuleDefinition.Scope, RuleDefinition.Type,
            // RuleDefinition.Parameters and the current state of puzzleInstance.

            // For example, a rule might only apply after a certain number of moves
            // if (rule.Parameters.TryGetValue("MinMoves", out var minMovesObj) && minMovesObj is int minMoves)
            // {
            //     if (puzzleInstance.MoveCount < minMoves) return false;
            // }

            // Another example: rule only applies to specific puzzle types (if PuzzleInstance has a Type property)
            // if (rule.Parameters.TryGetValue("ApplicablePuzzleType", out var puzzleTypeObj) && puzzleTypeObj is string requiredType)
            // {
            //     if (puzzleInstance.Type != requiredType) return false; // Assuming PuzzleInstance has a Type
            // }

            // By default, assume applicable if no specific counter-conditions are met.
            // More sophisticated logic would inspect rule.Type, rule.Scope, and rule.Parameters
            // in conjunction with puzzleInstance state.
            
            // This is a very basic check. In a real system, you'd parse rule.Type or rule.Parameters
            // to determine the conditions for applicability.
            switch (rule.Scope)
            {
                case Enums.RuleScope.LevelCompletion:
                    // Always applicable for checking, evaluation happens elsewhere.
                    return true; 
                case Enums.RuleScope.MoveValidation:
                    // Usually, move validation rules are checked directly during move attempt.
                    // This spec might check if a *general* move validation rule is active.
                    return true; 
                // Add more cases based on Enums.RuleScope
                default:
                    return true; // Or false, depending on default stance
            }
        }
    }
}