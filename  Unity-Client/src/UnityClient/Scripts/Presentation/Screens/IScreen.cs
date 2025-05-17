namespace PatternCipher.Client.Presentation.Screens
{
    public interface IScreen
    {
        void Show();
        void Hide();
        void UpdateView(); // As per instruction; often, data is passed to Show or specific update methods.
        // Consider adding Initialize(ScreenManager manager, object data = null) as per detailed SDS
        // void Initialize(ScreenManager manager, object data = null);
        // Consider adding OnScreenBecameActive / OnScreenBecameInactive as per detailed SDS
        // void OnScreenBecameActive();
        // void OnScreenBecameInactive();
    }
}