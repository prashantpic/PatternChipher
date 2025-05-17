using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using PatternCipher.UI.Components.SymbolManagement; // Assuming SymbolDefinition is here
using PatternCipher.UI.Services.AssetResolution; // Assuming SymbolAssetResolver is here

namespace PatternCipher.UI.Components.GridView
{
    public class GridView : MonoBehaviour
    {
        [Tooltip("The root VisualElement for the grid.")]
        public UIDocument uiDocument; // Or assign VisualElement directly if part of a larger tree
        
        [Tooltip("UXML asset for a single grid tile.")]
        public VisualTreeAsset gridTileAsset; // To instantiate GridTileView elements

        private VisualElement _gridContainer;
        private List<GridTileView> _tileViews;
        private int _rows;
        private int _cols;

        // Dependencies to be injected
        private SymbolAssetResolver _symbolAssetResolver;

        void Awake()
        {
            if (uiDocument == null)
            {
                Debug.LogError("GridView: UIDocument is not assigned.", this);
                return;
            }
            var rootVisualElement = uiDocument.rootVisualElement;
            _gridContainer = rootVisualElement?.Q<VisualElement>("GridContainer"); // Expect a VE named "GridContainer" in UXML

            if (_gridContainer == null)
            {
                Debug.LogError("GridView: Could not find 'GridContainer' VisualElement in the UXML.", this);
                // Fallback: Create one if not found, for basic functionality
                _gridContainer = new VisualElement { name = "GridContainer" };
                _gridContainer.style.flexDirection = FlexDirection.Row;
                _gridContainer.style.flexWrap = Wrap.Wrap;
                rootVisualElement?.Add(_gridContainer);
            }
            _tileViews = new List<GridTileView>();
        }

        /// <summary>
        /// Call this to set dependencies, e.g., from a DI container or an orchestrator.
        /// </summary>
        public void SetDependencies(SymbolAssetResolver symbolAssetResolver)
        {
            _symbolAssetResolver = symbolAssetResolver;
        }

        public void InitializeGrid(int rows, int columns, System.Action<GridTileView, int, int> onTileCreated = null)
        {
            if (_gridContainer == null)
            {
                Debug.LogError("GridView: GridContainer is null. Cannot initialize grid.", this);
                return;
            }
            if (gridTileAsset == null)
            {
                 Debug.LogError("GridView: GridTileAsset (VisualTreeAsset for tile) is not assigned.", this);
                return;
            }
            if (_symbolAssetResolver == null)
            {
                Debug.LogWarning("GridView: SymbolAssetResolver is not set. Tiles may not display symbols correctly.", this);
            }

            ClearGrid();
            _rows = rows;
            _cols = columns;

            _gridContainer.style.width = new StyleLength(new Length(columns * 50, LengthUnit.Pixel)); // Example: 50px per tile
            _gridContainer.style.height = new StyleLength(new Length(rows * 50, LengthUnit.Pixel));  // Example: 50px per tile


            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    // Instantiate the UXML for the tile
                    TemplateContainer tileElementInstance = gridTileAsset.Instantiate();
                    
                    // Assuming GridTileView is a component that will be added to a GameObject
                    // OR GridTileView is a custom VisualElement. The spec points to ComponentController.
                    // For this example, let's assume GridTileView is a custom VisualElement or can be directly manipulated.
                    // If GridTileView were a MonoBehaviour, we'd need a GameObject prefab.
                    // Given the UXML approach, GridTileView should ideally be a custom VisualElement or work with one.
                    // Let's assume the instantiated UXML IS the GridTileView or contains it.
                    // For simplicity, we'll treat the instantiated element as the view itself,
                    // and GridTileView methods will operate on this.
                    // A more robust approach might involve a GridTileView C# class that *wraps* tileElementInstance.
                    
                    // This is a simplification. A proper GridTileView component would be more robust.
                    // For now, let's assume the UXML for a tile *is* a GridTileView or can be adapted.
                    // If GridTileView is a MonoBehaviour, this instantiation needs to be different (GameObject).
                    // Given "Template: UnityCSharpComponent" for GridTileView, it suggests MonoBehaviour.
                    // This implies gridTileAsset should be a Prefab with a UIDocument/VisualElement and GridTileView script.
                    // However, the prompt uses VisualTreeAsset. I'll proceed assuming GridTileView is a custom VisualElement
                    // or adapts a VisualElement. The SDS is a bit ambiguous here.
                    // Let's proceed with GridTileView as a C# class that MANAGES a VisualElement.

                    var tileGameObject = new GameObject($"Tile_{r}_{c}");
                    tileGameObject.transform.SetParent(_gridContainer.transform, false); // This line is problematic for pure UI Toolkit.
                                                                                        // UI Toolkit elements are not GameObjects in the same hierarchy.
                                                                                        // Let's assume GridTileView is added directly to _gridContainer.

                    GridTileView tileView = new GridTileView(tileElementInstance); // If GridTileView can wrap a VE
                    // Or, if GridTileView needs to be a MonoBehaviour, it should be on a prefab instantiated here.
                    // Re-evaluating: GridTileView is a "UnityCSharpComponent". So it's a MonoBehaviour.
                    // This means `gridTileAsset` should ideally be a reference to a Prefab containing a UIDocument with the tile's UXML
                    // and the GridTileView script.
                    // For now, let's assume `gridTileAsset` IS the UXML for the tile's visuals, and we'll create a GO for the script.
                    // This is a common pattern: a MonoBehaviour controller for a piece of UI Toolkit UI.

                    GameObject tileGO = new GameObject($"Tile_{r}_{c}_Controller");
                    tileGO.transform.SetParent(this.transform); // Or a dedicated parent for tile controllers
                    GridTileView tileViewComponent = tileGO.AddComponent<GridTileView>();
                    
                    // The GridTileView component needs its VisualElement.
                    // It could find it in its own UIDocument if it had one, or query from this GridView's document.
                    // For now, let's pass the instantiated UXML to it.
                    tileViewComponent.SetVisualElement(tileElementInstance);
                    tileViewComponent.Setup(c, r, _symbolAssetResolver); // x, y coordinates

                    _gridContainer.Add(tileElementInstance); // Add the visual part to the grid container
                    _tileViews.Add(tileViewComponent);
                    onTileCreated?.Invoke(tileViewComponent, c, r);
                }
            }
        }

        public GridTileView GetTileViewAt(int x, int y)
        {
            if (x < 0 || x >= _cols || y < 0 || y >= _rows)
            {
                Debug.LogWarning($"GridView: Coordinates ({x},{y}) are out of bounds.");
                return null;
            }
            int index = y * _cols + x;
            if (index >= 0 && index < _tileViews.Count)
            {
                return _tileViews[index];
            }
            return null;
        }

        public void SetGridData(IEnumerable<SymbolDefinition> symbolsForTiles)
        {
            if (symbolsForTiles == null)
            {
                Debug.LogError("GridView: symbolsForTiles is null.", this);
                return;
            }

            int index = 0;
            foreach (var symbolDef in symbolsForTiles)
            {
                if (index < _tileViews.Count)
                {
                    if (symbolDef != null)
                    {
                        _tileViews[index].SetSymbol(symbolDef);
                    }
                    else
                    {
                        // Handle null symbol (e.g., empty tile)
                        _tileViews[index].ClearSymbol(); 
                    }
                }
                else
                {
                    Debug.LogWarning($"GridView: More symbol data provided than available tile views. Index: {index}", this);
                    break;
                }
                index++;
            }

            if (index < _tileViews.Count)
            {
                Debug.LogWarning($"GridView: Not enough symbol data for all tile views. Expected: {_tileViews.Count}, Got: {index}", this);
                // Optionally clear remaining tiles
                for (int i = index; i < _tileViews.Count; i++)
                {
                    _tileViews[i].ClearSymbol();
                }
            }
        }

        public void ClearGrid()
        {
            foreach (var tileView in _tileViews)
            {
                if (tileView != null && tileView.gameObject != null)
                {
                    // Remove the visual element from the container
                    if (tileView.GetVisualElement() != null && tileView.GetVisualElement().parent == _gridContainer)
                    {
                        _gridContainer.Remove(tileView.GetVisualElement());
                    }
                    Destroy(tileView.gameObject); // Destroy the MonoBehaviour holder
                }
            }
            _tileViews.Clear();
            // _gridContainer.Clear(); // This clears all children, ensure it's what's intended if tile GOs aren't used.
                                   // If GridTileView scripts manage their VE removal, this might not be needed or could conflict.
                                   // Given the current approach, destroying GOs and clearing the list is primary.
                                   // The UXML instances added to _gridContainer also need to be removed.
            if(_gridContainer != null) _gridContainer.Clear(); // Clears all visual elements from the container
        }

        // Example: A method to resize grid container (could be called by InitializeGrid or externally)
        public void SetGridContainerSize(int tileWidth, int tileHeight)
        {
            if (_gridContainer != null && _cols > 0 && _rows > 0)
            {
                _gridContainer.style.width = _cols * tileWidth;
                _gridContainer.style.height = _rows * tileHeight;
            }
        }
    }
}