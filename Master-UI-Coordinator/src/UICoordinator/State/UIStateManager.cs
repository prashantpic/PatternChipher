using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine; // For Debug
// Assuming IPersistenceService and UIState exist
// using PatternCipher.Utilities;
// namespace PatternCipher.UI.Coordinator.State { public class UIState { public ScreenType LastActiveScreen; /* other props */ } }
// namespace PatternCipher.UI.Coordinator.Navigation { public enum ScreenType { MainMenu } } // Ensure ScreenType is defined
// Assuming IUIStateManager interface exists
// namespace PatternCipher.UI.Coordinator.State { public interface IUIStateManager { /* methods */ } }


namespace PatternCipher.UI.Coordinator.State
{
    public class UIStateManager : IUIStateManager // Inferred from SDS
    {
        private UIState _uiState;
        private readonly Dictionary<string, List<Delegate>> _subscribers;
        private readonly PatternCipher.Utilities.IPersistenceService _persistenceService;
        private const string UIStateKey = "UIState";

        public UIStateManager(PatternCipher.Utilities.IPersistenceService persistenceService, UIState initialState = null)
        {
            _persistenceService = persistenceService ?? throw new ArgumentNullException(nameof(persistenceService));
            _uiState = initialState ?? new UIState();
            _subscribers = new Dictionary<string, List<Delegate>>();
        }

        public async Task LoadStateAsync()
        {
            var loadedState = await _persistenceService.LoadAsync<UIState>(UIStateKey);
            if (loadedState != null)
            {
                _uiState = loadedState;
            }
            else
            {
                _uiState = new UIState(); // Initialize with defaults
                // Potentially save this default state immediately
                // await SaveStateAsync(); 
            }
            // Notify about loaded state if necessary, e.g. for all relevant keys
            // For simplicity, assuming components fetch initial state after load.
        }

        public async Task SaveStateAsync()
        {
            await _persistenceService.SaveAsync(UIStateKey, _uiState);
        }

        public T GetStateValue<T>(string key)
        {
            // This requires _uiState to be a flexible dictionary or use reflection,
            // or have specific properties. The SDS implies UIState is a data model with known properties.
            // Let's assume direct property access based on UIState definition.
            // This generic GetStateValue is harder to implement safely without reflection
            // if UIState has fixed properties.

            // Example for a known property:
            // if (key == nameof(UIState.LastActiveScreen)) return (T)(object)_uiState.LastActiveScreen;
            
            // For a truly generic dictionary-like state:
            // if (_uiState.dynamicProperties.TryGetValue(key, out object value)) return (T)value;

            // Given the SDS, UIState is a class with specific properties like `CurrentLevelPackId`.
            // A simpler approach:
            // public UIState CurrentState => _uiState; // And access properties directly.
            // Or have specific getters:
            // public ScreenType GetLastActiveScreen() => _uiState.LastActiveScreen;

            // For the purpose of this example, let's use a simplified reflection approach (not recommended for performance).
            try
            {
                var property = typeof(UIState).GetProperty(key);
                if (property != null)
                {
                    return (T)property.GetValue(_uiState);
                }
                Debug.LogWarning($"UIStateManager: Property '{key}' not found in UIState.");
                return default(T);
            }
            catch (Exception ex)
            {
                Debug.LogError($"UIStateManager: Error getting state value for key '{key}': {ex.Message}");
                return default(T);
            }
        }

        public void SetStateValue<T>(string key, T value)
        {
            // Similar to GetStateValue, requires reflection or specific setters for a fixed UIState class.
            try
            {
                var property = typeof(UIState).GetProperty(key);
                if (property != null)
                {
                    T oldValue = (T)property.GetValue(_uiState);
                    if (!EqualityComparer<T>.Default.Equals(oldValue, value))
                    {
                        property.SetValue(_uiState, value);
                        NotifySubscribers(key, value);
                        // Auto-save on change? Or require explicit SaveStateAsync()?
                        // For now, require explicit save.
                    }
                }
                else
                {
                    Debug.LogWarning($"UIStateManager: Property '{key}' not found in UIState.");
                }
            }
            catch (Exception ex)
            {
                 Debug.LogError($"UIStateManager: Error setting state value for key '{key}': {ex.Message}");
            }
        }
        
        // Direct access for known UIState object if preferred over generic Get/Set
        public UIState GetCurrentUIState()
        {
            return _uiState;
        }


        public void SubscribeToStateChange<T>(string key, Action<T> callback)
        {
            if (!_subscribers.ContainsKey(key))
            {
                _subscribers[key] = new List<Delegate>();
            }
            _subscribers[key].Add(callback);
        }

        public void UnsubscribeFromStateChange<T>(string key, Action<T> callback)
        {
            if (_subscribers.TryGetValue(key, out var callbackList))
            {
                callbackList.Remove(callback);
                if (callbackList.Count == 0)
                {
                    _subscribers.Remove(key);
                }
            }
        }

        private void NotifySubscribers<T>(string key, T newValue)
        {
            if (_subscribers.TryGetValue(key, out var callbackList))
            {
                // ToList() to avoid issues if a callback unsubscribes itself
                foreach (var del in callbackList.ToList())
                {
                    if (del is Action<T> action)
                    {
                        try
                        {
                            action(newValue);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"UIStateManager: Error in subscriber for key '{key}': {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}