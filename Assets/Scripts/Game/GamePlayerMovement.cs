using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Game
{
    public class GamePlayerMovement : NetworkBehaviour, IKnockable
    {
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private float _moveSpeed = 5;

        private Vector2 _serverInput;
        public readonly SyncVar<bool> IsKnockedBack = new SyncVar<bool>();

        private void FixedUpdate()
        {
            if (IsServerInitialized == false)
                return;

            if (IsKnockedBack.Value)
            {
                if (_rb.linearVelocity.sqrMagnitude < 0.1f)
                {
                    IsKnockedBack.Value = false;
                    _rb.linearVelocity = Vector2.zero;
                }
                else
                    return;
            }

            _rb.linearVelocity = _serverInput * _moveSpeed;
        }

        public void MovePlayer(Vector2 input)
        {
            if (IsKnockedBack.Value)
                return;

            input = Vector2.ClampMagnitude(input, 1f);
            SendMoveInput(input);
        }

        [ServerRpc]
        private void SendMoveInput(Vector2 input)
        {
            _serverInput = Vector2.ClampMagnitude(input, 1f);
        }

        public void ApplyKnockback(Vector2 direction, float force)
        {
            if (IsServerInitialized == false)
                return;

            IsKnockedBack.Value = true;
            _serverInput = Vector2.zero;
            _rb.linearVelocity = Vector2.zero;
            _rb.AddForce(direction * force, ForceMode2D.Impulse);
        }
    }
}