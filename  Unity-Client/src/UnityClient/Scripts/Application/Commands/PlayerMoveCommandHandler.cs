using PatternCipher.Client.Domain.Aggregates;
using PatternCipher.Client.Domain.DomainServices;
using PatternCipher.Client.Domain.Exceptions;
using PatternCipher.Client.Domain.ValueObjects; // For GridPosition
using PatternCipher.Client.Core.Events; // For GlobalEventBus
using PatternCipher.Client.Domain.Events; // For PlayerMoveMadeEvent
using UnityEngine; // For Debug

namespace PatternCipher.Client.Application.Commands
{
    public class PlayerMoveCommandHandler
    {
        private readonly GridManagementService _gridManagementService;
        private readonly GlobalEventBus _eventBus;

        // The handler needs access to the current state of these aggregates.
        // This might be passed in, or the handler might fetch them from a central state manager/service.
        private GridAggregate _currentGridAggregate;
        private LevelProfileAggregate _currentLevelProfileAggregate;


        public PlayerMoveCommandHandler(
            GridManagementService gridManagementService,
            GlobalEventBus eventBus,
            GridAggregate gridAggregate, /* This needs to be the active game's grid */
            LevelProfileAggregate levelProfileAggregate /* Active game's profile */)
        {
            _gridManagementService = gridManagementService;
            _eventBus = eventBus;
            _currentGridAggregate = gridAggregate;
            _currentLevelProfileAggregate = levelProfileAggregate;
        }
        
        // Alternative constructor if aggregates are managed by another service (e.g. LevelLoadService or a GameStateManager)
        // public PlayerMoveCommandHandler(GridManagementService gridManagementService, GlobalEventBus eventBus, Func<GridAggregate> gridProvider, Func<LevelProfileAggregate> profileProvider)

        public void Handle(ProcessPlayerMoveCommand command)
        {
            if (_currentGridAggregate == null || _currentLevelProfileAggregate == null)
            {
                Debug.LogError("PlayerMoveCommandHandler: Current grid or level profile is not set.");
                // Optionally publish a generic error event or throw an application exception
                return;
            }

            if (_currentLevelProfileAggregate.IsCompleted)
            {
                Debug.LogWarning("PlayerMoveCommandHandler: Move attempted on already completed level.");
                return; // Don't process moves if level is already done
            }

            try
            {
                GridManagementService.ProcessMoveResult result = null; // Define ProcessMoveResult in GridManagementService

                switch (command.Type)
                {
                    case MoveType.Tap:
                        result = _gridManagementService.ProcessPlayerTap(_currentGridAggregate, command.Position1);
                        break;
                    case MoveType.Swap:
                        result = _gridManagementService.ProcessPlayerSwap(_currentGridAggregate, command.Position1, command.Position2);
                        break;
                    default:
                        Debug.LogError($"PlayerMoveCommandHandler: Unknown move type: {command.Type}");
                        return;
                }

                if (result != null && result.Success)
                {
                    _currentLevelProfileAggregate.IncrementMoves();
                    _currentLevelProfileAggregate.AddScore(result.ScoreGained);
                    
                    _eventBus.Publish(new PlayerMoveMadeEvent(
                        command.Type == MoveType.Tap ? 
                            new PlayerMove(command.Position1) : 
                            new PlayerMove(command.Position1, command.Position2), // Define PlayerMove struct/class
                        result.Success, 
                        result.ScoreGained, 
                        result.TilesAffectedCount
                    ));

                    // Check for level completion after a successful move
                    if (_currentLevelProfileAggregate.CheckCompletion(_currentGridAggregate))
                    {
                        _eventBus.Publish(new LevelCompletedEvent(
                            _currentLevelProfileAggregate.LevelId,
                            _currentLevelProfileAggregate.CurrentScore,
                            _currentLevelProfileAggregate.MovesTaken,
                            _currentLevelProfileAggregate.TimeElapsed,
                            _currentLevelProfileAggregate.CalculateStars() // Assuming a method to calculate stars
                        ));
                    }
                }
                else if (result != null && !result.Success)
                {
                     // Publish a feedback event for invalid move if not handled by GridManagementService
                     _eventBus.Publish(new TileInteractionFeedbackEvent(
                        command.Position1, 
                        command.Type == MoveType.Tap ? InteractionType.Tap : InteractionType.Swap, 
                        FeedbackType.OperationInvalid, // A more generic invalid feedback
                        false, 
                        command.Position2 // null if tap
                        // any other relevant data like error message
                     ));
                }
            }
            catch (InvalidGridOperationException ex)
            {
                Debug.LogWarning($"PlayerMoveCommandHandler: Invalid grid operation - {ex.Message}");
                // Publish feedback event based on exception details
                _eventBus.Publish(new TileInteractionFeedbackEvent(
                    command.Position1,
                    command.Type == MoveType.Tap ? InteractionType.Tap : InteractionType.Swap,
                    FeedbackType.OperationInvalid, // Or a more specific type based on ex
                    false,
                    command.Position2
                ));
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"PlayerMoveCommandHandler: Unexpected error - {ex.Message}");
                // Handle unexpected errors, possibly publish a generic error event
            }
        }
         public void UpdateAggregates(GridAggregate grid, LevelProfileAggregate profile)
        {
            _currentGridAggregate = grid;
            _currentLevelProfileAggregate = profile;
        }
    }
}