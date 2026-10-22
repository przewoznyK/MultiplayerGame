
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    void Update()
    {
        if (Camera.main == null)
            return;

        Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction =
            mousePosition - transform.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle + 90f);
    }
}
