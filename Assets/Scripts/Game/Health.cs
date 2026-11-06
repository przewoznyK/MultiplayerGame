using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Game
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] protected int _maxHealth = 100;
        protected readonly SyncVar<int> _health = new SyncVar<int>(100);

        public override void OnStartServer()
        {
            base.OnStartServer();
            _health.Value = _maxHealth;
        }

        public void TakeDamage(int damage)
        {
            _health.Value = Mathf.Max(0, _health.Value - damage);

            if (_health.Value <= 0)
                Die();
        }

        private void Die()
        {
            NetworkObject.Despawn();
        }
    }
}