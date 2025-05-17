using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PatternCipher.Domain.Aggregates.PuzzleInstance;
using PatternCipher.Domain.Interfaces;
using PatternCipher.Domain.ValueObjects;
using PatternCipher.Domain.Entities; // Required for ParDetails, PuzzleGrid, RuleDefinition, Tile
using PatternCipher.Domain.DomainEvents;
using PatternCipher.Domain.Exceptions;
using PatternCipher.Domain.Specifications; // Required for ISpecification<PlayerMoveContext>
using PatternCipher.Models; // For GenerationParameters, GeneratedPuzzleData, ValidationResult

namespace PatternCipher.Domain.Services
{
    /// <summary>
    /// Central domain service for orchestrating puzzle operations.
    /// Coordinates interactions between generation, solving, special tile mechanics, and rule enforcement.
    /// Acts as a primary entry point for commands related to puzzle setup, gameplay, and state management.
    /// </summary>
    public class PuzzleOrchestrator
    {
        private readonly IProceduralGenerationAdapter _generationAdapter;
        private readonly IPuzzleInstanceRepository _puzzleRepository;
        private readonly RuleValidator _ruleValidator;
        private readonly SpecialTileInteractionService _specialTileService;
        private readonly ParDeterminationService _parDeterminationService;
        private readonly RuleSetManagementService _ruleSetManagementService;
        private readonly ISpecification<PlayerMoveContext> _moveValidationSpec; // For PuzzleInstance creation

        // private readonly IEventPublisher _eventPublisher; // For publishing domain events

        public PuzzleOrchestrator(
            IProceduralGenerationAdapter generationAdapter,
            IPuzzleInstanceRepository puzzleRepository,
            RuleValidator ruleValidator,
            SpecialTileInteractionService specialTileService,
            ParDeterminationService parDeterminationService,
            RuleSetManagementService ruleSetManagementService,
            ISpecification<PlayerMoveContext> moveValidationSpec
            /*, IEventPublisher eventPublisher */)
        {
            _generationAdapter = generationAdapter ?? throw new ArgumentNullException(nameof(generationAdapter));
            _puzzleRepository = puzzleRepository ?? throw new ArgumentNullException(nameof(puzzleRepository));
            _ruleValidator = ruleValidator ?? throw new ArgumentNullException(nameof(ruleValidator));
            _specialTileService = specialTileService ?? throw new ArgumentNullException(nameof(specialTileService));
            _parDeterminationService = parDeterminationService ?? throw new ArgumentNullException(nameof(parDeterminationService));
            _ruleSetManagementService = ruleSetManagementService ?? throw new ArgumentNullException(nameof(ruleSetManagementService));
            _moveValidationSpec = moveValidationSpec ?? throw new ArgumentNullException(nameof(moveValidationSpec));
            // _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        }

        /// <summary>
        /// Creates a new puzzle, orchestrating generation, rule loading, par determination,
        /// initial validation, saving, and publishing relevant domain events.
        /// </summary>
        /// <param name="parameters">Parameters for puzzle generation.</param>
        /// <returns>The ID of the newly created puzzle instance.</returns>
        public async Task<Guid> CreateNewPuzzleAsync(GenerationParameters parameters)
        {
            GeneratedPuzzleData generatedData = await _generationAdapter.GeneratePuzzleAsync(parameters);

            // TODO: Map GeneratedPuzzleData (e.g., generatedData.RawGridData, generatedData.RawRulesData)
            // to domain entities: PuzzleGrid and initial List<RuleDefinition>.
            // This mapping logic might reside in a factory or helper class.
            var initialGrid = new PuzzleGrid(new GridDimensions(10, 10), new List<Tile>()); // Placeholder
            var generatedRules = new List<RuleDefinition>(); // Placeholder

            Guid puzzleId = Guid.NewGuid();

            // Load active rule set (e.g., from Firebase Remote Config via RuleSetManagementService)
            // The RuleSetVersion might come from parameters or be the 'latest'.
            // await _ruleSetManagementService.LoadRuleSetAsync(new RuleSetVersion("1.0")); // Example
            IEnumerable<RuleDefinition> activeRules = _ruleSetManagementService.GetActiveRuleSet();
            
            // Combine/merge generated rules with active rules if necessary
            var allRules = generatedRules.Concat(activeRules).Distinct().ToList(); // Example merging strategy

            // Create a preliminary puzzle instance (or structure) to determine par
            // ParDetails might be determined before full PuzzleInstance construction or after.
            // For now, let's assume ParDetails can be set later or a temporary instance is used.
            var tempParDetails = new ParDetails(0,0); // Placeholder before actual calculation

            var puzzleInstance = new PuzzleInstance(
                puzzleId,
                initialGrid,
                allRules,
                tempParDetails, // Temporary ParDetails
                _ruleValidator,
                _specialTileService,
                _moveValidationSpec
            );

            // Determine Par for the puzzle
            ParDetails parDetails = await _parDeterminationService.DetermineParAsync(puzzleInstance);
            puzzleInstance.SetParDetails(parDetails); // Assuming PuzzleInstance has a method to set/update ParDetails

            // Initial validation of the constructed puzzle
            ValidationResult integrityResult = await _ruleValidator.ValidatePuzzleIntegrityAsync(puzzleInstance);
            if (!integrityResult.IsValid)
            {
                // Handle invalid puzzle generation (e.g., log, throw specific exception)
                throw new PuzzleIntegrityException($"Generated puzzle '{puzzleId}' failed integrity check: {string.Join(", ", integrityResult.Issues)}");
            }

            await _puzzleRepository.SaveAsync(puzzleInstance);

            var initializedEvent = new PuzzleInitializedEvent(
                puzzleInstance.Id,
                puzzleInstance.Grid.Dimensions,
                puzzleInstance.Grid.GetAllTiles() // Assuming PuzzleGrid has GetAllTiles()
            );
            // _eventPublisher.Publish(initializedEvent);
            Console.WriteLine($"PuzzleInitializedEvent: PuzzleId={initializedEvent.PuzzleId}");

            return puzzleInstance.Id;
        }

        /// <summary>
        /// Applies a player's move to a puzzle instance.
        /// Orchestrates loading the puzzle, applying the move via PuzzleInstance,
        /// handling exceptions, processing domain events, and saving the puzzle.
        /// </summary>
        /// <param name="puzzleId">The ID of the puzzle instance.</param>
        /// <param name="move">The player move to apply.</param>
        public async Task ApplyPlayerMoveAsync(Guid puzzleId, PlayerMove move)
        {
            PuzzleInstance puzzleInstance = await _puzzleRepository.GetByIdAsync(puzzleId);
            if (puzzleInstance == null)
            {
                throw new KeyNotFoundException($"Puzzle instance with ID '{puzzleId}' not found.");
            }

            try
            {
                puzzleInstance.ApplyMove(move); // This will use injected specs, services, and update internal state
            }
            catch (InvalidMoveException ex)
            {
                // Log or handle invalid move attempts specifically
                Console.WriteLine($"Invalid move attempt on puzzle {puzzleId}: {ex.Message}");
                throw; // Re-throw to be handled by the application layer
            }
            catch (RuleConflictException ex)
            {
                // Log or handle rule conflicts
                Console.WriteLine($"Rule conflict during move on puzzle {puzzleId}: {ex.Message}");
                throw; // Re-throw
            }
            // Catch other domain-specific exceptions if necessary

            // Process domain events raised by the aggregate
            IEnumerable<object> domainEvents = puzzleInstance.GetDomainEvents();
            foreach (var domainEvent in domainEvents)
            {
                // _eventPublisher.Publish(domainEvent);
                Console.WriteLine($"Domain Event Published: {domainEvent.GetType().Name}");
                // Example: if (domainEvent is TileStateChangedEvent tse) { ... }
                // Example: if (domainEvent is PuzzleSolvedEvent pse) { ... handle puzzle solved ... }
            }
            puzzleInstance.ClearDomainEvents();

            await _puzzleRepository.SaveAsync(puzzleInstance);
        }
    }
}