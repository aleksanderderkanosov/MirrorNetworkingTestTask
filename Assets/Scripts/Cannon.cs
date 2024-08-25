using System.Collections;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private LayerMask _cameraToMouseRayMask;

    [Header("Barrel settings")]
    [SerializeField] private Transform _barrel;
    [SerializeField] private Renderer _barrelRenderer;
    [SerializeField] private float _barrelAimSmoothing = 10.0f;
    [SerializeField] private Vector2 _barrelVerticalRotationConstraints;

    [Header("Shoot settings")]
    [SerializeField] private Vector2 _shootForceLimits;
    [SerializeField] private float _forceIncreasePerSecond = 5.0f;

    private NetworkPlayer _player;
    private Camera _playerCamera;
    private float _targetVerticalRotation = 0.0f;
    private float _shootForce = 0.0f;

    public NetworkPlayer Player { 
        get => _player;
        set {
            _player = value;
            _playerCamera = _player.PlayerCamera;
        }
    }

    private IEnumerator Start() {
        _shootForce = _shootForceLimits.x;

        while (enabled) {
            Rotate();
            VerticalAiming();
            Shoot();
            yield return null;
        }

        yield return null;
    }

    public void SetBarrelColor(Color barrelColor) {
        _barrelRenderer.material.color = barrelColor;
    }

    private void Rotate() {
        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, _cameraToMouseRayMask)) {
            Vector3 lookRotation = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(lookRotation);
        }
    }

    private void VerticalAiming() {
        _targetVerticalRotation = Mathf.Clamp(_targetVerticalRotation + Input.mouseScrollDelta.y, _barrelVerticalRotationConstraints.x, _barrelVerticalRotationConstraints.y);
        var targetRotation = Quaternion.Euler(Vector3.right * _targetVerticalRotation);
        _barrel.transform.localRotation = Quaternion.Lerp(_barrel.transform.localRotation, targetRotation, _barrelAimSmoothing * Time.deltaTime);
    }

    private void Shoot() {
        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            Player.CmdShoot(_barrel.position, -_barrel.transform.up * _shootForce);
            _shootForce = _shootForceLimits.x;
            return;
        }

        if (Input.GetKey(KeyCode.Mouse0) && _shootForce < _shootForceLimits.y) {
            _shootForce += _forceIncreasePerSecond * Time.deltaTime;
        }
    }
}
