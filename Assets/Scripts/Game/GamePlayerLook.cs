using FishNet.Object;
using UnityEngine;

public class GamePlayerLook : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private float _rotationSpeed = 60f;
    private float _serverRotation;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        _serverRotation = _rigidbody.rotation;
    }

    public void LookPlayer(Vector2 mouseScreenPosition)
    {
        if (mouseScreenPosition == Vector2.zero)
            return;

        Camera cam = Camera.main;

        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, -cam.transform.position.z));
        Vector2 direction = mouseWorldPosition - transform.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        SendLookInput(rotation);
    }

    [ServerRpc]
    private void SendLookInput(float rotation)
    {
        _serverRotation = rotation;
    }

    private void FixedUpdate()
    {
        float nextRotation = Mathf.MoveTowardsAngle(_rigidbody.rotation, _serverRotation, _rotationSpeed * Time.fixedDeltaTime);
        _rigidbody.rotation = nextRotation;
    }
}