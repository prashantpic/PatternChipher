using UnityEngine;

namespace PatternCipher.Client.Presentation.Screens.Common
{
    // Define IScreen based on initial prompt's instructions for IScreen.cs
    // This would normally be in its own file: Presentation/Screens/IScreen.cs
    public interface IScreen
    {
        void Show();
        void Hide();
        void UpdateView(); // As per prompt's instruction for IScreen

        // Methods from the more detailed spec for IScreen (can be added if needed)
        // void Initialize(ScreenManager manager, object data = null);
        // void OnScreenBecameActive();
        // void OnScreenBecameInactive();
    }

    public abstract class BaseScreenController : MonoBehaviour, IScreen
    {
        [Header("Screen Configuration")]
        [SerializeField]
        protected GameObject screenRoot;

        protected ScreenManager screenManager;
        protected object screenData;

        public virtual void InitializeScreen(ScreenManager manager, object data = null)
        {
            this.screenManager = manager;
            this.screenData = data;
        }

        public virtual void Show()
        {
            if (screenRoot != null)
            {
                screenRoot.SetActive(true);
            }
            else
            {
                gameObject.SetActive(true);
            }
            OnShow();
        }

        public virtual void Hide()
        {
            OnHide();
            if (screenRoot != null)
            {
                screenRoot.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public virtual void UpdateView()
        {
            // This method is called to refresh the view, potentially with new data
            OnUpdateView();
        }

        protected virtual void OnShow()
        {
            // Template method: Called when the screen is shown.
            // Derived classes override this for specific show logic.
        }

        protected virtual void OnHide()
        {
            // Template method: Called when the screen is hidden.
            // Derived classes override this for specific hide logic.
        }

        protected virtual void OnUpdateView()
        {
            // Template method: Called when the screen's view needs to be updated.
            // Derived classes override this for specific update logic.
        }

        protected virtual void Awake()
        {
            if (screenRoot == null)
            {
                screenRoot = gameObject; // Default to this GameObject if not set
            }
        }
    }
}