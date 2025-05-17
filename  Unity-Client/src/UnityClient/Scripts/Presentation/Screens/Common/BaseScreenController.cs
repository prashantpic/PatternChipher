using UnityEngine;

namespace PatternCipher.Client.Presentation.Screens.Common
{
    public abstract class BaseScreenController : MonoBehaviour, IScreen
    {
        [SerializeField]
        protected GameObject screenRoot;

        protected ScreenManager screenManager;

        protected virtual void Awake()
        {
            if (screenRoot == null)
            {
                screenRoot = this.gameObject; // Default to this game object if not set
            }
            screenManager = ScreenManager.Instance;
        }

        public virtual void Show()
        {
            if (screenRoot != null)
            {
                screenRoot.SetActive(true);
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
        }

        public virtual void UpdateView()
        {
            OnUpdateView();
        }

        protected abstract void OnShow();
        protected abstract void OnHide();
        protected abstract void OnUpdateView();

        // As per SDS for IScreen interface, added Initialize methods
        // However, the SDS for ScreenManager does not explicitly call Initialize.
        // BaseScreenController's Awake can fetch ScreenManager.Instance.
        // Data passing can be done via a Show(object data) method if needed.
        public virtual void Initialize(ScreenManager manager, object data = null)
        {
            this.screenManager = manager;
            // Process data if provided
        }

        // From IScreen interface in SDS (different from original prompt's Show(), Hide(), UpdateView() for IScreen)
        // The original SDS for IScreen had:
        // void Initialize(ScreenManager manager, object data = null)
        // void Show()
        // void Hide()
        // void OnScreenBecameActive()
        // void OnScreenBecameInactive()
        // The prompt for BaseScreenController says "Implement the IScreen methods (Show, Hide, UpdateView)"
        // I will stick to Show, Hide, UpdateView as per BaseScreenController's prompt.
        // If OnScreenBecameActive/Inactive are needed, they can be added.
        // For now, aligning with the specific instructions for BaseScreenController.
    }
}