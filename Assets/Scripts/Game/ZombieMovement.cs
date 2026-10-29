using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Game
{
    public class ZombieMovement : NetworkBehaviour
    {
        [SerializeField] private float _moveSpeed = 2.5f;
        [SerializeField] private float _detectionRange = 10f;
        [SerializeField] private Rigidbody2D _rb;
        private readonly SyncVar<Transform> _target = new SyncVar<Transform>();

        private void FixedUpdate()
        {
            if (IsServerInitialized)
            {
                if (_target.Value == null)
                    FindNearestPlayer();
            }

            MoveTowardsTarget();
        }

        private void MoveTowardsTarget()
        {
            if (_target.Value == null)
            {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 direction = (Vector2)_target.Value.position - _rb.position;

            if (direction.sqrMagnitude > _detectionRange * _detectionRange)
            {
                _rb.linearVelocity = Vector2.zero;

                if (IsServerInitialized)
                    _target.Value = null;

                return;
            }

            direction = direction.normalized;
            _rb.linearVelocity = direction * _moveSpeed;
        }

        private void FindNearestPlayer()
        {
            Transform nearest = null;
            float nearestDistance = _detectionRange * _detectionRange;

            foreach (GameObject player in GamePlayerSpawner.Instance.Players)
            {
                if (player == null || !player.activeInHierarchy)
                    continue;

                float distance = ((Vector2)player.transform.position - _rb.position).sqrMagnitude;

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = player.transform;
                }
            }

            _target.Value = nearest;
        }
    }
}