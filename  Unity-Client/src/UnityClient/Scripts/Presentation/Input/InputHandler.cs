using UnityEngine;
using UnityEngine.InputSystem;
using PatternCipher.Client.Core.Events; // For GlobalEventBus
using PatternCipher.Client.Domain.ValueObjects; // For GridPosition

namespace PatternCipher.Client.Presentation.Input
{
    public class InputHandler : MonoBehaviour
    {
        private PlayerInput _playerInput;
        private InputAction _tapAction;
        private InputAction _dragPositionAction; // To get screen position for drag

        private IGameInputReceiver _gameInputReceiver;

        private Camera _mainCamera;

        void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            if (_playerInput == null)
            {
                Debug.LogError("PlayerInput component not found on InputHandler GameObject.");
                enabled = false;
                return;
            }

            _tapAction = _playerInput.actions["Tap"]; // Assuming "Tap" action exists in Input Actions asset
            _dragPositionAction = _playerInput.actions["DragPosition"]; // Assuming "DragPosition" action for pointer/touch position

            _mainCamera = Camera.main;
        }

        void OnEnable()
        {
            if (_tapAction != null)
            {
                _tapAction.performed += OnTapPerformed;
            }
            // For drag, we might need to monitor phase or ongoing input.
            // Simple drag could be: tap starts drag, release ends drag.
            // Or, use specific "DragStart", "DragPerformed", "DragEnd" actions if defined.
        }

        void OnDisable()
        {
            if (_tapAction != null)
            {
                _tapAction.performed -= OnTapPerformed;
            }
        }

        public void SetGameInputReceiver(IGameInputReceiver receiver)
        {
            _gameInputReceiver = receiver;
        }

        private void OnTapPerformed(InputAction.CallbackContext context)
        {
            Vector2 screenPosition = _dragPositionAction.ReadValue<Vector2>(); // Use drag position as it's likely the pointer/touch
            ProcessInput(screenPosition, InputType.Tap);
        }

        // Example for drag - this would need more complex state management
        // private GridPosition _dragStartPosition;
        // private bool _isDragging = false;
        //
        // private void OnDragStart(InputAction.CallbackContext context)
        // {
        //     _isDragging = true;
        //     Vector2 screenPosition = _dragPositionAction.ReadValue<Vector2>();
        //     GridPosition? gridPos = ScreenToGridPosition(screenPosition);
        //     if (gridPos.HasValue)
        //     {
        //         _dragStartPosition = gridPos.Value;
        //     }
        // }
        //
        // private void OnDragEnd(InputAction.CallbackContext context)
        // {
        //     if (!_isDragging) return;
        //     _isDragging = false;
        //     Vector2 screenPosition = _dragPositionAction.ReadValue<Vector2>();
        //     GridPosition? endGridPos = ScreenToGridPosition(screenPosition);
        //
        //     if (endGridPos.HasValue && _dragStartPosition != null) // Assuming GridPosition is a struct
        //     {
        //         _gameInputReceiver?.ReceiveTileDrag(_dragStartPosition, endGridPos.Value);
        //          // Or publish GlobalEventBus event
        //          // GlobalEventBus.Instance.Publish(new TileDragInputEvent(_dragStartPosition, endGridPos.Value));
        //     }
        // }


        private void ProcessInput(Vector2 screenPosition, InputType inputType)
        {
            // First, check for UI interaction using Unity's EventSystem
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                // Input is over UI, let UI handle it.
                // For touch, IsPointerOverGameObject needs fingerId for multi-touch.
                // For simplicity, assuming single touch/mouse for now.
                // if (Input.touchCount > 0 && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                //    return;
                return;
            }

            GridPosition? gridPos = ScreenToGridPosition(screenPosition);

            if (gridPos.HasValue)
            {
                if (inputType == InputType.Tap)
                {
                    _gameInputReceiver?.ReceiveTileTap(gridPos.Value);
                    // Or publish via GlobalEventBus if preferred for decoupling
                    // GlobalEventBus.Instance.Publish(new TileTapInputEvent(gridPos.Value));
                }
                // Handle other input types like drag if implemented
            }
        }

        private GridPosition? ScreenToGridPosition(Vector2 screenPosition)
        {
            Ray ray = _mainCamera.ScreenPointToRay(screenPosition);
            RaycastHit hit; // For 3D
            // RaycastHit2D hit2D = Physics2D.GetRayIntersection(_mainCamera.ScreenPointToRay(screenPosition)); // For 2D

            // This is a placeholder. Actual conversion depends on grid implementation (3D world, 2D world, UI grid).
            // If using a 3D grid with colliders on tiles:
            if (Physics.Raycast(ray, out hit, 100f)) // Adjust distance as needed
            {
                // Assuming tiles have a component that can provide GridPosition or can be identified.
                // For example, if TileView has a GridPosition property:
                var tileView = hit.collider.GetComponent<PatternCipher.Client.Presentation.Views.TileView>();
                if (tileView != null)
                {
                    // This assumes TileView has a public GridPosition property.
                    // This detail is from the original TileView.cs spec ("Needs a property or method to set its GridPosition")
                    // return tileView.Position; // Position might not be the best name, maybe GetGridPosition()
                    // For now, returning a placeholder. The actual GridPosition needs to be obtained from the TileView.
                    // Let's assume TileView.cs exposes its grid position.
                    // Example:
                    // return new GridPosition(tileView.Row, tileView.Col);
                }
            }
            // If using a GridView to convert world/screen to grid position (from GridView.cs spec):
            // var gridView = FindObjectOfType<PatternCipher.Client.Presentation.Views.GridView>();
            // if (gridView != null)
            // {
            //    Vector3 worldPoint = _mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, _mainCamera.nearClipPlane + 10f)); // adjust Z
            //    return gridView.GetGridPositionFromWorld(worldPoint);
            // }

            return null; // No valid grid position found
        }

        private enum InputType
        {
            Tap,
            DragStart,
            DragEnd
        }
    }
}