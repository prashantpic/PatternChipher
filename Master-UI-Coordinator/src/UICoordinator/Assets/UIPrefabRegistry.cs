using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PatternCipher.UI.Coordinator.Navigation; // Assuming ScreenType is defined here
// Assuming ScreenPrefabMapEntry is defined in PatternCipher.UI.Coordinator.Assets namespace
// e.g. in a file named ScreenPrefabMapEntry.cs with:
// namespace PatternCipher.UI.Coordinator.Assets {
//   [System.Serializable]
//   public class ScreenPrefabMapEntry {
//     public PatternCipher.UI.Coordinator.Navigation.ScreenType ScreenType;
//     public string AddressableKey;
//   }
// }


namespace PatternCipher.UI.Coordinator.Assets
{
    /// <summary>
    /// ScriptableObject mapping screen types or UI part names to their Addressable asset keys.
    /// Provides a centralized, designer-friendly mapping for Addressable keys.
    /// </summary>
    [CreateAssetMenu(fileName = "UIPrefabRegistry", menuName = "PatternCipher/UI Coordinator/UIPrefabRegistry")]
    public class UIPrefabRegistry : ScriptableObject
    {
        /// <summary>
        /// List of mappings between ScreenType and Addressable keys.
        /// </summary>
        public List<ScreenPrefabMapEntry> ScreenPrefabMappings = new List<ScreenPrefabMapEntry>();

        /// <summary>
        /// Gets the Addressable key for a given screen type.
        /// </summary>
        /// <param name="screenType">The screen type to look up.</param>
        /// <returns>The Addressable key as a string, or null if not found.</returns>
        public string GetAddressableKey(ScreenType screenType)
        {
            // Ensure ScreenPrefabMapEntry has public fields/properties:
            // public ScreenType ScreenType;
            // public string AddressableKey;
            var mapping = ScreenPrefabMappings.FirstOrDefault(m => m.ScreenType == screenType);
            if (mapping != null)
            {
                return mapping.AddressableKey;
            }

            Debug.LogWarning($"[UIPrefabRegistry] No Addressable key found for screen type: {screenType}");
            return null;
        }
    }
}