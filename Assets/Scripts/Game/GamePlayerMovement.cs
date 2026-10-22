using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class GamePlayerMovement : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 5;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private InputActionReference moveAction;

        private Vector2 _serverInput;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsOwner)
                moveAction.action.Enable();
        }

        private void Update()
        {
            if (!IsOwner)
                return;

            Vector2 input = moveAction.action.ReadValue<Vector2>();
            input = Vector2.ClampMagnitude(input, 1f);

            SendMoveInput(input);
        }

        [ServerRpc]
        private void SendMoveInput(Vector2 input)
        {
            _serverInput = Vector2.ClampMagnitude(input, 1f);
        }

        private void FixedUpdate()
        {
            if (!IsServerInitialized)
                return;

            _rigidbody.linearVelocity = _serverInput * moveSpeed;
        }

        public override void OnStopClient()
        {
            if (IsOwner)
                moveAction.action.Disable();

            base.OnStopClient();
        }
    }
}