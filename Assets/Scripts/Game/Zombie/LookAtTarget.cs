using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class LookAtTarget : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _rotationSpeed = 60f;

    private readonly SyncVar<float> _targetRotation = new SyncVar<float>();

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        if (IsServerInitialized)
            _targetRotation.Value = _rb.rotation;
    }

    public void Look(Vector2 targetWorldPosition)
    {
        Vector2 direction = targetWorldPosition - (Vector2)transform.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        CmdUpdateTargetRotation(rotation);
    }

    [ServerRpc]
    private void CmdUpdateTargetRotation(float rotation)
    {
        _targetRotation.Value = rotation;
    }

    private void FixedUpdate()
    {
        float nextRotation = Mathf.MoveTowardsAngle(_rb.rotation, _targetRotation.Value, _rotationSpeed * Time.fixedDeltaTime);
        _rb.rotation = nextRotation;
    }
}
