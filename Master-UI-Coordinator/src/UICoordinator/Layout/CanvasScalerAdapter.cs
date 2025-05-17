using UnityEngine;
using UnityEngine.UI;

namespace PatternCipher.UI.Coordinator.Layout
{
    public class CanvasScalerAdapter
    {
        private readonly CanvasScaler _canvasScaler;

        public CanvasScalerAdapter(CanvasScaler canvasScaler)
        {
            _canvasScaler = canvasScaler;
            if (_canvasScaler == null)
            {
                Debug.LogError("CanvasScalerAdapter: CanvasScaler dependency is null.");
            }
        }

        public void SetScaleMode(CanvasScaler.ScaleMode mode)
        {
            if (_canvasScaler != null)
            {
                _canvasScaler.uiScaleMode = mode;
            }
        }

        public void SetReferenceResolution(Vector2 resolution)
        {
            if (_canvasScaler != null)
            {
                _canvasScaler.referenceResolution = resolution;
            }
        }

        public void SetMatchWidthOrHeight(float match)
        {
            if (_canvasScaler != null)
            {
                _canvasScaler.matchWidthOrHeight = Mathf.Clamp01(match);
            }
        }
    }
}