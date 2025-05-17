using UnityEngine;
// Assuming IScreenNavigator exists in PatternCipher.UI.Coordinator.Navigation namespace
// namespace PatternCipher.UI.Coordinator.Navigation { public interface IScreenNavigator { Task GoBackAsync(); bool CanGoBack { get; } } }

namespace PatternCipher.UI.Coordinator.Layout
{
    public class PlatformLayoutAdjuster
    {
        // This method would typically be called in an Update loop or via a dedicated input handling system.
        public bool HandleAndroidBackButton(PatternCipher.UI.Coordinator.Navigation.IScreenNavigator screenNavigator)
        {
#if UNITY_ANDROID
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (screenNavigator != null && screenNavigator.CanGoBack) // Assuming CanGoBack property
                {
                    _ = screenNavigator.GoBackAsync(); // Fire and forget or await if context allows
                    return true; // Back button handled
                }
                // Optional: if cannot go back, quit application or show a confirmation
                // else { Application.Quit(); return true; } 
            }
#endif
            return false; // Back button not handled or not Android
        }
    }
}