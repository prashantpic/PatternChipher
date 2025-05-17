using System.Threading.Tasks;
using PatternCipher.Client.Domain.Aggregates;
using PatternCipher.Client.Domain.DomainServices;
using PatternCipher.Client.Domain.Entities;
using PatternCipher.Client.Domain.Events;
using PatternCipher.Client.Domain.Repositories;
using PatternCipher.Client.Domain.ValueObjects;
using PatternCipher.Client.Core.Events; // For GlobalEventBus
using UnityEngine; // For Debug

namespace PatternCipher.Client.Application.GameFlow
{
    public class LevelLoadService
    {
        private readonly LevelGenerationService _levelGenerationService;
        private readonly ILevelProfileRepository _levelProfileRepository; // To save/load level state if needed
        private readonly GlobalEventBus _eventBus;
        
        // These would be managed instances, potentially singletons or passed via DI
        private GridAggregate _currentGridAggregate; 
        private LevelProfileAggregate _currentLevelProfileAggregate;

        public LevelLoadService(
            LevelGenerationService levelGenerationService,
            ILevelProfileRepository levelProfileRepository,
            GlobalEventBus eventBus)
        {
            _levelGenerationService = levelGenerationService;
            _levelProfileRepository = levelProfileRepository;
            _eventBus = eventBus;
        }

        public async Task<bool> LoadLevelAsync(LevelGenerationParameters parameters)
        {
            Debug.Log($"LevelLoadService: Attempting to load level with parameters: {parameters}");

            // Option 1: Always generate a new level
            try
            {
                var generatedData = _levelGenerationService.GenerateLevel(parameters);
                if (generatedData == null)
                {
                    Debug.LogError("Level generation returned null data.");
                    return false;
                }

                _currentGridAggregate = generatedData.Grid;
                _currentLevelProfileAggregate = new LevelProfileAggregate(
                    parameters.LevelId, // Assuming LevelId is part of parameters or generated
                    generatedData.Objective,
                    0, // Initial score
                    0, // Initial moves
                    0f // Initial time
                );
                
                // If you have a HintGenerationService, you might set its solution path here
                // _hintGenerationService.SetSolutionPath(generatedData.Solution);

                _eventBus.Publish(new GridInitializedEvent(
                    _currentGridAggregate.Dimensions, 
                    _currentGridAggregate.GetAllTiles() // Or a DTO representation if needed
                ));
                
                // Publish an event that the level profile is ready
                // _eventBus.Publish(new LevelProfileInitializedEvent(_currentLevelProfileAggregate));


                Debug.Log("LevelLoadService: New level generated and initialized successfully.");
                return true;
            }
            catch (PuzzleGenerationException pge)
            {
                Debug.LogError($"LevelLoadService: PuzzleGenerationException - {pge.Message}");
                return false;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"LevelLoadService: Unexpected error during level generation/initialization - {ex.Message}");
                return false;
            }

            // Option 2: Try to load from a saved state first (e.g., for mid-level save/resume)
            // This would involve IGridRepository and potentially more complex logic.
            // For now, focusing on new level generation.
            /*
            string levelId = parameters.LevelId; // Assuming LevelId is part of parameters
            var loadedProfile = await _levelProfileRepository.LoadLevelProfileAsync(levelId);
            if (loadedProfile != null)
            {
                // _currentGridAggregate = await _gridRepository.LoadGridStateAsync(levelId);
                // _currentLevelProfileAggregate = loadedProfile;
                // _eventBus.Publish(new GridInitializedEvent(...));
                // Debug.Log("LevelLoadService: Level loaded from saved state.");
                // return true;
            }
            else
            {
                // Proceed with generation as above
            }
            */
        }

        // Method to get current game state for other services
        public GridAggregate GetCurrentGrid() => _currentGridAggregate;
        public LevelProfileAggregate GetCurrentLevelProfile() => _currentLevelProfileAggregate;
    }
}