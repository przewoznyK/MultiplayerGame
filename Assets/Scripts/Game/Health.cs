using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Globalization;
using UnityEngine;

namespace Game
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] protected int maxHealth = 100;
        protected readonly SyncVar<int> _health = new SyncVar<int>(100);

        public override void OnStartServer()
        {
            base.OnStartServer();
            _health.Value = maxHealth;
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