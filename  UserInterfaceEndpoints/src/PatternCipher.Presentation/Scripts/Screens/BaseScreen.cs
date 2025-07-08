using UnityEngine;

namespace PatternCipher.Presentation.Screens
{
    /// <summary>
    /// A foundational abstract class that all specific screen controllers inherit from.
    /// It provides a common interface and base functionality for all UI screens,
    /// standardizing how they are shown and hidden by the UIManager.
    /// </summary>
    public abstract class BaseScreen : MonoBehaviour
    {
        /// <summary>
        /// Shows the screen. This base implementation activates the screen's GameObject
        /// and calls the OnShow life-cycle method.
        /// </summary>
        /// <param name="data">Optional data to be passed to the screen for initialization.</param>
        public virtual void Show(object data = null)
        {
            gameObject.SetActive(true);
            OnShow(data);
        }

        /// <summary>
        /// Hides the screen. This base implementation calls the OnHide life-cycle method
        /// and then deactivates the screen's GameObject.
        /// </summary>
        public virtual void Hide()
        {
            OnHide();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Called when the screen is shown. Subclasses should override this method
        /// to implement screen-specific setup logic, like populating data.
        /// </summary>
        /// <param name="data">Data passed from the Show method.</param>
        protected virtual void OnShow(object data)
        {
            // Subclasses to override for setup logic.
        }

        /// <summary>
        /// Called when the screen is hidden. Subclasses should override this method
        /// to implement screen-specific teardown logic, like unsubscribing from events.
        /// </summary>
        protected virtual void OnHide()
        {
            // Subclasses to override for teardown logic.
        }
    }
}