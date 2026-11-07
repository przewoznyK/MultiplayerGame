using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Game
{
    public class ArrowProjectile : NetworkBehaviour
    {
        public readonly SyncVar<float> Speed = new SyncVar<float>(1);
        private int _damage;
        private GameObject _owner;

        public void Initialize(int damage, float speed, GameObject owner)
        {
            _damage = damage;
            Speed.Value = speed;
            _owner = owner;

            Invoke(nameof(DestroyArrow), 5f);
        }

        public void MoveArrow(float deltaTime)
        {
            transform.position += transform.right * Speed.Value * deltaTime;
        }

        [Server]
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject == _owner)
                return;

            if (col.TryGetComponent(out Health enemyHealth))
                enemyHealth.TakeDamage(_damage);

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
