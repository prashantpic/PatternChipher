using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine; // For Debug

// Assuming necessary types exist in their respective namespaces
// using PatternCipher.UI.Coordinator.Assets;
// using PatternCipher.UI.Coordinator.State;
// using PatternCipher.UI.Coordinator.Interfaces;
// using PatternCipher.UI.Coordinator.Events; // For UIEvents

// Define UIEvents if not provided elsewhere for compilation:
namespace PatternCipher.UI.Coordinator.Events
{
    public static class UIEvents
    {
        public static event Action<PatternCipher.UI.Coordinator.Navigation.ScreenType, PatternCipher.UI.Coordinator.Navigation.NavigationPayload> OnScreenChanged;
        public static void RaiseScreenChanged(PatternCipher.UI.Coordinator.Navigation.ScreenType type, PatternCipher.UI.Coordinator.Navigation.NavigationPayload payload) => OnScreenChanged?.Invoke(type, payload);
    }
}


namespace PatternCipher.UI.Coordinator.Navigation
{
    public class ScreenNavigator : IScreenNavigator // Inferred from SDS
    {
        private readonly PatternCipher.UI.Coordinator.Assets.IAssetLoader _assetLoader;
        private readonly IScreenTransitionController _transitionController;
        private readonly PatternCipher.UI.Coordinator.State.IUIStateManager _uiStateManager;
        private readonly PatternCipher.UI.Coordinator.Assets.UIPrefabRegistry _prefabRegistry;

        private readonly Dictionary<ScreenType, PatternCipher.UI.Coordinator.Interfaces.IUIView> _activeScreens;
        private readonly Dictionary<ScreenType, Func<Task<PatternCipher.UI.Coordinator.Interfaces.IUIView>>> _screenFactories;
        private readonly Stack<ScreenType> _navigationStack;
        
        private PatternCipher.UI.Coordinator.Interfaces.IUIView _currentScreenView;


        public bool CanGoBack => _navigationStack.Count > 1;

        public ScreenNavigator(
            PatternCipher.UI.Coordinator.Assets.IAssetLoader assetLoader,
            IScreenTransitionController transitionController,
            PatternCipher.UI.Coordinator.State.IUIStateManager uiStateManager,
            PatternCipher.UI.Coordinator.Assets.UIPrefabRegistry prefabRegistry)
        {
            _assetLoader = assetLoader ?? throw new ArgumentNullException(nameof(assetLoader));
            _transitionController = transitionController ?? throw new ArgumentNullException(nameof(transitionController));
            _uiStateManager = uiStateManager ?? throw new ArgumentNullException(nameof(uiStateManager));
            _prefabRegistry = prefabRegistry ?? throw new ArgumentNullException(nameof(prefabRegistry));

            _activeScreens = new Dictionary<ScreenType, PatternCipher.UI.Coordinator.Interfaces.IUIView>();
            _screenFactories = new Dictionary<ScreenType, Func<Task<PatternCipher.UI.Coordinator.Interfaces.IUIView>>>();
            _navigationStack = new Stack<ScreenType>();
        }

        public void RegisterScreenViewFactory(ScreenType screenType, Func<Task<PatternCipher.UI.Coordinator.Interfaces.IUIView>> factory)
        {
            if (_screenFactories.ContainsKey(screenType))
            {
                Debug.LogWarning($"ScreenNavigator: Factory for {screenType} already registered. Overwriting.");
            }
            _screenFactories[screenType] = factory;
        }

        public async Task ShowScreenAsync(ScreenType screenType, NavigationPayload payload = null)
        {
            PatternCipher.UI.Coordinator.Interfaces.IUIView previousScreen = _currentScreenView;

            if (previousScreen != null)
            {
                previousScreen.OnNavigateFrom();
                // previousScreen.HideAsync() will be called after transition or by transition controller
            }

            PatternCipher.UI.Coordinator.Interfaces.IUIView nextScreen = await GetOrCreateScreenAsync(screenType);
            if (nextScreen == null)
            {
                Debug.LogError($"ScreenNavigator: Could not create or find screen of type {screenType}.");
                return;
            }
            
            _currentScreenView = nextScreen;

            // Initialize if not already initialized or if it needs re-initialization logic
            // Assuming InitializeAsync is safe to call multiple times or handles its own state.
            // More typically, InitializeAsync is called once upon creation.
            // Here, payload is passed to OnNavigateTo as per SDS.
            // Let's assume InitializeAsync is called when GetOrCreateScreenAsync first creates it.
            
            _currentScreenView.OnNavigateTo(payload);

            if (_transitionController != null)
            {
                await _transitionController.PlayTransitionAsync(previousScreen, _currentScreenView);
            }
            
            if(previousScreen != null) await previousScreen.HideAsync(); // Hide old screen after transition
            await _currentScreenView.ShowAsync();


            // Manage navigation stack
            if (_navigationStack.Count == 0 || _navigationStack.Peek() != screenType)
            {
                _navigationStack.Push(screenType);
            }
            
            if (_uiStateManager != null)
            {
                 // Assuming UIState has a property LastActiveScreen of type ScreenType
                 // await _uiStateManager.SetStateValueAsync("LastActiveScreen", screenType); // If SetStateValueAsync exists
                 // For now, assuming UIState object is directly mutable or has specific setters.
                 // The SDS mentions UIState.LastActiveScreen.
            }

            PatternCipher.UI.Coordinator.Events.UIEvents.RaiseScreenChanged(screenType, payload);
        }

        public async Task GoBackAsync()
        {
            if (!CanGoBack)
            {
                Debug.LogWarning("ScreenNavigator: No screen to go back to.");
                return;
            }

            _navigationStack.Pop(); // Remove current screen
            ScreenType targetScreenType = _navigationStack.Peek(); // Get new top screen

            // Payload for going back is typically null or specific "back navigation" payload
            await ShowScreenAsync(targetScreenType, new NavigationPayload { IsBackNavigation = true });
        }
        
        private async Task<PatternCipher.UI.Coordinator.Interfaces.IUIView> GetOrCreateScreenAsync(ScreenType screenType)
        {
            if (_activeScreens.TryGetValue(screenType, out var screenView) && screenView != null)
            {
                // Screen instance exists, ensure it's properly set up for display
                // (e.g., parented correctly, if pooling detached it)
                return screenView;
            }

            PatternCipher.UI.Coordinator.Interfaces.IUIView newScreenInstance = null;
            if (_screenFactories.TryGetValue(screenType, out var factory))
            {
                newScreenInstance = await factory();
            }
            else if (_prefabRegistry != null)
            {
                string addressableKey = _prefabRegistry.GetPrefabKey(screenType);
                if (!string.IsNullOrEmpty(addressableKey))
                {
                    // Assuming IAssetLoader has LoadGameObjectAsync<T> or similar that returns/attaches IUIView
                    GameObject screenPrefabInstance = await _assetLoader.LoadGameObjectAsync(addressableKey);
                    if (screenPrefabInstance != null)
                    {
                        newScreenInstance = screenPrefabInstance.GetComponent<PatternCipher.UI.Coordinator.Interfaces.IUIView>();
                        if (newScreenInstance == null)
                        {
                            Debug.LogError($"ScreenNavigator: Loaded prefab for {screenType} does not have an IUIView component.");
                            UnityEngine.Object.Destroy(screenPrefabInstance); // Clean up
                        }
                    }
                }
                else
                {
                     Debug.LogError($"ScreenNavigator: No prefab key found in UIPrefabRegistry for {screenType}.");
                }
            }

            if (newScreenInstance != null)
            {
                await newScreenInstance.InitializeAsync(null); // Initial payload null, specific payload in OnNavigateTo
                _activeScreens[screenType] = newScreenInstance; // Cache it
                // Ensure it's parented to a UI Canvas, this might be handled by the factory or asset loader logic
            }
            else
            {
                 Debug.LogError($"ScreenNavigator: Failed to create screen instance for {screenType}.");
            }
            return newScreenInstance;
        }

        public async Task HideScreenAsync(ScreenType screenType) // Added as per common navigator needs
        {
            if (_activeScreens.TryGetValue(screenType, out var screenView) && screenView != null)
            {
                await screenView.HideAsync();
                // Optionally, could dispose or pool it here based on strategy
            }
        }

        public void Dispose()
        {
            foreach (var screen in _activeScreens.Values)
            {
                screen?.Dispose();
            }
            _activeScreens.Clear();
            _screenFactories.Clear();
            _navigationStack.Clear();
            _currentScreenView = null;
        }
    }
}