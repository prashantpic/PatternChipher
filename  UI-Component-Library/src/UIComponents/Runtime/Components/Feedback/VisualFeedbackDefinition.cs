using UnityEngine;
using UnityEngine.AddressableAssets; // Required for AssetReferenceGameObject
using PatternCipher.UI.Components.Feedback.Enums; // Assuming VisualAnimationTypeEnum will be here

namespace PatternCipher.UI.Components.Feedback
{
    /// <summary>
    /// ScriptableObject defining properties for a visual feedback event.
    /// This data container stores configurations for visual effects, such as animation types,
    /// DOTween parameters, or references to particle effect prefabs (REQ-6-007).
    /// It should support reduced motion preferences from accessibility settings (REQ-UIX-013.4).
    /// </summary>
    [CreateAssetMenu(fileName = "VisualFeedback", menuName = "PatternCipher/UI/Feedback/Visual Feedback Definition")]
    public class VisualFeedbackDefinition : ScriptableObject
    {
        [Tooltip("Type of visual animation or effect to play.")]
        public VisualAnimationTypeEnum AnimationType = VisualAnimationTypeEnum.ScalePunch;

        [Tooltip("Parameters for DOTween animations, often serialized as JSON or a custom string format. Specific to the AnimationType if it's DOTween-based.")]
        public string DOTweenParams = "{\"strength\":0.2,\"duration\":0.3}"; // Example for a punch scale

        [Tooltip("Addressable asset reference to a GameObject prefab containing a particle effect. Used if AnimationType is ParticleEffect.")]
        public AssetReferenceGameObject ParticleEffectAddress;

        [Tooltip("Default duration for the visual effect, if applicable. May be overridden by DOTweenParams or particle system settings.")]
        public float Duration = 0.5f;
    }
}