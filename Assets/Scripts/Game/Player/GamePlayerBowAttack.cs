using Core;
using Game.Data;
using Global.Enums;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class GamePlayerBowAttack : WeaponController
    {
        [SerializeField] private GameObject _visualArrowPrefab;
        [SerializeField] private Transform _arrowSpawnPoint;
        [SerializeField] private GameObject _fakeArrowPrefab;

        [Header("ANIMATION")]
        [SerializeField] private float _maxDrawTime = 1f;
        [SerializeField] private float _returnTime = 0.2f;
        [SerializeField] private Vector2 _handLOffset;
        [SerializeField] private Vector2 _handROffset;
        [SerializeField] private float _bowRecoil = 0.2f;
        private Coroutine _drawCoroutine;
        private Coroutine _returnCoroutine;
        private Vector3 _bowStartPosition;
        private Vector3 _handLStartPosition;
        private Vector3 _handRStartPosition;

        private GamePlayer _gamePlayer;
        private GameObject _fakeArrowInstance;
        private Transform _attackPivot;
        private RangeWeaponData _rangeData;
        private float _serverChargeStartTime;
        private bool _isCancelledLocal = false;
        
        public override void Initialize(GamePlayer gamePlayer, ItemData data)
        {
            _gamePlayer = gamePlayer;

            if (data is RangeWeaponData rangeData)
                _rangeData = data as RangeWeaponData;
            else
                Debug.LogError("Provided ItemData is not of type RangeWeaponData.");

            _gamePlayer = gamePlayer;

            gamePlayer.LeftHand.localPosition = new Vector3(0.2f, -0.4f, 0f);
            gamePlayer.RightHand.localPosition = new Vector3(0.003f, -0.468f, 0f);

            _attackPivot = transform.parent;
            _bowStartPosition = _attackPivot.localPosition;
            _handLStartPosition = gamePlayer.LeftHand.localPosition;
            _handRStartPosition = gamePlayer.RightHand.localPosition;
        }

        public override void PerformPrimaryUseLogic(GamePlayer owner)
        {
            _serverChargeStartTime = Time.time;
        }

        public override void PerformPrimaryReleaseLogic(GamePlayer owner)
        {
            if (_serverChargeStartTime == 0f)
                return;

            int finalDamage = owner.GamePlayerStats.GetTotalDamage(_rangeData.Damage, WeaponType.Bow);
            float finalMaxSpeed = owner.GamePlayerStats.GetTotalMaxArrowSpeed(_rangeData.MaxProjectileSpeed);
            float chargeTime = Time.time - _serverChargeStartTime;
            float chargeRatio = Mathf.Clamp01(chargeTime / _maxDrawTime);
            float finalSpeed = Mathf.Lerp(_rangeData.MinProjectileSpeed, finalMaxSpeed, chargeRatio);

            ArrowProjectile projectile = owner.NetworkManager.GetPooledInstantiated<ArrowProjectile>(_visualArrowPrefab, _arrowSpawnPoint.position, GetArrowRotation(), true);

            projectile.Initialize(finalDamage, finalSpeed, owner);
            owner.ServerManager.Spawn(projectile.NetworkObject);

            _serverChargeStartTime = 0f;
        }

        public override void PlayPrimaryCancelVisuals()
        {
            _isCancelledLocal = true;
            ReturnBowToIdleState();
        }

        public override void PlayPrimaryUseVisuals()
        {
            _isCancelledLocal = false;

            if (_returnCoroutine != null)
            {
                StopCoroutine(_returnCoroutine);
                _returnCoroutine = null;
            }

            _drawCoroutine = StartCoroutine(DrawAnimation());
        }
        public override void PerformPrimaryCancelLogic(GamePlayer owner)
        {
            _serverChargeStartTime = 0f;
        }

        public override void PlayPrimaryReleaseVisuals()
        {
            if (_isCancelledLocal) 
                return;

            ReturnBowToIdleState();
            _returnCoroutine = StartCoroutine(ReturnToIdleAnimation());
        }

        private void ReturnBowToIdleState()
        {
            if (_drawCoroutine != null)
            {
                StopCoroutine(_drawCoroutine);
                _drawCoroutine = null;
            }

            if (_fakeArrowInstance != null)
                Destroy(_fakeArrowInstance);

            _returnCoroutine = StartCoroutine(ReturnToIdleAnimation());
        }

        private IEnumerator DrawAnimation()
        {
            if (_fakeArrowInstance == null)
                _fakeArrowInstance = Instantiate(_fakeArrowPrefab, _arrowSpawnPoint.position, GetArrowRotation(), transform);

            float chargeTimeLocal = 0f;

            while (true)
            {
                chargeTimeLocal += Time.deltaTime;

                float t = Mathf.Clamp01(chargeTimeLocal / _maxDrawTime);
                float smoothT = Mathf.SmoothStep(0f, 1f, t);

                _gamePlayer.LeftHand.localPosition = Vector3.Lerp(_handLStartPosition, _handLStartPosition + (Vector3)_handLOffset, smoothT);
                _gamePlayer.RightHand.localPosition = Vector3.Lerp(_handRStartPosition, _handRStartPosition + (Vector3)_handROffset, smoothT);
                _attackPivot.localPosition = Vector3.Lerp(_bowStartPosition, _bowStartPosition - new Vector3(0f, _bowRecoil, 0f), smoothT);

                if (_fakeArrowInstance != null)
                {
                    _fakeArrowInstance.transform.position = _arrowSpawnPoint.position;
                    _fakeArrowInstance.transform.rotation = GetArrowRotation();
                }

                yield return null;
            }
        }

        private IEnumerator ReturnToIdleAnimation()
        {
            Vector3 bowCurrent = _attackPivot.localPosition;
            Vector3 leftCurrent = _gamePlayer.LeftHand.localPosition;
            Vector3 rightCurrent = _gamePlayer.RightHand.localPosition;

            float elapsed = 0f;
            while (elapsed < _returnTime)
            {
                elapsed += Time.deltaTime;
                float smoothT = Mathf.SmoothStep(0f, 1f, elapsed / _returnTime);

                _attackPivot.localPosition = Vector3.Lerp(bowCurrent, _bowStartPosition, smoothT);
                _gamePlayer.LeftHand.localPosition = Vector3.Lerp(leftCurrent, _handLStartPosition, smoothT);
                _gamePlayer.RightHand.localPosition = Vector3.Lerp(rightCurrent, _handRStartPosition, smoothT);

                yield return null;
            }

            _attackPivot.localPosition = _bowStartPosition;
            _gamePlayer.LeftHand.localPosition = _handLStartPosition;
            _gamePlayer.RightHand.localPosition = _handRStartPosition;

            _returnCoroutine = null;
        }

        private Quaternion GetArrowRotation()
        {
            return _arrowSpawnPoint.rotation;
        }
    }
}