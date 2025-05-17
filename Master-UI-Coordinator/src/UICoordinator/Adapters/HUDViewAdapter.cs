using PatternCipher.UI.Coordinator.Interfaces; // For IHUDViewAdapter
using PatternCipher.UI; // For HUDView (from UI-Component-Library)
using System;
using UnityEngine; // For Debug.Log, potentially other Unity types if HUDView API uses them

namespace PatternCipher.UI.Coordinator.Adapters
{
    /// <summary>
    /// Concrete adapter for the HUDView component from REPO-UI-COMPONENTS.
    /// Implements IHUDViewAdapter to interact with the actual HUDView component.
    /// Translates calls from UICoordinatorService to update the HUD's display elements.
    /// </summary>
    public class HUDViewAdapter : IHUDViewAdapter
    {
        private readonly PatternCipher.UI.HUDView _hudViewInstance;

        public HUDViewAdapter(PatternCipher.UI.HUDView hudViewInstance)
        {
            _hudViewInstance = hudViewInstance ?? throw new ArgumentNullException(nameof(hudViewInstance));
        }

        public void UpdateScore(int score)
        {
            if (_hudViewInstance == null) return;
            // Assuming _hudViewInstance has a method like SetScore(int score)
            // _hudViewInstance.SetScore(score);
            Debug.Log($"[HUDViewAdapter] UpdateScore: {score}. Implementation depends on HUDView API.");
        }

        public void UpdateMoves(int currentMoves)
        {
            if (_hudViewInstance == null) return;
            // Assuming _hudViewInstance has a method like SetMoves(int moves)
            // _hudViewInstance.SetMoves(currentMoves);
            Debug.Log($"[HUDViewAdapter] UpdateMoves: {currentMoves}. Implementation depends on HUDView API.");
        }

        public void UpdateTimer(TimeSpan time)
        {
            if (_hudViewInstance == null) return;
            // Assuming _hudViewInstance has a method like SetTime(TimeSpan time) or SetTime(string formattedTime)
            // _hudViewInstance.SetTime(time);
            Debug.Log($"[HUDViewAdapter] UpdateTimer: {time}. Implementation depends on HUDView API.");
        }

        public void DisplayObjective(string objectiveDescription)
        {
            if (_hudViewInstance == null) return;
            // Assuming _hudViewInstance has a method like SetObjective(string description)
            // _hudViewInstance.SetObjective(objectiveDescription);
            Debug.Log($"[HUDViewAdapter] DisplayObjective: {objectiveDescription}. Implementation depends on HUDView API.");
        }

        public void ShowHintButton(bool show)
        {
            if (_hudViewInstance == null) return;
            // Assuming _hudViewInstance has a method like SetHintButtonVisibility(bool show)
            // _hudViewInstance.SetHintButtonVisibility(show);
            // _hudViewInstance.SetHintButtonInteractable(show); // Potentially also interactability
            Debug.Log($"[HUDViewAdapter] ShowHintButton: {show}. Implementation depends on HUDView API.");
        }
    }
}