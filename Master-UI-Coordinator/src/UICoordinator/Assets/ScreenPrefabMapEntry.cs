using System;
using PatternCipher.UI.Coordinator.Navigation; // Assuming ScreenType is in this namespace

namespace PatternCipher.UI.Coordinator.Assets
{
    /// <summary>
    /// Represents a single mapping entry in the UIPrefabRegistry, 
    /// associating a ScreenType with an Addressable key.
    /// Used to configure the UIPrefabRegistry ScriptableObject.
    /// </summary>
    [Serializable]
    public class ScreenPrefabMapEntry
    {
        /// <summary>
        /// The type of the screen this entry maps.
        /// </summary>
        public ScreenType ScreenType;

        /// <summary>
        /// The Addressable key used to load the prefab for this screen.
        /// </summary>
        public string AddressableKey;
    }
}