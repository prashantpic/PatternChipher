using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

// Define a ScriptableObject for tutorial data for easy content management
namespace PatternCipher.Presentation.Data
{
    [CreateAssetMenu(fileName = "TutorialSectionData", menuName = "PatternCipher/Tutorial Section Data", order = 1)]
    public class TutorialSectionData : ScriptableObject
    {
        public string title;
        [TextArea(5, 10)]
        public string bodyText;
        public Sprite image;
    }
}

// Placeholder for the view component on the tutorial section prefab
namespace PatternCipher.Presentation.Screens.HowToPlay
{
    public class TutorialSectionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI bodyText;
        [SerializeField] private Image tutorialImage;

        public void Initialize(Data.TutorialSectionData data)
        {
            titleText.text = data.title;
            bodyText.text = data.bodyText;
            tutorialImage.sprite = data.image;
            tutorialImage.gameObject.SetActive(data.image != null);
        }
    }
}

namespace PatternCipher.Presentation.Screens.HowToPlay
{
    /// <summary>
    /// The controller for the screen that provides players with instructions and
    /// reference material on how to play the game.
    /// </summary>
    /// <remarks>
    /// This screen dynamically populates a scrollable area with prefab sections for each
    /// game mechanic, with data loaded from ScriptableObjects.
    /// </remarks>
    public class HowToPlayScreen : BaseScreen
    {
        [Header("UI References")]
        [SerializeField] private GameObject tutorialSectionPrefab;
        [SerializeField] private Transform contentContainer;
        
        [Header("Content Data")]
        [Tooltip("List of ScriptableObjects containing the tutorial content.")]
        [SerializeField] private List<Data.TutorialSectionData> tutorialSections;

        protected override void OnShow(object data)
        {
            base.OnShow(data);
            PopulateTutorialSections();
        }

        /// <summary>
        /// Instantiates and populates the tutorial sections from the data list.
        /// </summary>
        private void PopulateTutorialSections()
        {
            // Clear any old content first
            foreach (Transform child in contentContainer)
            {
                Destroy(child.gameObject);
            }

            if (tutorialSections == null || tutorialSections.Count == 0)
            {
                Debug.LogWarning("[HowToPlayScreen] No tutorial section data assigned.");
                return;
            }

            foreach (var sectionData in tutorialSections)
            {
                GameObject sectionGO = Instantiate(tutorialSectionPrefab, contentContainer);
                var sectionView = sectionGO.GetComponent<TutorialSectionView>();
                if (sectionView != null)
                {
                    sectionView.Initialize(sectionData);
                }
            }
        }
    }
}