
using System.Collections;
using UnityEngine;

public class BowPlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform bowPivot;
    [SerializeField] private Transform handL;
    [SerializeField] private Transform handR;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private GameObject arrowPrefab;

    [SerializeField] private float drawTime = 0.18f;
    [SerializeField] private float returnTime = 0.15f;
    [SerializeField] private float bowRecoil = 0.06f;
    [SerializeField] private float arrowSpeed = 8f;
    [SerializeField] private float arrowLifetime = 3f;

    [SerializeField] private Vector2 handLOffset = new Vector2(0f, 0.12f);
    [SerializeField] private Vector2 handROffset = new Vector2(0f, -0.08f);

    private Vector3 bowStartPosition;
    private Vector3 handLStartPosition;
    private Vector3 handRStartPosition;

    private GameObject chargingArrow;
    private Coroutine returnRoutine;

    private bool isCharging;
    private bool isReturning;
    private float chargeProgress;

    private void Awake()
    {
        bowStartPosition = bowPivot.localPosition;
        handLStartPosition = handL.localPosition;
        handRStartPosition = handR.localPosition;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isCharging && !isReturning)
        {
            BeginCharging();
        }

        if (isCharging && Input.GetMouseButton(0))
        {
            UpdateCharging();
        }

        if (isCharging && Input.GetMouseButtonUp(0))
        {
            ReleaseArrow();
        }
    }

    private void BeginCharging()
    {
        isCharging = true;
        chargeProgress = 0f;

        if (returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            returnRoutine = null;
        }

        chargingArrow = Instantiate(
            arrowPrefab,
            arrowSpawnPoint.position,
            GetArrowRotation()
        );

        chargingArrow.transform.SetParent(transform, true);
    }

    private void UpdateCharging()
    {
        chargeProgress = Mathf.MoveTowards(
            chargeProgress,
            1f,
            Time.deltaTime / Mathf.Max(drawTime, 0.01f)
        );

        float t = Mathf.SmoothStep(0f, 1f, chargeProgress);

        handL.localPosition = Vector3.Lerp(
            handLStartPosition,
            handLStartPosition + new Vector3(
                handLOffset.x, handLOffset.y, 0f),
            t
        );

        handR.localPosition = Vector3.Lerp(
            handRStartPosition,
            handRStartPosition + new Vector3(
                handROffset.x, handROffset.y, 0f),
            t
        );

        bowPivot.localPosition = Vector3.Lerp(
            bowStartPosition,
            bowStartPosition - new Vector3(0f, bowRecoil, 0f),
            t
        );

        if (chargingArrow != null)
        {
            chargingArrow.transform.position = arrowSpawnPoint.position;
            chargingArrow.transform.rotation = GetArrowRotation();
        }
    }

    private void ReleaseArrow()
    {
        isCharging = false;

        if (chargingArrow != null)
        {
            GameObject arrow = chargingArrow;
            chargingArrow = null;

            arrow.transform.SetParent(null, true);

            StartCoroutine(FlyArrow(arrow, -transform.up.normalized));
        }

        returnRoutine = StartCoroutine(ReturnToIdle());
    }

    private IEnumerator FlyArrow(GameObject arrow, Vector3 direction)
    {
        float elapsed = 0f;

        while (arrow != null && elapsed < arrowLifetime)
        {
            arrow.transform.position +=
                direction.normalized * arrowSpeed * Time.deltaTime;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (arrow != null)
        {
            Destroy(arrow);
        }
    }

    private IEnumerator ReturnToIdle()
    {
        isReturning = true;

        Vector3 bowCurrent = bowPivot.localPosition;
        Vector3 leftCurrent = handL.localPosition;
        Vector3 rightCurrent = handR.localPosition;

        float elapsed = 0f;

        while (elapsed < returnTime)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / returnTime);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            bowPivot.localPosition = Vector3.Lerp(
                bowCurrent, bowStartPosition, smoothT);

            handL.localPosition = Vector3.Lerp(
                leftCurrent, handLStartPosition, smoothT);

            handR.localPosition = Vector3.Lerp(
                rightCurrent, handRStartPosition, smoothT);

            yield return null;
        }

        bowPivot.localPosition = bowStartPosition;
        handL.localPosition = handLStartPosition;
        handR.localPosition = handRStartPosition;

        isReturning = false;
        returnRoutine = null;
    }

    private Quaternion GetArrowRotation()
    {
        Vector3 direction = transform.up;

        float angle = Mathf.Atan2(direction.y, direction.x)
                    * Mathf.Rad2Deg;

        return Quaternion.Euler(0f, 0f, angle - 180f);
    }
}
