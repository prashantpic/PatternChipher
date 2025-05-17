using UnityEngine.UIElements;
using DG.Tweening; // Requires DOTweenPro
using UnityEngine; // Required for Debug

namespace PatternCipher.UI.Utils
{
    /// <summary>
    /// Utility class providing helper methods for common UI animations, primarily using DOTween Pro.
    /// These methods aim to create 'juicy' UI effects (REQ-6-007) while also respecting 
    /// reduced motion accessibility settings (REQ-UIX-013.4).
    /// </summary>
    public static class UIAnimationUtils
    {
        private const float ReducedMotionStandardDuration = 0.05f; // Very short duration for reduced motion

        /// <summary>
        /// Applies a punch scale animation to a VisualElement.
        /// If reduced motion is respected and enabled, a minimal or no animation is played.
        /// </summary>
        /// <param name="element">The VisualElement to animate.</param>
        /// <param name="strength">The strength of the punch effect (e.g., 0.2f for 20% punch).</param>
        /// <param name="duration">The duration of the animation.</param>
        /// <param name="respectReducedMotion">Whether to check and apply reduced motion settings.</param>
        /// <param name="reducedMotionEnabled">The current state of the reduced motion setting. Only considered if respectReducedMotion is true.</param>
        /// <returns>A DOTween Sequence for the animation.</returns>
        public static Sequence PunchScale(
            VisualElement element,
            float strength,
            float duration,
            bool respectReducedMotion,
            bool reducedMotionEnabled)
        {
            if (element == null)
            {
                Debug.LogWarning("[UIAnimationUtils.PunchScale] Target element is null.");
                return DOTween.Sequence(); // Return an empty sequence
            }

            if (respectReducedMotion && reducedMotionEnabled)
            {
                // For reduced motion, perhaps a very quick, subtle scale or nothing.
                // Or simply ensure it's at its target scale.
                element.style.scale = new Scale(Vector2.one); // Ensure base scale
                Sequence reducedSequence = DOTween.Sequence();
                // Optionally, a very quick fade or alpha blink if appropriate instead of scale
                // reducedSequence.Append(element.DOFade(0.9f, ReducedMotionStandardDuration / 2).SetEase(Ease.OutQuad))
                //              .Append(element.DOFade(1f, ReducedMotionStandardDuration / 2).SetEase(Ease.InQuad));
                return reducedSequence.Play();
            }

            Vector3 punch = new Vector3(strength, strength, 0); // UI Toolkit scale is 2D but DOTween punch uses Vector3
            // Note: VisualElement doesn't have a direct .DOPunchScale(). We animate its `transform.scale`.
            // However, UI Toolkit's `transform.scale` is not directly equivalent to RectTransform's scale.
            // We use `style.scale` for UI Toolkit.
            // DOTween might not directly support `style.scale` out of the box for VisualElements.
            // A common approach is to use a generic tweener or create custom extension.
            // For simplicity, let's assume a conceptual punch or use available UGUI-like methods if a DOTween VisualElement bridge exists.
            // If direct DOTween for VE style.scale is not available, this needs a custom implementation or workaround.

            // Workaround: Animate scale up then back down.
            Sequence sequence = DOTween.Sequence();
            Vector2 originalScale = element.style.scale.value.value; // Assuming it starts at some scale (usually 1,1)
                                                                     // If not set, it might be (0,0) if it's using resolved style, be careful.
            if(originalScale == Vector2.zero) originalScale = Vector2.one; // default if not explicitly set

            Vector2 targetPunchScale = new Vector2(originalScale.x * (1 + strength), originalScale.y * (1 + strength));

            sequence.Append(DOTween.To(() => element.style.scale.value.value,
                                     s => element.style.scale = new Scale(s),
                                     targetPunchScale,
                                     duration * 0.5f).SetEase(Ease.OutQuad));
            sequence.Append(DOTween.To(() => element.style.scale.value.value,
                                     s => element.style.scale = new Scale(s),
                                     originalScale,
                                     duration * 0.5f).SetEase(Ease.InQuad));
            
            return sequence.Play();
        }

        /// <summary>
        /// Fades in a VisualElement (animates its opacity from 0 to 1).
        /// If reduced motion is respected and enabled, the element is made visible instantly.
        /// </summary>
        /// <param name="element">The VisualElement to animate.</param>
        /// <param name="duration">The duration of the fade animation.</param>
        /// <param name="respectReducedMotion">Whether to check and apply reduced motion settings.</param>
        /// <param name="reducedMotionEnabled">The current state of the reduced motion setting. Only considered if respectReducedMotion is true.</param>
        /// <returns>A DOTween Tweener for the animation.</returns>
        public static Tweener FadeIn(
            VisualElement element,
            float duration,
            bool respectReducedMotion,
            bool reducedMotionEnabled)
        {
            if (element == null)
            {
                Debug.LogWarning("[UIAnimationUtils.FadeIn] Target element is null.");
                return null; 
            }

            if (respectReducedMotion && reducedMotionEnabled)
            {
                element.style.opacity = 1f;
                element.style.visibility = Visibility.Visible; // Ensure visible
                // Return a completed tweener
                return DOTween.To(() => element.style.opacity.value, x => element.style.opacity = x, 1f, ReducedMotionStandardDuration).Complete().Play();
            }
            
            element.style.opacity = 0f; // Start from transparent
            element.style.visibility = Visibility.Visible; // Make sure it's not display:none

            // DOTween can tween generic float properties
            return DOTween.To(() => element.style.opacity.value, x => element.style.opacity = x, 1f, duration)
                .SetEase(Ease.Linear) // Or any other ease
                .Play();
        }

        /// <summary>
        /// Fades out a VisualElement (animates its opacity from 1 to 0).
        /// If reduced motion is respected and enabled, the element is made invisible instantly.
        /// </summary>
        /// <param name="element">The VisualElement to animate.</param>
        /// <param name="duration">The duration of the fade animation.</param>
        /// <param name="respectReducedMotion">Whether to check and apply reduced motion settings.</param>
        /// <param name="reducedMotionEnabled">The current state of the reduced motion setting.</param>
        /// <param name="hideWhenDone">Set element's visibility to Hidden when fade is complete.</param>
        /// <returns>A DOTween Tweener for the animation.</returns>
        public static Tweener FadeOut(
            VisualElement element,
            float duration,
            bool respectReducedMotion,
            bool reducedMotionEnabled,
            bool hideWhenDone = true)
        {
            if (element == null)
            {
                Debug.LogWarning("[UIAnimationUtils.FadeOut] Target element is null.");
                return null;
            }

            if (respectReducedMotion && reducedMotionEnabled)
            {
                element.style.opacity = 0f;
                if(hideWhenDone) element.style.visibility = Visibility.Hidden;
                return DOTween.To(() => element.style.opacity.value, x => element.style.opacity = x, 0f, ReducedMotionStandardDuration).Complete().Play();
            }
            
            element.style.visibility = Visibility.Visible; // Make sure it's visible to start fading

            return DOTween.To(() => element.style.opacity.value, x => element.style.opacity = x, 0f, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => {
                    if (hideWhenDone) element.style.visibility = Visibility.Hidden;
                })
                .Play();
        }
    }
}