using Game;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePlayerCameraController : MonoBehaviour
{
    [SerializeField] private InputActionReference _primaryAction;
    [SerializeField] private InputActionReference _secondaryAction;
    [SerializeField] private Vector3 _cameraOffset = new Vector3(0, 0, -10f);
    [SerializeField] private float _followSpeed = 10f;
    private Camera _mainCamera;
    private Transform _target;
    private bool _isSpectating = false;
    private int _spectatorIndex = 0;

    public void Initialize(Transform playerTarget)
    {
        _mainCamera = Camera.main;
        _target = playerTarget;
        _isSpectating = false;

        _mainCamera.transform.position = _target.position + _cameraOffset;
    }

    private void LateUpdate()
    {
        if (_target == null)
            return;

        _mainCamera.transform.position = _target.position + _cameraOffset;
    }

    public void StartSpectating()
    {
        _isSpectating = true;
        SwitchSpectatorTarget(0);

        _primaryAction.action.Enable();
        _secondaryAction.action.Enable();

        _primaryAction.action.performed += NextSpectatorTarget;
        _secondaryAction.action.performed += PreviousSpectatorTarget;
    }

    private void SwitchSpectatorTarget(int direction)
    {
        var alivePlayers = GamePlayerSpawnManager.Instance.GamePlayers;

        _spectatorIndex += direction;

        if (_spectatorIndex < 0)
            _spectatorIndex = alivePlayers.Count - 1;
        else if (_spectatorIndex >= alivePlayers.Count)
            _spectatorIndex = 0;

        _target = alivePlayers[_spectatorIndex].transform;
    }

    private void NextSpectatorTarget(InputAction.CallbackContext ctx)
    {
        SwitchSpectatorTarget(-1);
    }

    private void PreviousSpectatorTarget(InputAction.CallbackContext ctx)
    {
        SwitchSpectatorTarget(1);
    }

    private void OnDisable()
    {
        _primaryAction.action.performed -= NextSpectatorTarget;
        _secondaryAction.action.performed -= PreviousSpectatorTarget;

        _primaryAction.action.Disable();
        _secondaryAction.action.Disable();
    }
}
