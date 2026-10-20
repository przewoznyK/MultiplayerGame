
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    void Update()
    {
        if (Camera.main == null)
            return;

        // Pobierz pozycjê kursora w œwiecie gry
        Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Oblicz kierunek od postaci do kursora
        Vector2 direction =
            mousePosition - transform.position;

        // Nie obracaj postaci, gdy kursor jest dok³adnie na niej
        if (direction.sqrMagnitude < 0.001f)
            return;

        // Oblicz k¹t w kierunku kursora
        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Obróæ ca³¹ postaæ
        transform.rotation =
            Quaternion.Euler(0f, 0f, angle + 90f);
    }
}
