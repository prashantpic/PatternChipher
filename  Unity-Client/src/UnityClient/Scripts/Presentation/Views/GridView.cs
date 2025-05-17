using PatternCipher.Client.Domain.Aggregates; // For GridAggregate
using PatternCipher.Client.Domain.Entities; // For Tile
using PatternCipher.Client.Domain.ValueObjects; // For GridPosition, GridDimensions
using PatternCipher.Client.Presentation.Assets; // For SymbolDatabaseSO
using System.Collections.Generic;
using UnityEngine;

namespace PatternCipher.Client.Presentation.Views
{
    public class GridView : MonoBehaviour
    {
        [SerializeField]
        private TileView tileViewPrefab;

        [SerializeField]
        private Transform tilesContainer; // Parent object for tile views

        [Header("Grid Layout Settings")]
        [SerializeField] private float cellWidth = 1.0f;
        [SerializeField] private float cellHeight = 1.0f;
        [SerializeField] private Vector2 gridOffset = Vector2.zero;


        private Dictionary<GridPosition, TileView> _tileViews;
        private SymbolDatabaseSO _symbolDatabase; // To get symbol sprites
        private GridDimensions _currentDimensions;

        // Optional: Object Pool for TileViews
        // private ObjectPool<TileView> _tileViewPool;

        protected virtual void Awake()
        {
            _tileViews = new Dictionary<GridPosition, TileView>();
            if (tilesContainer == null)
            {
                tilesContainer = transform; // Default to self if not set
            }

            // Initialize Object Pool if using one
            // _tileViewPool = new ObjectPool<TileView>(
            //     createFunc: () => Instantiate(tileViewPrefab, tilesContainer),
            //     actionOnGet: (tv) => tv.OnPooled(),
            //     actionOnRelease: (tv) => tv.OnUnpooled(),
            //     actionOnDestroy: (tv) => Destroy(tv.gameObject),
            //     collectionCheck: false, defaultCapacity: 50, maxSize: 100
            // );
        }

        public void Initialize(GridAggregate gridData, SymbolDatabaseSO symbolDatabase)
        {
            if (gridData == null)
            {
                Debug.LogError("GridView: GridData cannot be null for initialization.");
                return;
            }
            if (symbolDatabase == null)
            {
                Debug.LogError("GridView: SymbolDatabaseSO cannot be null for initialization.");
                return;
            }

            _symbolDatabase = symbolDatabase;
            _currentDimensions = gridData.Dimensions;

            ClearGrid(); // Clear any existing tiles

            foreach (var tileEntity in gridData.GetAllTiles())
            {
                CreateAndSetupTileView(tileEntity);
            }
        }

        private void CreateAndSetupTileView(Tile tileEntity)
        {
            if (tileEntity == null) return;

            TileView newTileView;
            // if (_tileViewPool != null)
            // {
            //     newTileView = _tileViewPool.Get();
            // }
            // else
            // {
                newTileView = Instantiate(tileViewPrefab, tilesContainer);
            // }
            
            newTileView.gameObject.name = $"Tile_{tileEntity.Position.Row}_{tileEntity.Position.Column}";
            newTileView.Setup(tileEntity.Position);
            newTileView.transform.localPosition = GetWorldPositionFromGrid(tileEntity.Position);

            Sprite symbolSprite = _symbolDatabase.GetSymbolAsset(tileEntity.SymbolId);
            newTileView.UpdateAppearance(tileEntity, symbolSprite);
            
            _tileViews[tileEntity.Position] = newTileView;
            newTileView.AnimateSpawn(0.3f); // Optional spawn animation
        }

        public void UpdateTileView(GridPosition position, Tile tileData)
        {
            if (_tileViews.TryGetValue(position, out TileView tileView))
            {
                if (tileData == null || string.IsNullOrEmpty(tileData.SymbolId)) // Tile might be "empty" after a match
                {
                    // Handle empty tile - e.g., return to pool or hide
                    // if (_tileViewPool != null) _tileViewPool.Release(tileView);
                    // else Destroy(tileView.gameObject);
                    tileView.OnUnpooled(); // if it hides itself
                    _tileViews.Remove(position);
                    return;
                }

                Sprite symbolSprite = _symbolDatabase.GetSymbolAsset(tileData.SymbolId);
                tileView.UpdateAppearance(tileData, symbolSprite);
            }
            else if (tileData != null && !string.IsNullOrEmpty(tileData.SymbolId)) // Tile exists in domain but not in view (e.g. new tile from cascade)
            {
                 CreateAndSetupTileView(tileData);
            }
        }
        
        public TileView GetTileViewAt(GridPosition pos)
        {
            _tileViews.TryGetValue(pos, out TileView tileView);
            return tileView;
        }

        public Vector3 GetWorldPositionFromGrid(GridPosition gridPosition)
        {
            // Adjust for centered grid if needed
            float xOffset = (_currentDimensions.Columns - 1) * cellWidth * 0.5f;
            float yOffset = (_currentDimensions.Rows - 1) * cellHeight * 0.5f;

            return new Vector3(
                (gridPosition.Column * cellWidth) - xOffset + gridOffset.x,
                (gridPosition.Row * cellHeight) - yOffset + gridOffset.y, // Unity's Y is typically up
                0
            );
        }

        public GridPosition? GetGridPositionFromWorld(Vector3 worldPosition)
        {
            if (_currentDimensions.Rows == 0 || _currentDimensions.Columns == 0) return null;

            // Convert world position to local position relative to the grid's origin/anchor
            Vector3 localPosition = tilesContainer.InverseTransformPoint(worldPosition);
            
            float xOffset = (_currentDimensions.Columns - 1) * cellWidth * 0.5f;
            float yOffset = (_currentDimensions.Rows - 1) * cellHeight * 0.5f;

            int col = Mathf.RoundToInt((localPosition.x + xOffset - gridOffset.x) / cellWidth);
            int row = Mathf.RoundToInt((localPosition.y + yOffset - gridOffset.y) / cellHeight);

            var gridPos = new GridPosition(row, col);
            if (_currentDimensions.IsValidPosition(gridPos))
            {
                return gridPos;
            }
            return null;
        }


        public void ClearGrid()
        {
            foreach (var tileView in _tileViews.Values)
            {
                // if (_tileViewPool != null)
                // {
                //     _tileViewPool.Release(tileView);
                // }
                // else
                // {
                    Destroy(tileView.gameObject);
                // }
            }
            _tileViews.Clear();
        }
    }
}