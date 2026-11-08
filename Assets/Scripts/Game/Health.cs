using FishNet.Object;
using FishNet.Object.Synchronizing;
using Game.Managers;
using UnityEngine;

namespace Game
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] protected int _maxHealth = 100;
        protected readonly SyncVar<int> _health = new SyncVar<int>();
        public readonly SyncVar<bool> IsDead = new SyncVar<bool>(false);
        protected GamePlayer _lastAttacker;

        public override void OnStartServer()
        {
            base.OnStartServer();
            _health.Value = _maxHealth;
        }

        [Server]
        public void TakeDamage(int amount, GamePlayer attacker = null)
        {
            if (IsDead.Value) 
               return;

            _health.Value -= amount;
            _lastAttacker = attacker;

            if (_health.Value <= 0)
                Die();
        }

        [Server]
        protected virtual void Die()
        {
            IsDead.Value = true;
        }
    }
}