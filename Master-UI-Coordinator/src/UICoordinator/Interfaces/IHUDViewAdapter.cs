namespace PatternCipher.UI.Coordinator.Interfaces
{
    /// <summary>
    /// Adapter interface for the HUDView component from UI-Component-Library.
    /// Defines a contract for the UICoordinatorService to interact with the HUDView component.
    /// </summary>
    public interface IHUDViewAdapter
    {
        /// <summary>
        /// Updates the displayed score.
        /// </summary>
        /// <param name="score">The new score to display.</param>
        void UpdateScore(int score);

        /// <summary>
        /// Updates the displayed moves count.
        /// </summary>
        /// <param name="moves">The new moves count to display.</param>
        void UpdateMoves(int moves);

        /// <summary>
        /// Updates the displayed timer.
        /// </summary>
        /// <param name="time">The current time to display (e.g., remaining seconds or elapsed time).</param>
        void UpdateTimer(float time);

        /// <summary>
        /// Displays or updates the current game objective.
        /// </summary>
        /// <param name="objectiveData">Data representing the objective to display (e.g., text, target icons).</param>
        void DisplayObjective(object objectiveData);

        /// <summary>
        /// Shows or hides the hint button.
        /// </summary>
        /// <param name="show">True to show the hint button, false to hide it.</param>
        void ShowHintButton(bool show);
    }
}