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
        [SerializeField] private InputActionReference _attackAction;
        [SerializeField] private GamePlayerMovement _gamePlayerMovement;
        [SerializeField] private GamePlayerLook _gamePlayerLook;
        [SerializeField] private GamePlayerAttack _gamePlayerAttack;
        private Vector2 _moveInput;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsOwner)
            {
                _moveAction.action.performed += OnMovePerformed;
                _moveAction.action.canceled += OnMoveCanceled;
                _attackAction.action.performed += OnAttackPerformed;

                _moveAction.action.Enable();
                _lookAction.action.Enable();
                _attackAction.action.Enable();

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
                _attackAction.action.performed -= OnAttackPerformed;

                _moveAction.action.Disable();
                _lookAction.action.Disable();
                _attackAction.action.Disable();
            }

            base.OnStopClient();
        }

        private void OnMovePerformed(InputAction.CallbackContext context) =>
            _moveInput = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);

        private void OnMoveCanceled(InputAction.CallbackContext context) =>
            _moveInput = Vector2.zero;

        private void OnAttackPerformed(InputAction.CallbackContext _) =>
            _gamePlayerAttack.PlayAttackAnimation();

        private void OnTick()
        {
            if (!IsOwner)
                return;

            _gamePlayerMovement.MovePlayer(_moveInput);
            
            Vector2 lookInput = _lookAction.action.ReadValue<Vector2>();
            _gamePlayerLook.LookPlayer(lookInput);
        }
    }
}