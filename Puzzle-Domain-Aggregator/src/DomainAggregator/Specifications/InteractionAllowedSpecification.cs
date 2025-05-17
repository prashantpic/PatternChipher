using System;
using PatternCipher.Domain.Aggregates.PuzzleInstance;
using PatternCipher.Domain.ValueObjects;
// Assuming Tile, PuzzleGrid entities are in PatternCipher.Domain.Entities

namespace PatternCipher.Domain.Specifications
{
    /// <summary>
    /// Context for evaluating if a player's move is allowed.
    /// </summary>
    public readonly struct PlayerMoveContext
    {
        public IPuzzleInstance Puzzle { get; } // Using IPuzzleInstance
        public PlayerMove Move { get; }

        public PlayerMoveContext(IPuzzleInstance puzzle, PlayerMove move)
        {
            Puzzle = puzzle ?? throw new ArgumentNullException(nameof(puzzle));
            Move = move; // PlayerMove is a struct, so direct assignment is fine
        }
    }

    /// <summary>
    /// Specification to check if a specific tile interaction is allowed under current rules.
    /// Encapsulates the logic to determine if a player's attempted move (e.g., swap, tap) is valid.
    /// </summary>
    public class InteractionAllowedSpecification : ISpecification<PlayerMoveContext>
    {
        // Dependencies can be injected here if complex rules require external services/data
        // For example: IGameRulesProvider gameRulesProvider;

        public InteractionAllowedSpecification(/* IGameRulesProvider gameRulesProvider */)
        {
            // this.gameRulesProvider = gameRulesProvider;
        }

        public bool IsSatisfiedBy(PlayerMoveContext context)
        {
            if (context.Puzzle == null) return false; // Or throw

            var puzzleInstance = context.Puzzle;
            var move = context.Move;

            // Basic boundary checks
            if (!puzzleInstance.Grid.IsPositionValid(move.PrimaryPosition))
                return false;
            if (move.MoveType == PlayerMove.MoveTypeEnum.Swap && 
                (!move.SecondaryPosition.HasValue || !puzzleInstance.Grid.IsPositionValid(move.SecondaryPosition.Value)))
                return false;

            // Tile existence and state checks
            var primaryTile = puzzleInstance.Grid.GetTile(move.PrimaryPosition);
            if (primaryTile == null || primaryTile.State.IsLocked) // Assuming TileState has IsLocked
                return false;

            if (move.MoveType == PlayerMove.MoveTypeEnum.Swap)
            {
                if (!move.SecondaryPosition.HasValue) return false; // Should be caught by constructor or validation of PlayerMove
                
                var secondaryTile = puzzleInstance.Grid.GetTile(move.SecondaryPosition.Value);
                if (secondaryTile == null || secondaryTile.State.IsLocked)
                    return false;

                // Adjacency check for swaps (example, simple Manhattan distance 1)
                int dx = Math.Abs(move.PrimaryPosition.X - move.SecondaryPosition.Value.X);
                int dy = Math.Abs(move.PrimaryPosition.Y - move.SecondaryPosition.Value.Y);
                if (!((dx == 1 && dy == 0) || (dx == 0 && dy == 1)))
                    return false; // Not adjacent
            }
            else if (move.MoveType == PlayerMove.MoveTypeEnum.Tap)
            {
                // Logic for tap eligibility, e.g., specific tile types or non-empty.
                // if (primaryTile.Type == TileType.NonTappable) return false; // Example
            }
            else
            {
                return false; // Unknown move type
            }

            // Further rule checks could be added here, potentially using gameRulesProvider
            // For example: gameRulesProvider.IsMoveTypeAllowedOnTile(move.MoveType, primaryTile.Type)

            return true; // If all checks pass
        }
    }
}