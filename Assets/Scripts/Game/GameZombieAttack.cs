using FishNet.Object;
using UnityEngine;

namespace Game
{
    public class ZombieAttack : NetworkBehaviour
    {
        [SerializeField] private int damage = 10;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!IsServerInitialized)
                return;

            GamePlayerHealth playerHealth = collision.gameObject.GetComponent<GamePlayerHealth>();

            if (playerHealth == null)
                return;

            playerHealth.TakeDamage(damage);
        }
    }
}