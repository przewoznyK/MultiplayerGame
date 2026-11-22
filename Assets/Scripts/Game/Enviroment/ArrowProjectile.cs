using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Game
{
    public class ArrowProjectile : NetworkBehaviour
    {
        public readonly SyncVar<float> Speed = new SyncVar<float>(1);
        private float _knockbackForce;
        private int _damage;
        private GamePlayer _owner;

        public void Initialize(int damage, float speed, GamePlayer owner, float knockbackForce)
        {
            _damage = damage;
            Speed.Value = speed;
            _owner = owner;
            _knockbackForce = knockbackForce;

            Invoke(nameof(DestroyArrow), 5f);
        }

        public void MoveArrow(float deltaTime)
        {
            transform.position += transform.right * Speed.Value * deltaTime;
        }

        [Server]
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject == _owner.gameObject)
                return;

            if (col.TryGetComponent(out Health enemyHealth))
                enemyHealth.TakeDamage(_damage, _owner);

            if (col.TryGetComponent(out IKnockable knockableEntity))
            {
                Vector2 knockbackDir = (col.transform.position - transform.position).normalized;
                knockableEntity.ApplyKnockback(knockbackDir, _knockbackForce);
            }

            DestroyArrow();
        }

        [Server]
        private void DestroyArrow()
        {
            if (IsSpawned)
                Despawn();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            ProjectileManager.Instance.RegisterArrow(this);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            ProjectileManager.Instance.UnregisterArrow(this);
        }
    }
}
