using FishNet.Object;
using FishNet.Object.Synchronizing;
using Game.Data;
using Global;
using Global.Managers;
using UnityEngine;

namespace Game
{
    public class GamePlayer : NetworkBehaviour
    {
        public GamePlayerStats GamePlayerStats;
        public Transform LeftHand;
        public Transform RightHand;
        [SerializeField] private EquippableController _defaultHands;
        [SerializeField] private SpriteRenderer _bodySR;
        [SerializeField] private Transform _itemHolder;
        private EquippableController _currentEquippable;
        public readonly SyncVar<int> NetworkClassIndex = new SyncVar<int>(0);

        private void Awake()
        {
            NetworkClassIndex.OnChange += OnClassIndexChanged;
        }

        public void Initialize(int classIndex)
        {
            NetworkClassIndex.Value = classIndex;
            ApplyClassData(classIndex);
        }

        private void OnClassIndexChanged(int prevIndex, int newIndex, bool asServer)
        {
            if (asServer == false)
                ApplyClassData(newIndex);
        }

        private void ApplyClassData(int index)
        {
            PlayerClassData classData = PlayerClassManager.Instance.GetClassById(index);

            if (classData != null)
            {
                GamePlayerStats.Initialize(classData);
                _bodySR.sprite = classData.ClassSprite;
            }
        }

        public void UpdateEquippable(ItemData itemData)
        {
            if (_currentEquippable != null)
            {
                if(IsOwner)
                    OnCancelPrimaryPressed();
                
                if(_currentEquippable != _defaultHands)
                    Destroy(_currentEquippable.gameObject);
            }

            if(itemData.VisualPrefab == null)
            {
                _currentEquippable = _defaultHands;
                return;
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