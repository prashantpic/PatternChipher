using System;
using System.Threading.Tasks;

namespace PatternCipher.UI.Coordinator.State
{
    /// <summary>
    /// Interface for managing shared UI state across different screens and components.
    /// This includes mechanisms for getting, setting, and persisting UI-related state data.
    /// </summary>
    public interface IUIStateManager
    {
        /// <summary>
        /// Retrieves a state value associated with the given key.
        /// </summary>
        /// <typeparam name="T">The type of the state value.</typeparam>
        /// <param name="key">The unique key for the state value.</param>
        /// <param name="defaultValue">The value to return if the key is not found.</param>
        /// <returns>The state value, or the defaultValue if not found.</returns>
        T GetStateValue<T>(string key, T defaultValue = default);

        /// <summary>
        /// Sets a state value for the given key.
        /// If the value changes, subscribers for this key will be notified.
        /// </summary>
        /// <typeparam name="T">The type of the state value.</typeparam>
        /// <param name="key">The unique key for the state value.</param>
        /// <param name="value">The new value to set.</param>
        void SetStateValue<T>(string key, T value);

        /// <summary>
        /// Subscribes a callback to be invoked when the state value for the specified key changes.
        /// </summary>
        /// <typeparam name="T">The type of the state value.</typeparam>
        /// <param name="key">The key of the state value to monitor.</param>
        /// <param name="callback">The action to invoke with the new value when it changes.</param>
        void SubscribeToStateChange<T>(string key, Action<T> callback);

        /// <summary>
        /// Unsubscribes a callback from state changes for the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the state value.</typeparam>
        /// <param name="key">The key of the state value.</param>
        /// <param name="callback">The callback action to remove.</param>
        void UnsubscribeFromStateChange<T>(string key, Action<T> callback);

        /// <summary>
        /// Asynchronously loads the UI state from a persistent store.
        /// </summary>
        /// <returns>A task that completes when the state has been loaded.</returns>
        Task LoadStateAsync();

        /// <summary>
        /// Asynchronously saves the current UI state to a persistent store.
        /// </summary>
        /// <returns>A task that completes when the state has been saved.</returns>
        Task SaveStateAsync();
    }
}