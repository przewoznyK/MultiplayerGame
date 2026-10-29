namespace Game
{
    public class GamePlayerHealth : Health
    {
        public override void OnStartServer()
        {
            base.OnStartServer();
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
    }
}