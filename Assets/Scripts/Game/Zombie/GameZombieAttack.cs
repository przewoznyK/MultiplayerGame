using FishNet.Object;
using UnityEngine;
using System.Collections;

namespace Game
{
    public class ZombieAttack : NetworkBehaviour
    {
        [SerializeField] private int _damage = 10;
        [SerializeField] private float _attackCooldown = 1.5f;
        [Header("Animation")]
        [SerializeField] private Transform _handL;
        [SerializeField] private Transform _handR;
        [SerializeField] private Vector2 _windupOffset = new(-0.10f, -0.02f);
        [SerializeField] private Vector2 _punchOffset = new(0.08f, -0.15f);
        [SerializeField] private float _windupTime = 0.14f;
        [SerializeField] private float _punchTime = 0.18f;
        [SerializeField] private float _returnTime = 0.20f;
        [SerializeField] private float _arcWidth = 0.25f;
        [SerializeField] private float _windupAngle = 45f;
        [SerializeField] private float _punchAngle = 70f;

        private Vector2 _leftStartPosition;
        private Vector2 _rightStartPosition;

        private Quaternion _leftStartRotation;
        private Quaternion _rightStartRotation;

        private float _nextAttackTime;
        private bool _isAttacking;
        private bool _nextLeftHand = true;

        private void Awake()
        {
            _leftStartPosition = _handL.localPosition;
            _rightStartPosition = _handR.localPosition;

            _leftStartRotation = _handL.localRotation;
            _rightStartRotation = _handR.localRotation;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            TryAttackTarget(collision.gameObject);
        }

        private void TryAttackTarget(GameObject target)
        {
            if (IsServerInitialized == false)
                return;

            if (Time.time < _nextAttackTime)
                return;

            GamePlayerHealth playerHealth = target.GetComponent<GamePlayerHealth>();

            if (playerHealth == null)
                return;

            _nextAttackTime = Time.time + _attackCooldown;

            ObserversRpcPlayAttackAnimation();

            playerHealth.TakeDamage(_damage);
        }

        [ObserversRpc]
        private void ObserversRpcPlayAttackAnimation()
        {
            if (_isAttacking == false)
                StartCoroutine(Attack());
        }

        private IEnumerator Attack()
        {
            _isAttacking = true;

            bool useLeft = _nextLeftHand;
            _nextLeftHand = !_nextLeftHand;

            Transform hand = useLeft ? _handL : _handR;

            Vector2 startPosition = useLeft ? _leftStartPosition : _rightStartPosition;
            Quaternion startRotation = useLeft ? _leftStartRotation : _rightStartRotation;

            float side = useLeft ? -1f : 1f;

            Vector2 windupPosition = startPosition + new Vector2(_windupOffset.x * side, _windupOffset.y);
            Quaternion windupRotation = startRotation * Quaternion.Euler(0f, 0f, _windupAngle * side);

            yield return Animate(hand, windupPosition, windupRotation, _windupTime);
            yield return SwingArc(hand, startPosition, startRotation, side);
            yield return Animate(hand, startPosition, startRotation, _returnTime);

            hand.localPosition = startPosition;
            hand.localRotation = startRotation;

            _isAttacking = false;
        }

        private IEnumerator SwingArc(Transform hand, Vector2 startPosition, Quaternion startRotation, float side)
        {
            Vector2 p0 = hand.localPosition;

            Vector2 p1 = startPosition + new Vector2(_arcWidth * side, 0.35f);
            Vector2 p2 = startPosition + new Vector2(-_arcWidth * side, 0.35f);
            Vector2 p3 = startPosition + new Vector2(_punchOffset.x * side, _punchOffset.y);

            Quaternion r0 = hand.localRotation;
            Quaternion r1 = startRotation * Quaternion.Euler(0f, 0f, _windupAngle * side);
            Quaternion r2 = startRotation * Quaternion.Euler(0f, 0f, -_punchAngle * side);

            float elapsed = 0f;

            while (elapsed < _punchTime)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / _punchTime);
                float u = 1f - t;

                Vector3 position = (u * u * u * p0) + (3f * u * u * t * p1) + (3f * u * t * t * p2) + (t * t * t * p3);

                hand.localPosition = position;

                if (t < 0.5f)
                    hand.localRotation = Quaternion.Slerp(r0, r1, t * 2f);
                else
                    hand.localRotation = Quaternion.Slerp(r1, r2, (t - 0.5f) * 2f);

                yield return null;
            }

            hand.localPosition = p3;
            hand.localRotation = r2;
        }

        private IEnumerator Animate(Transform hand, Vector2 targetPosition, Quaternion targetRotation, float duration)
        {
            Vector2 initialPosition = hand.localPosition;
            Quaternion initialRotation = hand.localRotation;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);
                float smoothT = Mathf.SmoothStep(0f, 1f, t);

                hand.localPosition = Vector3.Lerp(initialPosition, targetPosition, smoothT);
                hand.localRotation = Quaternion.Slerp(initialRotation, targetRotation, smoothT);

                yield return null;
            }

            hand.localPosition = targetPosition;
            hand.localRotation = targetRotation;
        }
    }
}