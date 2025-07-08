using UnityEngine;
using System;
using System.Collections.Generic;
using PatternCipher.Presentation.Managers;
using PatternCipher.Application; // Placeholder for Application Layer access

// Placeholder for a view component on the level item prefab
namespace PatternCipher.Presentation.Screens.LevelSelect
{
    public class LevelSelectItemView : MonoBehaviour 
    {
        public void Initialize(LevelProgressData data, Action<int> onSelectedCallback) { }
    }

    public class LevelProgressData 
    {
        public int LevelId;
        public string LevelName;
        public int StarsEarned;
        public bool IsUnlocked;
    }
}


namespace PatternCipher.Presentation.Screens.LevelSelect
{
    /// <summary>
    /// This screen acts as the hub for players to see their progression and choose a level.
    /// It dynamically generates the list of levels based on the player's save data.
    /// </summary>
    public class LevelSelectScreen : BaseScreen
    {
        [Header("UI References")]
        [Tooltip("The prefab for a single item in the level list.")]
        [SerializeField] private GameObject levelItemPrefab;
        [Tooltip("The parent transform where level items will be instantiated.")]
        [SerializeField] private Transform contentContainer;
        
        /// <summary>
        /// Overridden to populate the level list when the screen is shown.
        /// </summary>
        protected override void OnShow(object data)
        {
            base.OnShow(data);
            PopulateLevels();
        }

        /// <summary>
        /// Fetches level data and dynamically creates UI elements for each level.
        /// </summary>
        private void PopulateLevels()
        {
            // Clear any previously instantiated level items
            foreach (Transform child in contentContainer)
            {
                Destroy(child.gameObject);
            }

            // Fetch level progression data from the Application Layer's ProgressionManager
            // This is a placeholder call.
            var allLevelData = ProgressionManager.Instance.GetAllLevelProgress();

            if (allLevelData == null)
            {
                Debug.LogError("[LevelSelectScreen] Failed to get level progression data.");
                return;
            }

            foreach (var levelProgress in allLevelData)
            {
                var itemGO = Instantiate(levelItemPrefab, contentContainer);
                var itemView = itemGO.GetComponent<LevelSelectItemView>();
                if (itemView != null)
                {
                    // Initialize the item view with its data and a callback for when it's selected
                    itemView.Initialize(levelProgress, OnLevelSelected);
                }
            }
        }

        /// <summary>
        /// Callback method invoked when a level item is clicked.
        /// </summary>
        /// <param name="levelId">The ID of the selected level.</param>
        private void OnLevelSelected(int levelId)
        {
            // Tell the Application Layer's GameManager to start the selected level
            GameManager.Instance.StartLevel(levelId);
        }
    }
}