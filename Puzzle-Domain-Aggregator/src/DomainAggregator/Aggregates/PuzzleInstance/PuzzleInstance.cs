using System;
using System.Collections.Generic;
using System.Linq;
using PatternCipher.Domain.Entities;
using PatternCipher.Domain.ValueObjects;
using PatternCipher.Domain.Specifications; // Assumed to contain ISpecification and PlayerMoveContext
using PatternCipher.Domain.Exceptions;
using PatternCipher.Domain.DomainEvents;   // Assumed to contain event classes
using PatternCipher.Domain.Services;       // Assumed to contain RuleValidator

namespace PatternCipher.Domain.Aggregates.PuzzleInstance
{
    /// <summary>
    /// Aggregate root representing a single instance of a puzzle being played or defined.
    /// Encapsulates the state and behavior of a specific puzzle, including its grid, rules, 
    /// objective, and solution criteria (like par). Acts as a transactional boundary for puzzle operations.
    /// </summary>
    public class PuzzleInstance : IPuzzleInstance
    {
        private readonly PuzzleGrid _grid;
        private readonly List<RuleDefinition> _rules;
        private ParDetails _parDetails;
        private int _moveCount;
        private bool _isSolved;

        private readonly List<object> _domainEvents = new List<object>();

        private readonly RuleValidator _ruleValidator; // Assumed type from PatternCipher.Domain.Services
        private readonly SpecialTileInteractionService _specialTileService;
        private readonly ISpecification<PlayerMoveContext> _moveValidationSpec; // Assumed ISpecification and PlayerMoveContext

        public Guid Id { get; private set; }
        public PuzzleGrid Grid => _grid;
        public IEnumerable<RuleDefinition> Rules => _rules.AsReadOnly();
        public ParDetails ParDetails => _parDetails;
        public int MoveCount => _moveCount;
        public bool IsSolved => _isSolved; // Added based on SDS description of PuzzleInstance


        public PuzzleInstance(
            Guid id,
            PuzzleGrid initialGrid,
            IEnumerable<RuleDefinition> rules,
            ParDetails parDetails,
            RuleValidator ruleValidator, // Assumed type
            SpecialTileInteractionService specialTileService,
            ISpecification<PlayerMoveContext> moveValidationSpec) // Assumed types
        {
            Id = id;
            _grid = initialGrid ?? throw new ArgumentNullException(nameof(initialGrid));
            _rules = rules?.ToList() ?? throw new ArgumentNullException(nameof(rules));
            _parDetails = parDetails ?? throw new ArgumentNullException(nameof(parDetails));
            _ruleValidator = ruleValidator ?? throw new ArgumentNullException(nameof(ruleValidator));
            _specialTileService = specialTileService ?? throw new ArgumentNullException(nameof(specialTileService));
            _moveValidationSpec = moveValidationSpec ?? throw new ArgumentNullException(nameof(moveValidationSpec));
            _moveCount = 0;
            _isSolved = false; // Initial state
        }

        /// <summary>
        /// Applies a player's move to the puzzle.
        /// Validates the move, updates grid, increments move count, triggers special tile effects,
        /// checks for completion, and raises relevant domain events.
        /// </summary>
        /// <param name="move">The player move to apply.</param>
        public void ApplyMove(PlayerMove move)
        {
            if (_isSolved)
            {
                // Optionally throw an exception or ignore if puzzle already solved
                // throw new InvalidOperationException("Cannot apply move to an already solved puzzle.");
                return; 
            }

            var moveContext = new PlayerMoveContext(this, move); // Assumed PlayerMoveContext constructor
            if (!_moveValidationSpec.IsSatisfiedBy(moveContext))
            {
                throw new InvalidMoveException($"Move is not allowed: {move}");
            }

            _moveCount++;

            // Perform tile operations based on move type
            // This is a simplified example; actual logic depends on move.MoveType
            List<Tile> directlyAffectedTiles = new List<Tile>();
            if (move.MoveType == MoveType.Swap && move.SecondaryPosition.HasValue)
            {
                var tile1 = _grid.GetTile(move.PrimaryPosition);
                var tile2 = _grid.GetTile(move.SecondaryPosition.Value);
                if (tile1 != null) directlyAffectedTiles.Add(new Tile(tile1.Id, tile1.Position, tile1.Symbol, tile1.Type, tile1.State)); // snapshot before change
                if (tile2 != null) directlyAffectedTiles.Add(new Tile(tile2.Id, tile2.Position, tile2.Symbol, tile2.Type, tile2.State)); // snapshot before change
                
                _grid.SwapTiles(move.PrimaryPosition, move.SecondaryPosition.Value);
                
                 // update snapshot with new state if needed for event
                var swappedTile1 = _grid.GetTile(move.PrimaryPosition);
                var swappedTile2 = _grid.GetTile(move.SecondaryPosition.Value);
                if(swappedTile1 != null) directlyAffectedTiles.Add(swappedTile1);
                if(swappedTile2 != null) directlyAffectedTiles.Add(swappedTile2);

            }
            else if (move.MoveType == MoveType.Tap)
            {
                // Example: Tapping might change a tile's state or symbol
                var tile = _grid.GetTile(move.PrimaryPosition);
                if (tile != null)
                {
                    directlyAffectedTiles.Add(new Tile(tile.Id, tile.Position, tile.Symbol, tile.Type, tile.State)); // snapshot
                    // Example: TileState newState = tile.State.WithTapped(); // Assuming WithTapped method on TileState
                    // ModifyTileState(move.PrimaryPosition, newState);
                    // directlyAffectedTiles.Add(_grid.GetTile(move.PrimaryPosition));
                }
            }
            // Add other move types as necessary

            // Placeholder for RuleAppliedEvent based on the move itself
            // This would depend on rules related to MoveValidation scope
            // _domainEvents.Add(new RuleAppliedEvent(Id, "someMoveRuleId", true, "Move applied"));

            // Identify activated special tiles and apply effects
            // For simplicity, let's assume any tile involved in the move could be special
            var potentialTriggerPositions = new List<TilePosition> { move.PrimaryPosition };
            if (move.SecondaryPosition.HasValue)
            {
                potentialTriggerPositions.Add(move.SecondaryPosition.Value);
            }
            
            IEnumerable<Tile> specialEffectAffectedTiles = _specialTileService.ApplySpecialTileEffects(this, potentialTriggerPositions);

            // Combine all affected tiles and raise event
            var allAffectedTiles = directlyAffectedTiles.Union(specialEffectAffectedTiles).Distinct().ToList();
            if (allAffectedTiles.Any())
            {
                 // Assuming TileStateChangedEvent constructor takes affected tiles directly
                _domainEvents.Add(new TileStateChangedEvent(Id, allAffectedTiles));
            }

            CheckCompletion();
        }

        /// <summary>
        /// Checks if the puzzle has been completed according to its rules.
        /// Updates IsSolved state and raises PuzzleSolvedEvent if completed.
        /// </summary>
        public bool IsCompleted()
        {
            // Delegate to RuleValidator, which would use RuleDefinitions and current grid state.
            // RuleValidator.CheckCompletion is expected to handle the specific logic.
            if (!_isSolved && _ruleValidator.CheckCompletion(this)) // Assuming CheckCompletion returns bool
            {
                _isSolved = true;
                // Assuming PuzzleSolvedEvent constructor
                _domainEvents.Add(new PuzzleSolvedEvent(Id, _moveCount, _parDetails.ParMoveCount)); 
            }
            return _isSolved;
        }
        
        /// <summary>
        /// Internal method to modify a tile's state.
        /// Used by ApplyMove or SpecialTileInteractionService.
        /// </summary>
        internal void ModifyTileState(TilePosition position, TileState newState)
        {
            var tile = _grid.GetTile(position);
            if (tile != null)
            {
                // This assumes PuzzleGrid.ApplyStateChange updates the tile in its collection
                _grid.ApplyStateChange(position, newState);
            }
        }

        /// <summary>
        /// Internal method to swap tiles.
        /// Used by ApplyMove.
        /// </summary>
        internal void SwapTilesInternal(TilePosition pos1, TilePosition pos2)
        {
            _grid.SwapTiles(pos1, pos2);
        }

        /// <summary>
        /// Adds a rule definition to the puzzle instance.
        /// </summary>
        public void AddRule(RuleDefinition rule)
        {
            if (rule == null) throw new ArgumentNullException(nameof(rule));
            _rules.Add(rule);
        }

        /// <summary>
        /// Sets the par details for the puzzle.
        /// </summary>
        public void SetParDetails(ParDetails par)
        {
            _parDetails = par ?? throw new ArgumentNullException(nameof(par));
        }


        public IEnumerable<object> GetDomainEvents()
        {
            return _domainEvents.ToList(); // Return a copy
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        // Private helper method, if needed, for checking completion criteria
        private void CheckCompletion()
        {
            IsCompleted(); // This will call the logic including event raising
        }
    }
}