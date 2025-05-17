using UnityEngine;
using System.Collections.Generic;

namespace PatternCipher.UI.Coordinator.Layout
{
    [CreateAssetMenu(fileName = "ResponsiveProfile", menuName = "PatternCipher/UI Coordinator/Responsive Profile")]
    public class ResponsiveProfile : ScriptableObject
    {
        public string ProfileName;
        public List<ResponsiveRule> Rules = new List<ResponsiveRule>();
    }
}