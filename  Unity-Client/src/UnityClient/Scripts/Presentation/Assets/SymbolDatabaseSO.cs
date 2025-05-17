using UnityEngine;
using System.Collections.Generic; // Required for List

namespace PatternCipher.Client.Presentation.Assets
{
    [System.Serializable]
    public class SymbolDefinition
    {
        public string SymbolId; // Unique identifier (e.g., "ColorRed", "ShapeSquare_TypeA")
        public Sprite SymbolSprite;
        public Color SymbolColorTint = Color.white; // Optional tint
        // Add any other presentation-relevant properties like particle effects on match, specific sounds, etc.
        // public GameObject MatchParticlePrefab;
        // public AudioClip MatchSound;
    }

    [CreateAssetMenu(fileName = "SymbolDatabase", menuName = "PatternCipher/Assets/Symbol Database")]
    public class SymbolDatabaseSO : ScriptableObject
    {
        [Header("Symbol Definitions")]
        public List<SymbolDefinition> AllSymbols;

        private Dictionary<string, SymbolDefinition> symbolLookup;

        private void OnEnable()
        {
            // Initialize lookup dictionary for faster access
            // This is better than doing it in OnValidate as OnValidate is editor-only
            // and OnEnable runs at runtime when the SO is loaded.
            InitializeLookup();
        }

        private void InitializeLookup()
        {
            if (AllSymbols == null) return;

            symbolLookup = new Dictionary<string, SymbolDefinition>();
            foreach (var symbolDef in AllSymbols)
            {
                if (symbolDef != null && !string.IsNullOrEmpty(symbolDef.SymbolId))
                {
                    if (!symbolLookup.ContainsKey(symbolDef.SymbolId))
                    {
                        symbolLookup.Add(symbolDef.SymbolId, symbolDef);
                    }
                    else
                    {
                        Debug.LogWarning($"Duplicate SymbolId '{symbolDef.SymbolId}' found in SymbolDatabaseSO. Ignoring duplicate.", this);
                    }
                }
            }
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            // Can also call InitializeLookup here for editor-time updates if needed,
            // but be mindful of performance with large lists.
            // InitializeLookup(); 
        }
        #endif


        /// <summary>
        /// Retrieves the Sprite asset for a given symbol ID.
        /// </summary>
        /// <param name="symbolId">The unique ID of the symbol.</param>
        /// <returns>The Sprite for the symbol, or null if not found.</returns>
        public Sprite GetSymbolSprite(string symbolId)
        {
            if (symbolLookup == null || symbolLookup.Count != AllSymbols.Count)
            {
                 // Ensure lookup is initialized, especially if SO was modified without OnEnable being called (e.g. hot reload)
                InitializeLookup();
            }

            if (symbolLookup != null && symbolLookup.TryGetValue(symbolId, out SymbolDefinition def))
            {
                return def.SymbolSprite;
            }
            Debug.LogWarning($"Symbol Sprite for ID '{symbolId}' not found in SymbolDatabaseSO.", this);
            return null;
        }

        /// <summary>
        /// Retrieves the full SymbolDefinition for a given symbol ID.
        /// </summary>
        /// <param name="symbolId">The unique ID of the symbol.</param>
        /// <returns>The SymbolDefinition, or null if not found.</returns>
        public SymbolDefinition GetSymbolDefinition(string symbolId)
        {
            if (symbolLookup == null || symbolLookup.Count != AllSymbols.Count)
            {
                InitializeLookup();
            }
            
            if (symbolLookup != null && symbolLookup.TryGetValue(symbolId, out SymbolDefinition def))
            {
                return def;
            }
            Debug.LogWarning($"Symbol Definition for ID '{symbolId}' not found in SymbolDatabaseSO.", this);
            return null;
        }
    }
}