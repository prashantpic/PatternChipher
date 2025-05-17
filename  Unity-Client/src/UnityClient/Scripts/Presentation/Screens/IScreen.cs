namespace PatternCipher.Client.Presentation.Screens
{
    public interface IScreen
    {
        void Initialize(ScreenManager manager, object data = null);
        void Show();
        void Hide();
        // UpdateView might not be in the spec from initial prompt, but IScreen.cs section says: Show(), Hide(), UpdateView()
        // Keeping UpdateView as per specific IScreen.cs instruction
        void UpdateView(); 
        void OnScreenBecameActive();
        void OnScreenBecameInactive();
    }
}