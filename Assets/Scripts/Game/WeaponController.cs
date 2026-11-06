using UnityEngine;

namespace Game
{
    public class WeaponController : EquippableController
    {
        public override void PlayPrimaryVisuals() { }
        public override void PerformServerLogic(GamePlayer owner) { }
        [SerializeField] protected LayerMask _targetMask;
    }
}