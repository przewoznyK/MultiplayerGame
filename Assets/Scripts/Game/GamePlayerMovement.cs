using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class GamePlayerMovement : NetworkBehaviour
    {
        [SerializeField] private float _moveSpeed = 5;
        [SerializeField] private Rigidbody2D _rigidbody;
        private Vector2 _serverInput;

        public void MovePlayer(Vector2 input)
        {
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
            _rigidbody.linearVelocity = _serverInput * _moveSpeed;
        }
    }
}