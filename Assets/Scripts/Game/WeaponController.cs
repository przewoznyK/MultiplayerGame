using UnityEngine;

namespace Game
{
    public class WeaponController : EquippableController
    {
        [SerializeField] protected LayerMask _targetMask;
        protected bool _isAttacking;
        protected int _damage;

        public override void PerformPrimaryUseLogic(GamePlayer owner)
        {
            throw new System.NotImplementedException();
        }

        public override void PlayPrimaryUseVisuals()
        {
            throw new System.NotImplementedException();
        }
    }
}