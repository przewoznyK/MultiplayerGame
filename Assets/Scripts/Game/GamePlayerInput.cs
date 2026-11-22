using FishNet;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class GamePlayerInput : NetworkBehaviour
    {
        [SerializeField] private InputActionReference _lookAction;
        [SerializeField] private InputActionReference _moveAction;
        [SerializeField] private InputActionReference _primaryAction;
        [SerializeField] private InputActionReference _secondaryAction;
        [SerializeField] private InputActionReference _slotChangeAction;
        [SerializeField] private InputActionReference _scrollAction;
        [SerializeField] private InputActionReference _dropItemAction;
        [SerializeField] private InputActionReference _interactAction;

        [SerializeField] private GamePlayerMovement _gamePlayerMovement;
        [SerializeField] private GamePlayerLook _gamePlayerLook;
        [SerializeField] private GamePlayer _gamePlayer;
        [SerializeField] private GamePlayerInventory _playerInventory;
        [SerializeField] private GamePlayerInteractor _playerInteractor;

        private Vector2 _moveInput;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsOwner)
            {
                _moveAction.action.performed += OnMovePerformed;
                _moveAction.action.canceled += OnMoveCanceled;
                _primaryAction.action.started += OnPrimaryPressed;
                _primaryAction.action.canceled += OnPrimaryReleased;
                _secondaryAction.action.performed += OnSecondaryPerformed;
                _scrollAction.action.performed += OnScrollPerformed;
                _slotChangeAction.action.performed += OnSlotChangePerformed;
                _dropItemAction.action.performed += OnDropItemPerformed;
                _interactAction.action.performed += OnInteractPerformed;

                _moveAction.action.Enable();
                _lookAction.action.Enable();
                _primaryAction.action.Enable();
                _scrollAction.action.Enable();
                _slotChangeAction.action.Enable();
                _dropItemAction.action.Enable();
                _interactAction.action.Enable();

                InstanceFinder.TimeManager.OnTick += OnTick;
            }
        }

        public override void OnStopClient()
        {
            if (IsOwner)
            {
                InstanceFinder.TimeManager.OnTick -= OnTick;

                _moveAction.action.performed -= OnMovePerformed;
                _moveAction.action.canceled -= OnMoveCanceled;
                _primaryAction.action.performed -= OnPrimaryPressed;
                _primaryAction.action.canceled -= OnPrimaryReleased;
                _secondaryAction.action.performed -= OnSecondaryPerformed;
                _scrollAction.action.performed -= OnScrollPerformed;
                _slotChangeAction.action.performed -= OnSlotChangePerformed;
                _dropItemAction.action.performed -= OnDropItemPerformed;
                _interactAction.action.performed -= OnInteractPerformed;

                _moveAction.action.Disable();
                _lookAction.action.Disable();
                _primaryAction.action.Disable();
                _scrollAction.action.Disable();
                _slotChangeAction.action.Disable();
                _dropItemAction.action.Disable();
                _interactAction.action.Disable();
            }

            base.OnStopClient();
        }

        private void OnMovePerformed(InputAction.CallbackContext context) =>
            _moveInput = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);

        private void OnMoveCanceled(InputAction.CallbackContext context) =>
            _moveInput = Vector2.zero;

        private void OnPrimaryPressed(InputAction.CallbackContext _) =>
            _gamePlayer.UsePrimary();

        private void OnPrimaryReleased(InputAction.CallbackContext _) =>
            _gamePlayer.ReleasePrimary();

        private void OnSecondaryPerformed(InputAction.CallbackContext _) =>
            _gamePlayer.OnCancelPrimaryPressed();

        private void OnSlotChangePerformed(InputAction.CallbackContext ctx)
        {
            if (int.TryParse(ctx.control.name, out int slotId))
                _playerInventory.RequestChangeSlot(slotId - 1);
        }

        private void OnDropItemPerformed(InputAction.CallbackContext _) =>
            _playerInventory.RequestDropEquippedItem();

        private void OnInteractPerformed(InputAction.CallbackContext _) =>
            _playerInteractor.RequestInteract();

        private void OnScrollPerformed(InputAction.CallbackContext ctx)
        {
            float scrollValue = ctx.ReadValue<Vector2>().y;

            if (scrollValue > 0f)
                _playerInventory.OnScrollSlot(-1);
            else if (scrollValue < 0f)
                _playerInventory.OnScrollSlot(1);
        }

        private void OnTick()
        {
            if (IsOwner == false)
                return;

            _gamePlayerMovement.MovePlayer(_moveInput);
            
            Vector2 lookInput = _lookAction.action.ReadValue<Vector2>();
            _gamePlayerLook.LookPlayer(lookInput);
        }
    }
}