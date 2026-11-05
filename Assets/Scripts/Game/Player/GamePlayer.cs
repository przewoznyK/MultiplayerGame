using FishNet.Object;
using Global;
using UnityEngine;

namespace Game
{
    public class GamePlayer : NetworkBehaviour
    {
        [SerializeField] private SpriteRenderer _bodySR;
        [SerializeField] private Transform _weaponHolder;
        [SerializeField] private WeaponController _currentWeapon;

        public void Initialize(int classIndex)
        {
            _bodySR.sprite = SpriteManager.Instance.GetClassSpriteById(classIndex);
        }

        public void UpdateWeapon(ItemData itemData)
        {
            if (_currentWeapon != null)
                Destroy(_currentWeapon.gameObject);

            if (itemData == null || itemData.WeaponVisualPrefab == null) 
                return;

            _currentWeapon = Instantiate(itemData.WeaponVisualPrefab, _weaponHolder);
        }

        public void Attack()
        {
            if (IsOwner == false || _currentWeapon == null) 
                return;

            _currentWeapon.PlayAttackAnimation();
            CmdPerformAttack();
        }

        [ServerRpc]
        private void CmdPerformAttack()
        {
            if (_currentWeapon != null)
                _currentWeapon.PerformServerLogic(this);

            RpcPlayAttackAnimation();
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void RpcPlayAttackAnimation()
        {
            if (_currentWeapon != null)
                _currentWeapon.PlayAttackAnimation();
        }
    }
}