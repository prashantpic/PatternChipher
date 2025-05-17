using UnityEngine;

namespace PatternCipher.UI.Coordinator.Layout
{
    public static class SafeAreaHandler
    {
        public static void ApplySafeAreaToTransform(RectTransform rectTransform, Rect safeArea)
        {
            if (rectTransform == null) return;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            
            // Reset offsets as anchors now define the safe area
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}