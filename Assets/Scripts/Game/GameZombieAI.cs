using FishNet.Object;
using UnityEngine;

namespace Game
{
    public class GameZombieAI : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 2.5f;
        [SerializeField] private float detectionRange = 10f;
        [SerializeField] private float updateTargetInterval = 0.25f;
        [SerializeField] private Rigidbody2D rb;

        private Transform target;
        private float targetTimer;

        public override void OnStartServer()
        {
            base.OnStartServer();
            FindNearestPlayer();
        }

        private void Start()
        {
            FindNearestPlayer();
        }

        private void FixedUpdate()
        {
            if (!IsServerInitialized)
                return;

            targetTimer -= Time.fixedDeltaTime;

            if (targetTimer <= 0f)
            {
                targetTimer = updateTargetInterval;
                FindNearestPlayer();
            }

            if (target == null)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 direction =
                (Vector2)target.position - rb.position;

            if (direction.sqrMagnitude > detectionRange * detectionRange)
            {
                rb.linearVelocity = Vector2.zero;
                target = null;
                return;
            }

            direction = direction.normalized;
            rb.linearVelocity = direction * moveSpeed;
        }

        private void FindNearestPlayer()
        {
            Transform nearest = null;
            float nearestDistance = detectionRange * detectionRange;

            foreach (GameObject player in GamePlayerSpawner.Instance.Players)
            {
                if (player == null || !player.activeInHierarchy)
                {
                    continue;
                }

                float distance = ((Vector2)player.transform.position - rb.position)
                    .sqrMagnitude;

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = player.transform;
                }
            }

            target = nearest;
        }
    }

}