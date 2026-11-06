using FishNet.Object;
using Game.Data;
using Global;
using UnityEngine;

namespace Game
{
    public class GamePlayer : NetworkBehaviour
    {
        [SerializeField] private SpriteRenderer _bodySR;
        [SerializeField] private Transform _itemHolder;
        private EquippableController _currentEquippable;

        public void Initialize(int classIndex)
        {
            _bodySR.sprite = SpriteManager.Instance.GetClassSpriteById(classIndex);
        }

        public void UpdateEquippable(ItemData itemData)
        {
            if (_currentEquippable != null)
                Destroy(_currentEquippable.gameObject);

            if (itemData == null || itemData.VisualPrefab == null)
                return;

            EquippableController spawnedItem = Instantiate(itemData.VisualPrefab, _itemHolder);
            spawnedItem.Initialize(itemData);

            _currentEquippable = spawnedItem;
        }

        public void Attack()
        {
            if (IsOwner == false || _currentEquippable == null) 
                return;

            _currentEquippable.PlayPrimaryVisuals();
            CmdPerformAttack();
        }

        [ServerRpc]
        private void CmdPerformAttack()
        {
            if (_currentEquippable != null)
                _currentEquippable.PerformServerLogic(this);

            RpcPlayAttackAnimation();
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void RpcPlayAttackAnimation()
        {
            if (_currentEquippable != null)
                _currentEquippable.PlayPrimaryVisuals();
        }
    }
}