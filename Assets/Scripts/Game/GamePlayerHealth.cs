using FishNet.Connection;
using FishNet.Object;
using Game.Managers;
using Game.UI;
using UnityEngine;

namespace Game
{
    public class GamePlayerHealth : Health
    {
        private GamePlayer _gamePlayer;

        public void Initialize(GamePlayer gamePlayer)
        {
            _gamePlayer = gamePlayer;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsOwner)
            {
                GameUIManager.Instance.InitializePlayerHealthSlider(_maxHealth);
                UpdateHealthUI(_health.Value);
            }
        }

        [Server]
        public void Heal(int amount)
        {
            _health.Value = Mathf.Min(_health.Value + amount, _maxHealth);
        }

        [Server]
        protected override void Die()
        {
            base.Die();

            GamePlayerSpawnManager.Instance.GamePlayers.Remove(_gamePlayer);

            if (_lastAttacker != null && _lastAttacker != _gamePlayer)
                _lastAttacker.AddPlayerKill();

            GameManager.Instance.PlayerDied();
            ShowGameOverScreen(Owner);
            ServerManager.Despawn(gameObject);
        }

        [TargetRpc]
        private void ShowGameOverScreen(NetworkConnection conn)
        {
            GameUIManager.Instance.ShowMatchEndScreen(false, GameManager.Instance.AlivePlayersCount.Value, _gamePlayer.PlayerKills.Value, _gamePlayer.ZombieKills.Value, GameManager.Instance.MatchTimeSeconds.Value);
        }

        private void Awake()
        {
            _health.OnChange += OnHealthChanged;
        }

        private void OnDestroy()
        {
            _health.OnChange -= OnHealthChanged;
        }

        private void OnHealthChanged(int previous, int next, bool asServer)
        {
            if (asServer || IsOwner == false)
                return;

            UpdateHealthUI(next);
        }

        private void UpdateHealthUI(int health)
        {
            GameUIManager.Instance.UpdatePlayerHp(health);
        }
    }
}