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

    private NetworkPlayer _owner;
    private Camera _playerCamera;
    private float _targetVerticalRotation = 0.0f;
    private float _shootForce = 0.0f;

    public NetworkPlayer Owner { 
        get => _owner;
        set {
            _owner = value;
            _playerCamera = _owner.PlayerCamera;
        }
    }

    private IEnumerator Start() {
        _shootForce = _shootForceLimits.x;

        while (enabled) {
            Rotate();
            VerticalAiming();
            ShotForceCalculation();
            TryToShoot();
            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// Change the color of barrel.
    /// </summary>
    public void SetBarrelColor(Color barrelColor) {
        _barrelRenderer.material.color = barrelColor;
    }

    /// <summary>
    /// Calculate the rotation of the cannon depending on the position of the mouse cursor.
    /// </summary>
    private void Rotate() {
        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, _cameraToMouseRayMask)) {
            Vector3 lookRotation = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(lookRotation);
        }
    }

    /// <summary>
    /// Calculate the vertical rotation of the barrel depending on the mouse wheel.
    /// </summary>
    private void VerticalAiming() {
        _targetVerticalRotation = Mathf.Clamp(_targetVerticalRotation + Input.mouseScrollDelta.y, _barrelVerticalRotationConstraints.x, _barrelVerticalRotationConstraints.y);
        var targetRotation = Quaternion.Euler(Vector3.right * _targetVerticalRotation);
        _barrel.transform.localRotation = Quaternion.Lerp(_barrel.transform.localRotation, targetRotation, _barrelAimSmoothing * Time.deltaTime);
    }

    /// <summary>
    /// Calculate the force of the shot depending on the time you press the LMB.
    /// </summary>
    private void ShotForceCalculation() {
        if (Input.GetKey(KeyCode.Mouse0) && _shootForce < _shootForceLimits.y) {
            _shootForce += _forceIncreasePerSecond * Time.deltaTime;
        }
    }

    /// <summary>
    /// Try to shoot if LMB is released.
    /// </summary>
    private void TryToShoot() {
        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            Owner.CmdRegisterShoot(_barrel.position, -_barrel.transform.up * _shootForce);
            _shootForce = _shootForceLimits.x;
            return;
        }
    }
}
