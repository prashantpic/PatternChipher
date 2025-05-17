using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PatternCipher.Client.Presentation.Screens
{
    public class ScreenManager : MonoBehaviour
    {
        [Header("Screen Prefabs")]
        [SerializeField] private List<MonoBehaviour> screenPrefabs; // Ensure these implement IScreen

        private Dictionary<System.Type, IScreen> _registeredScreens = new Dictionary<System.Type, IScreen>();
        private Stack<IScreen> _navigationHistory = new Stack<IScreen>();
        private IScreen _currentScreen;

        private static ScreenManager _instance;
        public static ScreenManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ScreenManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("ScreenManager");
                        _instance = go.AddComponent<ScreenManager>();
                    }
                }
                return _instance;
            }
        }

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeScreens();
        }

        private void InitializeScreens()
        {
            foreach (var screenPrefab in screenPrefabs)
            {
                if (screenPrefab is IScreen screenInstance)
                {
                    var screenObject = Instantiate(screenPrefab.gameObject, transform);
                    IScreen screen = screenObject.GetComponent<IScreen>();
                    if (screen != null)
                    {
                        screenObject.SetActive(false); // Start with all screens hidden
                        _registeredScreens[screen.GetType()] = screen;
                         // The original instructions for IScreen in SDS mention Initialize(ScreenManager, object),
                         // but BaseScreenController might handle this via Awake/Start.
                         // For now, let's assume screens initialize themselves or BaseScreenController handles it.
                    }
                    else
                    {
                        Debug.LogError($"Prefab {screenPrefab.name} does not implement IScreen or have it as a component.");
                    }
                }
                else
                {
                     Debug.LogError($"Prefab {screenPrefab.name} is not an IScreen.");
                }
            }
        }

        public void PushScreen<T>() where T : MonoBehaviour, IScreen
        {
            System.Type screenType = typeof(T);
            if (_registeredScreens.TryGetValue(screenType, out IScreen screenToShow))
            {
                if (_currentScreen != null)
                {
                    _currentScreen.Hide();
                    _navigationHistory.Push(_currentScreen);
                }
                _currentScreen = screenToShow;
                _currentScreen.Show();
                // _currentScreen.UpdateView(); // Or handle view updates within Show/OnShow
            }
            else
            {
                Debug.LogError($"Screen type {screenType.Name} not registered or prefab not found.");
            }
        }

        public void PopScreen()
        {
            if (_navigationHistory.Count > 0)
            {
                if (_currentScreen != null)
                {
                    _currentScreen.Hide();
                }
                _currentScreen = _navigationHistory.Pop();
                _currentScreen.Show();
                // _currentScreen.UpdateView();
            }
            else
            {
                Debug.LogWarning("Navigation history is empty. Cannot pop screen.");
            }
        }

        public void ShowOnlyScreen<T>() where T : MonoBehaviour, IScreen
        {
            System.Type screenType = typeof(T);
            if (_registeredScreens.TryGetValue(screenType, out IScreen screenToShow))
            {
                if (_currentScreen != null && _currentScreen != screenToShow)
                {
                    _currentScreen.Hide();
                }

                // Hide all other screens that might be in history but not currently active
                foreach (var screen in _navigationHistory)
                {
                    if (screen != screenToShow)
                    {
                        screen.Hide();
                    }
                }
                _navigationHistory.Clear();

                _currentScreen = screenToShow;
                _currentScreen.Show();
                // _currentScreen.UpdateView();
            }
            else
            {
                Debug.LogError($"Screen type {screenType.Name} not registered or prefab not found.");
            }
        }
        
        // For IScreen to call:
        // public void NotifyScreenShown(IScreen screen) { /* Potentially handle analytics or state changes */ }
        // public void NotifyScreenHidden(IScreen screen) { /* Potentially handle analytics or state changes */ }
    }
}