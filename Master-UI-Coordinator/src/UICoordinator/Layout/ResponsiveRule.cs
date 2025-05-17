using UnityEngine;
using System.Collections.Generic;
using System;

namespace PatternCipher.UI.Coordinator.Layout
{
    [Serializable]
    public class ResponsiveRule
    {
        public string RuleName;
        public float MinScreenWidth;
        public float MaxScreenWidth = float.MaxValue;
        public float MinScreenHeight;
        public float MaxScreenHeight = float.MaxValue;
        public float MinAspectRatio;
        public float MaxAspectRatio = float.MaxValue;
        public ScreenOrientation? Orientation; // Nullable for 'any' orientation
        public Dictionary<string, object> LayoutSettings = new Dictionary<string, object>();

        public bool IsMatch(float screenWidth, float screenHeight, ScreenOrientation currentOrientation)
        {
            if (screenWidth < MinScreenWidth || screenWidth > MaxScreenWidth)
                return false;
            if (screenHeight < MinScreenHeight || screenHeight > MaxScreenHeight)
                return false;

            float aspectRatio = screenWidth / screenHeight;
            if (aspectRatio < MinAspectRatio || aspectRatio > MaxAspectRatio)
                return false;

            if (Orientation.HasValue && Orientation.Value != currentOrientation)
                return false;

            return true;
        }
    }
}