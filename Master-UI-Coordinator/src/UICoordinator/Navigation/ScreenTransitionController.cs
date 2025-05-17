using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening; // Assuming DOTween is available

// Assuming IUIView and IScreenTransitionController exist
// using PatternCipher.UI.Coordinator.Interfaces;
// namespace PatternCipher.UI.Coordinator.Navigation { public interface IScreenTransitionController { Task PlayTransitionAsync(IUIView screenOut, IUIView screenIn, string transitionTypeKey = null); } }


namespace PatternCipher.UI.Coordinator.Navigation
{
    public class ScreenTransitionController : IScreenTransitionController // Inferred from SDS
    {
        private float _defaultTransitionDuration = 0.3f; // Default, can be configured
        private readonly bool _reducedMotionEnabled; // Should be updated via ThemeEngine/AccessibilitySettings

        public ScreenTransitionController(float defaultTransitionDuration = 0.3f, bool initialReducedMotion = false)
        {
            _defaultTransitionDuration = defaultTransitionDuration;
            _reducedMotionEnabled = initialReducedMotion; // This should be dynamically updated
        }
        
        // Method to update reduced motion status, called by ThemeEngine
        public void SetReducedMotion(bool isEnabled)
        {
            // This implementation of ScreenTransitionController doesn't directly use _reducedMotionEnabled field
            // for DOTween global settings. ReducedMotionHandler would do that.
            // Here, it can be used to select different types of animations.
            // For now, we'll assume transitions respect it internally.
            // Ideally, ThemeEngine updates a global state or ReducedMotionHandler that DOTween respects.
        }


        public async Task PlayTransitionAsync(PatternCipher.UI.Coordinator.Interfaces.IUIView screenOut, PatternCipher.UI.Coordinator.Interfaces.IUIView screenIn, string transitionTypeKey = null)
        {
            // Respect NFR-P-001: Ensure smooth transitions. DOTween is generally performant.
            // Reduced motion should be handled here or by individual animation calls.
            // For simplicity, we'll do a basic fade. More complex transitions can be added.

            bool useReducedMotion = _reducedMotionEnabled; 
            // Ideally, check global accessibility setting:
            // useReducedMotion = AccessibilitySettings.IsReducedMotionEnabled; (if static access)
            // Or get it from an injected service.

            CanvasGroup screenOutCanvasGroup = screenOut?.ViewTransform?.GetComponent<CanvasGroup>();
            CanvasGroup screenInCanvasGroup = screenIn?.ViewTransform?.GetComponent<CanvasGroup>();

            if (screenInCanvasGroup != null)
            {
                screenInCanvasGroup.alpha = 0f;
                screenIn.ViewTransform.gameObject.SetActive(true); // Make sure incoming screen is active for transition
            }

            Sequence sequence = DOTween.Sequence();

            if (useReducedMotion)
            {
                if (screenOutCanvasGroup != null)
                    sequence.Append(screenOutCanvasGroup.DOFade(0f, 0.01f)); // Instant for reduced motion
                if (screenInCanvasGroup != null)
                    sequence.Append(screenInCanvasGroup.DOFade(1f, 0.01f));
            }
            else
            {
                // Example: Cross-fade transition
                if (screenOutCanvasGroup != null && screenInCanvasGroup != null)
                {
                    sequence.Append(screenOutCanvasGroup.DOFade(0f, _defaultTransitionDuration));
                    sequence.Join(screenInCanvasGroup.DOFade(1f, _defaultTransitionDuration));
                }
                else if (screenOutCanvasGroup != null) // Only screenOut
                {
                    sequence.Append(screenOutCanvasGroup.DOFade(0f, _defaultTransitionDuration));
                }
                else if (screenInCanvasGroup != null) // Only screenIn
                {
                     sequence.Append(screenInCanvasGroup.DOFade(1f, _defaultTransitionDuration));
                }
                else
                {
                    // No canvas groups, instant switch
                    await Task.CompletedTask;
                    return;
                }
            }
            
            await sequence.Play().AsyncWaitForCompletion();

            if (screenOut != null && screenOut.ViewTransform != null)
            {
                 // Deactivate after transition, HideAsync on IUIView should handle this more robustly.
                 // screenOut.ViewTransform.gameObject.SetActive(false);
            }
        }
    }
}