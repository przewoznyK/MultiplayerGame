using FishNet.Object;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class GamePlayerAttack : NetworkBehaviour
    {
        [SerializeField] private Transform _attackPivot;
        [SerializeField] private float _pullbackTime = 0.10f;
        [SerializeField] private float _thrustTime = 0.05f;
        [SerializeField] private float _slashTime = 0.12f;
        [SerializeField] private float _returnTime = 0.12f;
        [SerializeField] private Vector2 _pullbackOffset = new Vector2(0f, -0.04f);
        [SerializeField] private Vector2 _forwardOffset = new Vector2(0f, 0.12f);
        [SerializeField] private float _slashAngle = -70f;

        private Vector2 _startPosition;
        private Quaternion _startRotation;
        private bool _isAttacking;

        private void Awake()
        {
            _startPosition = _attackPivot.localPosition;
            _startRotation = _attackPivot.localRotation;
        }

        public void AttackPlayer()
        {
            if (_isAttacking) 
                return;

            StartCoroutine(Attack());
            CmdAttack();
        }

        [ServerRpc]
        private void CmdAttack()
        {
            if (!_isAttacking)
                StartCoroutine(Attack());

            RpcPlayAttackAnimation();
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void RpcPlayAttackAnimation()
        {
            if (!_isAttacking)
                StartCoroutine(Attack());
        }

        private IEnumerator Attack()
        {
            _isAttacking = true;

            Vector2 pulledPosition = _startPosition + new Vector2(_pullbackOffset.x, _pullbackOffset.y);
            Vector2 forwardPosition = _startPosition + new Vector2(_forwardOffset.x, _forwardOffset.y);

            yield return Animate(pulledPosition, _startRotation, _pullbackTime);
            yield return Animate(forwardPosition, _startRotation, _thrustTime);
            yield return SweepSlash();
            yield return Animate(_startPosition, _startRotation, _returnTime);

            _attackPivot.localPosition = _startPosition;
            _attackPivot.localRotation = _startRotation;

            _isAttacking = false;
        }

        private IEnumerator SweepSlash()
        {
            Quaternion leftRotation = _startRotation * Quaternion.Euler(0f, 0f, -_slashAngle);
            Quaternion rightRotation = _startRotation * Quaternion.Euler(0f, 0f, _slashAngle);

            _attackPivot.localRotation = leftRotation;

            float elapsed = 0f;

            while (elapsed < _slashTime)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / _slashTime);
                float smoothT = Mathf.SmoothStep(0f, 1f, t);

                _attackPivot.localPosition = _startPosition + new Vector2(_forwardOffset.x, _forwardOffset.y);
                _attackPivot.localRotation = Quaternion.Slerp(leftRotation, rightRotation, smoothT);

                yield return null;
            }

            _attackPivot.localRotation = rightRotation;
        }

        private IEnumerator Animate(Vector3 targetPosition, Quaternion targetRotation, float duration)
        {
            Vector3 fromPosition = _attackPivot.localPosition;
            Quaternion fromRotation = _attackPivot.localRotation;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);
                float smoothT = Mathf.SmoothStep(0f, 1f, t);

                _attackPivot.localPosition = Vector3.Lerp(fromPosition, targetPosition, smoothT);
                _attackPivot.localRotation = Quaternion.Slerp(fromRotation, targetRotation, smoothT);

                yield return null;
            }

            _attackPivot.localPosition = targetPosition;
            _attackPivot.localRotation = targetRotation;
        }
    }
}