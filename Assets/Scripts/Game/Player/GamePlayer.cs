using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Game.Data;
using Game.Managers;
using Game.UI;
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
        public GamePlayerHealth GamePlayerHealth;
        [SerializeField] private EquippableController _defaultHands;
        [SerializeField] private SpriteRenderer _bodySR;
        [SerializeField] private Transform _itemHolder;
        private EquippableController _currentEquippable;

        public readonly SyncVar<int> NetworkClassIndex = new SyncVar<int>(0);
        public readonly SyncVar<int> PlayerKills = new SyncVar<int>(0);
        public readonly SyncVar<int> ZombieKills = new SyncVar<int>(0);

        private void Awake()
        {
            GamePlayerHealth.Initialize(this);
            NetworkClassIndex.OnChange += OnClassIndexChanged;
            PlayerKills.OnChange += OnPlayerKillsChanged;
            ZombieKills.OnChange += OnZombieKillsChanged;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            GamePlayerSpawnManager.Instance.GamePlayers.Add(this);

            if (IsOwner)
            {
                GamePlayerCameraController cameraController = Camera.main.GetComponent<GamePlayerCameraController>();
                cameraController.Initialize(transform);
                GameUIManager.Instance.AssignLocalPlayer(this, cameraController);
            }
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            GamePlayerSpawnManager.Instance.GamePlayers.Remove(this);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            if (GamePlayerHealth.IsDead.Value == false)
            {
                GameManager.Instance.PlayerDisconnected(true);
            }
        }

        private void OnPlayerKillsChanged(int prev, int next, bool asServer)
        {
            if (asServer == false && IsOwner)
                GameUIManager.Instance.UpdatePlayerKills(next);
        }

        private void OnZombieKillsChanged(int prev, int next, bool asServer)
        {
            if (asServer == false && IsOwner)
                GameUIManager.Instance.UpdateZombieKills(next);
        }

        [Server]
        public void AddPlayerKill() 
        {
            PlayerKills.Value++;
        }

        [Server]
        public void AddZombieKill()
        {
            ZombieKills.Value++;
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

        [TargetRpc]
        public void TargetMatchWon(NetworkConnection conn, int alivePlayers, int pKills, int zKills, int time)
        {
            GameUIManager.Instance.ShowMatchEndScreen(true, alivePlayers, pKills, zKills, time);
        }
    }
}