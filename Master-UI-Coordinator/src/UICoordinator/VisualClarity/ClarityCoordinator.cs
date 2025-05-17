using UnityEngine; // For Debug
// Assuming IHUDViewAdapter, IGridViewAdapter, SpecialTileVisualCueManager exist
// using PatternCipher.UI.Coordinator.Interfaces;
// namespace PatternCipher.UI.Coordinator.VisualClarity { public class SpecialTileVisualCueManager { /* methods */ } }

namespace PatternCipher.UI.Coordinator.VisualClarity
{
    public class ClarityCoordinator // : IClarityCoordinator (inferred from SDS)
    {
        private readonly PatternCipher.UI.Coordinator.Interfaces.IHUDViewAdapter _hudViewAdapter;
        private readonly PatternCipher.UI.Coordinator.Interfaces.IGridViewAdapter _gridViewAdapter;
        private readonly SpecialTileVisualCueManager _specialTileVisualCueManager;

        public ClarityCoordinator(
            PatternCipher.UI.Coordinator.Interfaces.IHUDViewAdapter hudViewAdapter,
            PatternCipher.UI.Coordinator.Interfaces.IGridViewAdapter gridViewAdapter,
            SpecialTileVisualCueManager specialTileVisualCueManager)
        {
            _hudViewAdapter = hudViewAdapter ?? throw new System.ArgumentNullException(nameof(hudViewAdapter));
            _gridViewAdapter = gridViewAdapter ?? throw new System.ArgumentNullException(nameof(gridViewAdapter));
            _specialTileVisualCueManager = specialTileVisualCueManager ?? throw new System.ArgumentNullException(nameof(specialTileVisualCueManager));
        }

        // REQ-UIX-015: Ensures critical game info is visually clear.

        public void UpdatePuzzleGoalDisplay(string goalDescription, int currentProgress, int targetProgress)
        {
            if (_hudViewAdapter != null)
            {
                // Assuming IHUDViewAdapter has a method like this:
                // _hudViewAdapter.DisplayObjective(goalDescription, currentProgress, targetProgress);
                Debug.Log($"ClarityCoordinator: Updating HUD - Goal: {goalDescription}, Progress: {currentProgress}/{targetProgress}");
            }
        }

        public void UpdateScoreDisplay(int newScore)
        {
             if (_hudViewAdapter != null)
            {
                // _hudViewAdapter.UpdateScore(newScore);
                Debug.Log($"ClarityCoordinator: Updating HUD - Score: {newScore}");
            }
        }
        
        public void UpdateMovesDisplay(int moves)
        {
             if (_hudViewAdapter != null)
            {
                // _hudViewAdapter.UpdateMoves(moves);
                Debug.Log($"ClarityCoordinator: Updating HUD - Moves: {moves}");
            }
        }


        // Example: Domain data for a tile might include its symbol, type, and a special state (e.g., "highlighted_goal")
        public void UpdateTileStateVisuals(Vector2Int tilePosition, string symbol, string tileState, bool isSpecial)
        {
            if (_gridViewAdapter != null)
            {
                // Assuming IGridViewAdapter has methods to update visuals of a specific tile
                // _gridViewAdapter.UpdateTileVisual(tilePosition, symbol, tileState);
                Debug.Log($"ClarityCoordinator: Updating Grid Tile [{tilePosition.x},{tilePosition.y}] - Symbol: {symbol}, State: {tileState}");
            }

            if (isSpecial && _specialTileVisualCueManager != null)
            {
                // _specialTileVisualCueManager.ShowCueForTile(tilePosition, tileState); // or type of special
                 Debug.Log($"ClarityCoordinator: Updating Special Cue for Tile [{tilePosition.x},{tilePosition.y}] based on state: {tileState}");
            }
        }
        
        public void UpdateSpecialTileCues(/* Domain data about special tiles */)
        {
            if (_specialTileVisualCueManager != null)
            {
                // This would iterate over special tiles data and call methods on _specialTileVisualCueManager
                // e.g., _specialTileVisualCueManager.UpdateCue(tileId, cueType, parameters);
                Debug.Log($"ClarityCoordinator: Updating special tile cues globally.");
            }
        }

        public void HighlightGoalTiles(System.Collections.Generic.List<Vector2Int> tilePositions)
        {
            if(_gridViewAdapter != null)
            {
                // _gridViewAdapter.HighlightTiles(tilePositions, HighlightType.Goal);
                 Debug.Log($"ClarityCoordinator: Highlighting {tilePositions.Count} goal tiles.");
            }
        }
    }
}