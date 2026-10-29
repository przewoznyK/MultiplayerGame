
using System.Collections;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    [SerializeField] private Transform handL;
    [SerializeField] private Transform handR;

    [SerializeField] private float windupTime = 0.14f;
    [SerializeField] private float punchTime = 0.18f;
    [SerializeField] private float returnTime = 0.20f;

    [SerializeField] private Vector2 windupOffset = new Vector2(-0.10f, -0.02f);
    [SerializeField] private Vector2 punchOffset = new Vector2(0.08f, 0.18f);

    [SerializeField] private float arcWidth = 0.30f;
    [SerializeField] private float windupAngle = 65f;
    [SerializeField] private float punchAngle = 120f;

    private Vector3 leftStartPosition;
    private Vector3 rightStartPosition;

    private Quaternion leftStartRotation;
    private Quaternion rightStartRotation;

    private bool isAttacking;
    private bool nextLeftHand = true;

    private void Awake()
    {
        leftStartPosition = handL.localPosition;
        rightStartPosition = handR.localPosition;

        leftStartRotation = handL.localRotation;
        rightStartRotation = handR.localRotation;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        bool useLeft = nextLeftHand;
        nextLeftHand = !nextLeftHand;

        Transform hand = useLeft ? handL : handR;

        Vector3 startPosition = useLeft
            ? leftStartPosition : rightStartPosition;

        Quaternion startRotation = useLeft
            ? leftStartRotation : rightStartRotation;

        float side = useLeft ? -1f : 1f;

        Vector3 windupPosition = startPosition + new Vector3(
            windupOffset.x * side,
            windupOffset.y,
            0f
        );

        Quaternion windupRotation = startRotation *
            Quaternion.Euler(0f, 0f, windupAngle * side);

        yield return Animate(
            hand, windupPosition, windupRotation, windupTime);

        yield return SwingArc(
            hand, startPosition, startRotation, side);

        yield return Animate(
            hand, startPosition, startRotation, returnTime);

        hand.localPosition = startPosition;
        hand.localRotation = startRotation;

        isAttacking = false;
    }

    private IEnumerator SwingArc(
        Transform hand,
        Vector3 startPosition,
        Quaternion startRotation,
        float side)
    {
        Vector3 p0 = hand.localPosition;

        Vector3 p1 = startPosition + new Vector3(
            arcWidth * side,
            0.35f,
            0f
        );

        Vector3 p2 = startPosition + new Vector3(
            -arcWidth * side,
            0.35f,
            0f
        );

        Vector3 p3 = startPosition + new Vector3(
            punchOffset.x * side,
            punchOffset.y,
            0f
        );

        Quaternion r0 = hand.localRotation;
        Quaternion r1 = startRotation *
            Quaternion.Euler(0f, 0f, windupAngle * side);

        Quaternion r2 = startRotation *
            Quaternion.Euler(0f, 0f, -punchAngle * side);

        float elapsed = 0f;

        while (elapsed < punchTime)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / punchTime);
            float u = 1f - t;

            Vector3 position =
                u * u * u * p0 +
                3f * u * u * t * p1 +
                3f * u * t * t * p2 +
                t * t * t * p3;

            hand.localPosition = position;

            if (t < 0.5f)
            {
                hand.localRotation = Quaternion.Slerp(
                    r0, r1, t * 2f);
            }
            else
            {
                hand.localRotation = Quaternion.Slerp(
                    r1, r2, (t - 0.5f) * 2f);
            }

            yield return null;
        }

        hand.localPosition = p3;
        hand.localRotation = r2;
    }

    private IEnumerator Animate(
        Transform hand,
        Vector3 targetPosition,
        Quaternion targetRotation,
        float duration)
    {
        Vector3 initialPosition = hand.localPosition;
        Quaternion initialRotation = hand.localRotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            hand.localPosition = Vector3.Lerp(
                initialPosition, targetPosition, smoothT);

            hand.localRotation = Quaternion.Slerp(
                initialRotation, targetRotation, smoothT);

            yield return null;
        }

        hand.localPosition = targetPosition;
        hand.localRotation = targetRotation;
    }
}
