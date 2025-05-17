using UnityEngine;
using UnityEngine.InputSystem;
using PatternCipher.Client.Core.Events; // For GlobalEventBus
using PatternCipher.Client.Domain.ValueObjects; // For GridPosition (assuming it exists)

namespace PatternCipher.Client.Presentation.Input
{
    // Define IGameInputReceiver based on prompt's instructions
    // This would normally be in its own file: Presentation/Input/IGameInputReceiver.cs
    public interface IGameInputReceiver
    {
        void ReceiveTileTap(GridPosition position);
        void ReceiveTileDrag(GridPosition startPosition, GridPosition endPosition);
        // Add other methods like ReceiveSwipe, etc., if needed
    }

    public class InputHandler : MonoBehaviour
    {
        private PlayerInput playerInput;
        private InputAction tapAction;
        private InputAction dragPressAction; // To detect start of drag
        private InputAction pointerPositionAction;

        private Camera mainCamera;
        private IGameInputReceiver gameInputReceiver;

        private bool isDragging = false;
        private Vector2 dragStartPositionScreen;
        // private GridPosition dragStartGridPosition; // Assuming GridPosition struct exists

        // Layer mask for raycasting, e.g., to target only game tiles
        [SerializeField] private LayerMask tileLayerMask;
        [SerializeField] private LayerMask uiLayerMask;


        void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            if (playerInput == null)
            {
                Debug.LogError("PlayerInput component not found on InputHandler.", this);
                enabled = false;
                return;
            }

            tapAction = playerInput.actions["Tap"]; // Assumes "Tap" action exists in Input Actions asset
            dragPressAction = playerInput.actions["DragPress"]; // Assumes "DragPress" action for press/release
            pointerPositionAction = playerInput.actions["PointerPosition"]; // Assumes "PointerPosition" for current pos

            mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            if (tapAction != null)
            {
                tapAction.performed += OnTapPerformed;
            }
            if (dragPressAction != null)
            {
                dragPressAction.started += OnDragStarted; // Corresponds to press
                dragPressAction.canceled += OnDragEnded;   // Corresponds to release
            }
        }

        private void OnDisable()
        {
            if (tapAction != null)
            {
                tapAction.performed -= OnTapPerformed;
            }
            if (dragPressAction != null)
            {
                dragPressAction.started -= OnDragStarted;
                dragPressAction.canceled -= OnDragEnded;
            }
        }

        public void SetGameInputReceiver(IGameInputReceiver receiver)
        {
            this.gameInputReceiver = receiver;
        }

        private void OnTapPerformed(InputAction.CallbackContext context)
        {
            if (isDragging) return; // Don't process tap if a drag was just completed

            Vector2 screenPosition = pointerPositionAction.ReadValue<Vector2>();
            
            if (IsPointerOverUI(screenPosition))
            {
                // Let UI handle its own input
                return;
            }

            TryProcessGameInput(screenPosition, isTap: true);
        }

        private void OnDragStarted(InputAction.CallbackContext context)
        {
            dragStartPositionScreen = pointerPositionAction.ReadValue<Vector2>();
             if (IsPointerOverUI(dragStartPositionScreen))
            {
                isDragging = false; // Don't start a game drag if over UI
                return;
            }
            isDragging = true;
            // Optionally, convert dragStartPositionScreen to GridPosition here if needed for immediate feedback
        }

        private void OnDragEnded(InputAction.CallbackContext context)
        {
            if (!isDragging) return;
            isDragging = false;

            Vector2 dragEndPositionScreen = pointerPositionAction.ReadValue<Vector2>();

            if (IsPointerOverUI(dragStartPositionScreen) || IsPointerOverUI(dragEndPositionScreen))
            {
                 // If drag started on UI or ended on UI, might be a UI drag action
                return;
            }
            
            // Check for minimal drag distance to differentiate from a tap
            if (Vector2.Distance(dragStartPositionScreen, dragEndPositionScreen) < GetDragThreshold())
            {
                // Treat as tap if drag distance is too small
                TryProcessGameInput(dragStartPositionScreen, isTap: true);
                return;
            }

            TryProcessGameInput(dragStartPositionScreen, dragEndPositionScreen, isTap: false);
        }
        
        private float GetDragThreshold()
        {
            // Example: 5% of screen height as threshold
            return Screen.height * 0.02f; // Make this configurable
        }


        private void TryProcessGameInput(Vector2 screenPosition, bool isTap)
        {
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, tileLayerMask))
            {
                // Assuming the hit object (tile) has a way to get its GridPosition
                // For example, a TileView component with a GridPosition property
                // This part needs to be adapted to how GridPosition is obtained from a GameObject
                var tileView = hit.collider.GetComponent<PatternCipher.Client.Presentation.Views.TileView>();
                if (tileView != null)
                {
                    GridPosition gridPos = tileView.Position; // Assuming TileView has a GridPosition property
                    if (isTap)
                    {
                        gameInputReceiver?.ReceiveTileTap(gridPos);
                        // Or: GlobalEventBus.Instance.Publish(new TileTappedEvent(gridPos));
                    }
                }
            }
        }
        
        private void TryProcessGameInput(Vector2 startScreenPos, Vector2 endScreenPos, bool isTap)
        {
             if (isTap) // This overload is for drag
            {
                TryProcessGameInput(startScreenPos, true);
                return;
            }

            Ray startRay = mainCamera.ScreenPointToRay(startScreenPos);
            Ray endRay = mainCamera.ScreenPointToRay(endScreenPos);
            RaycastHit startHit, endHit;

            GridPosition? startGridPos = null;
            GridPosition? endGridPos = null;

            if (Physics.Raycast(startRay, out startHit, 100f, tileLayerMask))
            {
                var startTileView = startHit.collider.GetComponent<PatternCipher.Client.Presentation.Views.TileView>();
                if (startTileView != null) startGridPos = startTileView.Position;
            }

            if (Physics.Raycast(endRay, out endHit, 100f, tileLayerMask))
            {
                 var endTileView = endHit.collider.GetComponent<PatternCipher.Client.Presentation.Views.TileView>();
                if (endTileView != null) endGridPos = endTileView.Position;
            }

            if (startGridPos.HasValue && endGridPos.HasValue && startGridPos.Value != endGridPos.Value)
            {
                gameInputReceiver?.ReceiveTileDrag(startGridPos.Value, endGridPos.Value);
                // Or: GlobalEventBus.Instance.Publish(new TileDragEvent(startGridPos.Value, endGridPos.Value));
            }
            else if (startGridPos.HasValue && !endGridPos.HasValue) // Dragged off a tile to empty space
            {
                // Handle if necessary, e.g. cancel selection or specific drag-off-tile action
            }
        }


        private bool IsPointerOverUI(Vector2 screenPosition)
        {
            if (UnityEngine.EventSystems.EventSystem.current == null) return false;
            
            UnityEngine.EventSystems.PointerEventData eventDataCurrentPosition = 
                new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            eventDataCurrentPosition.position = screenPosition;
            List<UnityEngine.EventSystems.RaycastResult> results = 
                new List<UnityEngine.EventSystems.RaycastResult>();
            UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            
            foreach (var result in results)
            {
                if (((1 << result.gameObject.layer) & uiLayerMask) != 0) // Check if the hit UI element is on our UI layer
                {
                    return true;
                }
            }
            return false;
        }
    }
}