using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using PatternCipher.Presentation.Screens;

namespace PatternCipher.Presentation.Managers
{
    /// <summary>
    /// This component acts as the main controller for the entire UI system.
    /// It takes requests to show or hide specific screens (e.g., Main Menu, Settings)
    /// and manages the screen stack and transitions.
    /// </summary>
    /// <remarks>
    /// Implements the Singleton pattern for global access. Manages a stack of screens for navigation.
    /// Handles fade-in/out transitions between screens.
    /// Listens to high-level game events to trigger UI changes automatically.
    /// </remarks>
    public class UIManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance of the UIManager.
        /// </summary>
        public static UIManager Instance { get; private set; }

        [Header("Screen Prefabs")]
        [Tooltip("A list of all screen prefabs that the UIManager can instantiate.")]
        [SerializeField] private List<BaseScreen> screenPrefabs;

        [Header("Transition Settings")]
        [Tooltip("The CanvasGroup used to fade the screen in and out.")]
        [SerializeField] private CanvasGroup faderCanvasGroup;
        [Tooltip("The duration of the fade transition in seconds.")]
        [SerializeField] private float fadeDuration = 0.3f;

        private Stack<BaseScreen> screenStack = new Stack<BaseScreen>();
        private Dictionary<Type, BaseScreen> activeScreens = new Dictionary<Type, BaseScreen>();
        
        // A placeholder for a GameEvent class from another layer
        public class GameEvent { }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeScreens();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeScreens()
        {
            if (screenPrefabs == null) return;

            foreach (var prefab in screenPrefabs)
            {
                if (prefab != null)
                {
                    var screenInstance = Instantiate(prefab, transform, false);
                    var screenComponent = screenInstance.GetComponent<BaseScreen>();
                    if (screenComponent != null)
                    {
                        Type screenType = screenComponent.GetType();
                        if (!activeScreens.ContainsKey(screenType))
                        {
                            activeScreens.Add(screenType, screenComponent);
                            screenComponent.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Instantiates and displays a screen of a specific type, pushing it onto the screen stack.
        /// </summary>
        /// <typeparam name="T">The type of the screen to show, must inherit from BaseScreen.</typeparam>
        /// <param name="data">Optional data to pass to the screen's OnShow method.</param>
        public void ShowScreen<T>(object data = null) where T : BaseScreen
        {
            Type screenType = typeof(T);
            if (!activeScreens.ContainsKey(screenType))
            {
                Debug.LogError($"[UIManager] Screen of type {screenType.Name} not found in prefabs list.");
                return;
            }

            BaseScreen screenToShow = activeScreens[screenType];

            faderCanvasGroup.DOFade(1, fadeDuration).OnComplete(() =>
            {
                if (screenStack.Count > 0)
                {
                    screenStack.Peek().Hide();
                }

                screenStack.Push(screenToShow);
                screenToShow.Show(data);
                faderCanvasGroup.DOFade(0, fadeDuration);
            });
        }
        
        /// <summary>
        /// Hides the currently active screen and reveals the previous one on the stack.
        /// </summary>
        public void HideCurrentScreen()
        {
            if (screenStack.Count <= 0) return;

            faderCanvasGroup.DOFade(1, fadeDuration).OnComplete(() =>
            {
                BaseScreen screenToHide = screenStack.Pop();
                screenToHide.Hide();

                if (screenStack.Count > 0)
                {
                    BaseScreen nextScreen = screenStack.Peek();
                    nextScreen.Show(); 
                }
                
                faderCanvasGroup.DOFade(0, fadeDuration);
            });
        }
        
        /// <summary>
        /// Hides a specific screen type. This is generally used for non-stacked "pop-up" style screens.
        /// </summary>
        public void HideScreen<T>() where T : BaseScreen
        {
            Type screenType = typeof(T);
            if (activeScreens.ContainsKey(screenType))
            {
                 activeScreens[screenType].Hide();
            }
        }

        /// <summary>
        /// Shows a screen without affecting the main navigation stack (e.g., a modal dialog).
        /// </summary>
        public void ShowPopup<T>(object data = null) where T : BaseScreen
        {
            Type screenType = typeof(T);
            if (activeScreens.ContainsKey(screenType))
            {
                 activeScreens[screenType].Show(data);
            }
        }

        /// <summary>
        /// Placeholder for handling game-wide events to trigger UI changes.
        /// </summary>
        /// <param name="gameEvent">The event that occurred.</param>
        private void HandleGameEvent(GameEvent gameEvent)
        {
            // Example:
            // if (gameEvent is LevelCompletedEvent)
            // {
            //     ShowScreen<LevelCompleteScreen>( (gameEvent as LevelCompletedEvent).Data );
            // }
        }
    }
}