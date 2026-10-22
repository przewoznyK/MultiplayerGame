
using System.Collections;
using UnityEngine;

public class PlayerSwordAttack : MonoBehaviour
{
    [SerializeField] private Transform attackPivot;

    [SerializeField] private float pullbackTime = 0.10f;
    [SerializeField] private float thrustTime = 0.07f;
    [SerializeField] private float slashTime = 0.12f;
    [SerializeField] private float returnTime = 0.14f;

    [SerializeField] private Vector2 pullbackOffset = new Vector2(0f, -0.04f);
    [SerializeField] private Vector2 forwardOffset = new Vector2(0f, 0.12f);

    [SerializeField] private float slashAngle = 20f;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isAttacking;

    private void Awake()
    {
        startPosition = attackPivot.localPosition;
        startRotation = attackPivot.localRotation;
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

        Vector3 pulledPosition = startPosition +
            new Vector3(pullbackOffset.x, pullbackOffset.y, 0f);

        Vector3 forwardPosition = startPosition +
            new Vector3(forwardOffset.x, forwardOffset.y, 0f);

        yield return Animate(pulledPosition, startRotation, pullbackTime);
        yield return Animate(forwardPosition, startRotation, thrustTime);
        yield return SweepSlash();
        yield return Animate(startPosition, startRotation, returnTime);

        attackPivot.localPosition = startPosition;
        attackPivot.localRotation = startRotation;

        isAttacking = false;
    }

    private IEnumerator SweepSlash()
    {
        Quaternion leftRotation = startRotation *
            Quaternion.Euler(0f, 0f, -slashAngle);

        Quaternion rightRotation = startRotation *
            Quaternion.Euler(0f, 0f, slashAngle);

        attackPivot.localRotation = leftRotation;

        float elapsed = 0f;

        while (elapsed < slashTime)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / slashTime);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            attackPivot.localPosition = startPosition +
                new Vector3(forwardOffset.x, forwardOffset.y, 0f);

            attackPivot.localRotation = Quaternion.Slerp(
                leftRotation, rightRotation, smoothT);

            yield return null;
        }

        attackPivot.localRotation = rightRotation;
    }

    private IEnumerator Animate(
        Vector3 targetPosition,
        Quaternion targetRotation,
        float duration)
    {
        Vector3 fromPosition = attackPivot.localPosition;
        Quaternion fromRotation = attackPivot.localRotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            attackPivot.localPosition = Vector3.Lerp(
                fromPosition, targetPosition, smoothT);

            attackPivot.localRotation = Quaternion.Slerp(
                fromRotation, targetRotation, smoothT);

            yield return null;
        }

        attackPivot.localPosition = targetPosition;
        attackPivot.localRotation = targetRotation;
    }
}
