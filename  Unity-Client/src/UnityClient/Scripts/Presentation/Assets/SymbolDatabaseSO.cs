using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PatternCipher.Client.Presentation.Assets
{
    [CreateAssetMenu(fileName = "SymbolDatabase", menuName = "PatternCipher/Symbol Database", order = 0)]
    public class SymbolDatabaseSO : ScriptableObject
    {
        [System.Serializable]
        public class SymbolDefinition
        {
            public string symbolId; // Unique identifier (e.g., "RedGem", "BlueSquare")
            public Sprite sprite;
            public Color symbolColor = Color.white; // Optional: for tinting or procedural symbols
            // Add other presentation-relevant properties here
            // public Material material;
            // public GameObject modelPrefab; // If symbols are 3D
            public string displayName; // For UI/debug
        }

        public List<SymbolDefinition> symbolDefinitions = new List<SymbolDefinition>();

        private Dictionary<string, SymbolDefinition> _symbolMap;

        private void OnEnable()
        {
            // Initialize the dictionary for quick lookups
            // This is better than doing it on every GetSymbolAsset call
            // However, OnEnable might not be called in editor if data changes,
            // so a lazy initialization or editor script might be needed for robustness.
            InitializeMap();
        }
        
        private void InitializeMap()
        {
            if (_symbolMap == null || _symbolMap.Count != symbolDefinitions.Count)
            {
                _symbolMap = new Dictionary<string, SymbolDefinition>();
                foreach (var def in symbolDefinitions)
                {
                    if (!string.IsNullOrEmpty(def.symbolId) && !_symbolMap.ContainsKey(def.symbolId))
                    {
                        _symbolMap.Add(def.symbolId, def);
                    }
                    else
                    {
                        Debug.LogWarning($"Duplicate or empty Symbol ID found in SymbolDatabase: {def.symbolId}");
                    }
                }
            }
        }


        public SymbolDefinition GetSymbolDefinition(string symbolId)
        {
            InitializeMap(); // Ensure map is initialized
            if (_symbolMap.TryGetValue(symbolId, out SymbolDefinition definition))
            {
                return definition;
            }
            Debug.LogWarning($"Symbol ID '{symbolId}' not found in SymbolDatabase.");
            return null;
        }

        public Sprite GetSymbolSprite(string symbolId)
        {
            SymbolDefinition definition = GetSymbolDefinition(symbolId);
            return definition?.sprite;
        }
        
        // Optional: Get all available symbol IDs
        public List<string> GetAllSymbolIds()
        {
            InitializeMap();
            return _symbolMap.Keys.ToList();
        }
    }
}