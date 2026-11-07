using FishNet.Object;
using Game.Data;
using Global;
using UnityEngine;

namespace Game
{
    public class GamePlayer : NetworkBehaviour
    {
        public Transform LeftHand;
        public Transform RightHand;
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
            {
                OnCancelPrimaryPressed();
                Destroy(_currentEquippable.gameObject);
            }

            EquippableController spawnedItem = Instantiate(itemData.VisualPrefab, _itemHolder);
            spawnedItem.Initialize(this, itemData);

            _currentEquippable = spawnedItem;
        }

        public void UsePrimary()
        {
            if (_currentEquippable == null)
                return;

            _currentEquippable.PlayPrimaryUseVisuals();
            CmdUsePrimary();
        }

        public void ReleasePrimary()
        {
            if (_currentEquippable == null)
                return;

            _currentEquippable.PlayPrimaryReleaseVisuals();
            CmdReleasePrimary();
        }

        [ServerRpc]
        private void CmdUsePrimary()
        {
            if (_currentEquippable != null)
                _currentEquippable.PerformPrimaryUseLogic(this);

            RpcPlayPrimaryUseVisuals();
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void RpcPlayPrimaryUseVisuals()
        {
            if (_currentEquippable != null)
                _currentEquippable.PlayPrimaryUseVisuals();
        }

        [ServerRpc]
        private void CmdReleasePrimary()
        {
            if (_currentEquippable != null)
                _currentEquippable.PerformPrimaryReleaseLogic(this);

            RpcPlayPrimaryReleaseVisuals();
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void RpcPlayPrimaryReleaseVisuals()
        {
            if (_currentEquippable != null)
                _currentEquippable.PlayPrimaryReleaseVisuals();
        }

        public void OnCancelPrimaryPressed()
        {
            if (_currentEquippable == null) 
                return;

            _currentEquippable.PlayPrimaryCancelVisuals();
            CmdCancelPrimary();
        }

        [ServerRpc]
        private void CmdCancelPrimary()
        {
            if (_currentEquippable != null)
                _currentEquippable.PerformPrimaryCancelLogic(this);

            RpcPlayPrimaryCancelVisuals();
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void RpcPlayPrimaryCancelVisuals()
        {
            if (_currentEquippable != null)
                _currentEquippable.PlayPrimaryCancelVisuals();
        }
    }
}