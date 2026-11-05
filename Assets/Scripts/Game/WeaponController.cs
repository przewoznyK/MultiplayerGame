using UnityEngine;

namespace Game
{
    public abstract class WeaponController : MonoBehaviour
    {
        public abstract void PlayAttackAnimation();
        public abstract void PerformServerLogic(GamePlayer owner);
    }
}