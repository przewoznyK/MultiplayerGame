using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Game
{
    public class GamePlayerHealth : NetworkBehaviour
    {
        [SerializeField] private int maxHealth = 100;

        private readonly SyncVar<int> _health = new SyncVar<int>(100);

        public int CurrentHealth => _health.Value;
        public int MaxHealth => maxHealth;

        public override void OnStartServer()
        {
            base.OnStartServer();

            _health.Value = maxHealth;
            GameUIManager.Instance.InitializePlayerHealthSlider(maxHealth);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            UpdateHealthUI(_health.Value);
        }

        private void Awake()
        {
            _health.OnChange += OnHealthChanged;
        }

        private void OnDestroy()
        {
            _health.OnChange -= OnHealthChanged;
        }

        public void TakeDamage(int damage)
        {
            _health.Value = Mathf.Max(0, _health.Value - damage);

            if (_health.Value <= 0)
                Die();
        }

        private void OnHealthChanged(int previous, int next, bool asServer)
        {
            if (!IsOwner)
                return;

            UpdateHealthUI(next);
        }

        private void UpdateHealthUI(int health)
        {
            GameUIManager.Instance.UpdatePlayerHp(health);
        }

        private void Die()
        {
            gameObject.SetActive(false);
        }
    }
}