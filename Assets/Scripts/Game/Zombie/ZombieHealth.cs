using FishNet.Object;

namespace Game
{
    public class ZombieHealth : Health
    {
        [Server]
        protected override void Die()
        {
            base.Die();

            if (_lastAttacker != null)
                _lastAttacker.AddZombieKill();

            ServerManager.Despawn(gameObject);
        }
    }
}