using UnityEngine;
using System.Collections.Generic;
using System; // Required for Action

namespace PatternCipher.Client.Presentation.Screens
{
    public enum ScreenType
    {
        // Define screen types, e.g., MainMenu, GameScreen, SettingsScreen, LevelSelectionScreen, etc.
        MainMenu,
        GameScreen,
        Settings,
        LevelSelection
        // Add other screen types as needed
    }

    public class ScreenManager : MonoBehaviour
    {
        [Header("Screen Prefabs")]
        [SerializeField] private GameObject mainMenuScreenPrefab;
        [SerializeField] private GameObject gameScreenPrefab;
        [SerializeField] private GameObject settingsScreenPrefab;
        // Add other screen prefabs here

        private Dictionary<ScreenType, GameObject> screenPrefabs = new Dictionary<ScreenType, GameObject>();
        private Dictionary<ScreenType, IScreen> activeScreens = new Dictionary<ScreenType, IScreen>();
        private Stack<IScreen> screenHistory = new Stack<IScreen>();

        private IScreen currentScreen;

        public static ScreenManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeScreenPrefabs();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeScreenPrefabs()
        {
            if (mainMenuScreenPrefab != null) screenPrefabs[ScreenType.MainMenu] = mainMenuScreenPrefab;
            if (gameScreenPrefab != null) screenPrefabs[ScreenType.GameScreen] = gameScreenPrefab;
            if (settingsScreenPrefab != null) screenPrefabs[ScreenType.Settings] = settingsScreenPrefab;
            // Initialize other prefabs
        }

        private IScreen GetOrInstantiateScreen(ScreenType screenType)
        {
            if (activeScreens.TryGetValue(screenType, out IScreen screenInstance) && screenInstance != null && (screenInstance as MonoBehaviour)?.gameObject != null)
            {
                return screenInstance;
            }

            if (screenPrefabs.TryGetValue(screenType, out GameObject prefab))
            {
                GameObject screenObject = Instantiate(prefab, transform); // Instantiate under ScreenManager
                IScreen newScreen = screenObject.GetComponent<IScreen>();
                if (newScreen != null)
                {
                    // Assuming IScreen has an Initialize method as per initial detailed spec
                    // newScreen.Initialize(this, null); // Pass data if needed
                    activeScreens[screenType] = newScreen;
                    screenObject.SetActive(false); // Start hidden
                    return newScreen;
                }
                else
                {
                    Debug.LogError($"Prefab for {screenType} does not have a component implementing IScreen.");
                }
            }
            else
            {
                Debug.LogError($"Screen prefab for {screenType} not found.");
            }
            return null;
        }


        public void PushScreen<T>(object data = null) where T : MonoBehaviour, IScreen
        {
            ScreenType screenTypeToFind = ScreenTypeForComponent<T>();
            if (screenTypeToFind == default && !IsKnownScreenType<T>()) // A bit of a hack, better to map T to ScreenType
            {
                 Debug.LogError($"Screen type for {typeof(T).Name} is not registered or inferable.");
                 return;
            }
            if (screenTypeToFind == default) // Attempt to infer if not directly mapped
            {
                foreach(var kvp in screenPrefabs)
                {
                    if (kvp.Value.GetComponent<T>() != null)
                    {
                        screenTypeToFind = kvp.Key;
                        break;
                    }
                }
            }


            IScreen newScreen = GetOrInstantiateScreen(screenTypeToFind);

            if (newScreen != null)
            {
                if (currentScreen != null)
                {
                    currentScreen.Hide();
                    screenHistory.Push(currentScreen);
                }
                currentScreen = newScreen;
                // If IScreen has Initialize method:
                // (currentScreen as PatternCipher.Client.Presentation.Screens.Common.BaseScreenController)?.InitializeScreen(this, data);
                currentScreen.Show(); // Or pass data here if Show takes data
            }
        }

        public void PopScreen()
        {
            if (currentScreen != null)
            {
                currentScreen.Hide();
                if (activeScreens.ContainsValue(currentScreen) && currentScreen is MonoBehaviour mbScreen)
                {
                     // Optionally destroy or just deactivate
                     // Destroy(mbScreen.gameObject);
                     // activeScreens.Remove(GetKeyByValue(activeScreens, currentScreen));
                }
            }

            if (screenHistory.Count > 0)
            {
                currentScreen = screenHistory.Pop();
                currentScreen.Show();
            }
            else
            {
                currentScreen = null;
                Debug.LogWarning("Screen history is empty. No screen to pop to.");
                // Optionally, go to a default screen like MainMenu
                // ShowOnlyScreen<MainMenuController>();
            }
        }

        public void ShowOnlyScreen<T>(object data = null) where T : MonoBehaviour, IScreen
        {
            ScreenType screenTypeToFind = ScreenTypeForComponent<T>();
             if (screenTypeToFind == default && !IsKnownScreenType<T>())
            {
                 Debug.LogError($"Screen type for {typeof(T).Name} is not registered or inferable.");
                 return;
            }
            if (screenTypeToFind == default)
            {
                foreach(var kvp in screenPrefabs)
                {
                    if (kvp.Value.GetComponent<T>() != null)
                    {
                        screenTypeToFind = kvp.Key;
                        break;
                    }
                }
            }

            IScreen newScreen = GetOrInstantiateScreen(screenTypeToFind);

            if (newScreen != null)
            {
                if (currentScreen != null)
                {
                    currentScreen.Hide();
                }
                // Hide all other screens and clear history
                foreach (var screen in activeScreens.Values)
                {
                    if (screen != newScreen && screen != null && (screen as MonoBehaviour)?.gameObject.activeSelf == true)
                    {
                        screen.Hide();
                    }
                }
                screenHistory.Clear();

                currentScreen = newScreen;
                // If IScreen has Initialize method:
                // (currentScreen as PatternCipher.Client.Presentation.Screens.Common.BaseScreenController)?.InitializeScreen(this, data);
                currentScreen.Show(); // Or pass data here
            }
        }
        
        // Helper to map Component Type T to ScreenType enum.
        // This would ideally be more robust, e.g., attributes on IScreen implementations.
        private ScreenType ScreenTypeForComponent<T>() where T : IScreen
        {
            if (typeof(T).Name.Contains("MainMenu")) return ScreenType.MainMenu;
            if (typeof(T).Name.Contains("GameScreen")) return ScreenType.GameScreen;
            if (typeof(T).Name.Contains("Settings")) return ScreenType.Settings;
            // Add more mappings
            return default(ScreenType); // Or throw an error
        }
         private bool IsKnownScreenType<T>() where T : IScreen
        {
            // A simple check. Could be improved with a registration system.
            return typeof(T).Name.Contains("MainMenu") ||
                   typeof(T).Name.Contains("GameScreen") ||
                   typeof(T).Name.Contains("Settings");
        }

        // Helper to get key by value, useful if you need to remove from activeScreens by IScreen instance
        private ScreenType GetKeyByValue(Dictionary<ScreenType, IScreen> dict, IScreen value)
        {
            foreach (var pair in dict)
            {
                if (pair.Value.Equals(value))
                {
                    return pair.Key;
                }
            }
            return default(ScreenType); // Or throw exception
        }
    }
}