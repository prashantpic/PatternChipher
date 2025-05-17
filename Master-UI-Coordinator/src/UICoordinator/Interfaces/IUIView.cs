using UnityEngine;
using System.Threading.Tasks;
// Assuming ScreenType and NavigationPayload exist in PatternCipher.UI.Coordinator.Navigation
// namespace PatternCipher.UI.Coordinator.Navigation { public enum ScreenType { None, MainMenu, GameScreen, LevelSelect } }
// namespace PatternCipher.UI.Coordinator.Navigation { public class NavigationPayload { } }


namespace PatternCipher.UI.Coordinator.Interfaces
{
    public interface IUIView
    {
        PatternCipher.UI.Coordinator.Navigation.ScreenType ScreenType { get; }
        RectTransform ViewTransform { get; } // Or GameObject if UI Toolkit is used more abstractly

        Task InitializeAsync(PatternCipher.UI.Coordinator.Navigation.NavigationPayload payload);
        Task ShowAsync();
        Task HideAsync();
        void OnNavigateTo(PatternCipher.UI.Coordinator.Navigation.NavigationPayload payload);
        void OnNavigateFrom();
        void Dispose(); // For cleanup
    }
}