using FishNet.Object;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class GamePlayerSwordAttack : GamePlayerAttack
    {
        [Header("ANIMATION")]
        [SerializeField] private Transform _attackPivot;
        [SerializeField] private float _pullbackTime = 0.10f;
        [SerializeField] private float _thrustTime = 0.05f;
        [SerializeField] private float _slashTime = 0.12f;
        [SerializeField] private float _returnTime = 0.12f;
        [SerializeField] private Vector2 _pullbackOffset = new Vector2(0f, -0.04f);
        [SerializeField] private Vector2 _forwardOffset = new Vector2(0f, 0.12f);
        [SerializeField] private float _slashAngle = -70f;
        [SerializeField] private LayerMask _enemyLayer;

        private Vector2 _startPosition;
        private Quaternion _startRotation;
        private bool _isAttacking;

        private void Awake()
        {
            _startPosition = _attackPivot.localPosition;
            _startRotation = _attackPivot.localRotation;
        }

        public override void Attack()
        {
            if (_isAttacking)
                return;

            StartCoroutine(AttackAnimation());
            CmdPlayAttackAnimation();
        }

        [ServerRpc]
        private void CmdPlayAttackAnimation()
        {
            if (!_isAttacking)
                StartCoroutine(AttackAnimation());

            RpcPlayAttackAnimation();
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void RpcPlayAttackAnimation()
        {
            if (!_isAttacking)
                StartCoroutine(AttackAnimation());
        }

        [ServerRpc]
        public void CmdPerformAttack()
        {
            if (targetPoint == null) return;

            Vector2 size = new Vector2(radius * 2f, height);
            float angle = targetPoint.eulerAngles.z;

            Collider2D[] hitEnemies = Physics2D.OverlapCapsuleAll(targetPoint.position, size, CapsuleDirection2D.Vertical, angle, _enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                if(enemy.TryGetComponent(out Health enemyHealth))
                    enemyHealth.TakeDamage(1000);
            }
        }

        private IEnumerator AttackAnimation()
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

            if (IsOwner)
                CmdPerformAttack();

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

        [Header("DEBUG")]
        public Transform targetPoint;
        public float height = 2f;
        public float radius = 0.5f;
        public Color gizmoColor = Color.green;

        private void OnDrawGizmos()
        {
            if (targetPoint == null)
                return;

            Gizmos.color = gizmoColor;
            Gizmos.matrix = Matrix4x4.TRS(targetPoint.position, targetPoint.rotation, Vector3.one);

            float pointOffset = Mathf.Max(0, (height / 2f) - radius);

            Vector3 topPoint = new Vector3(0, pointOffset, 0);
            Vector3 bottomPoint = new Vector3(0, -pointOffset, 0);

            Gizmos.DrawWireSphere(topPoint, radius);
            Gizmos.DrawWireSphere(bottomPoint, radius);

            Vector3 right = new Vector3(radius, 0, 0);

            Gizmos.DrawLine(topPoint + right, bottomPoint + right);
            Gizmos.DrawLine(topPoint - right, bottomPoint - right);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}