using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private LayerMask _raycastLayerMask;

    [Header("Barrel settings")]
    [SerializeField] private Transform _barrel;
    [SerializeField] private float _barrelAimSmoothing = 10.0f;
    [SerializeField] private Vector2 _barrelVerticalRotationConstraints;

    [Header("Shoot settings")]
    [SerializeField] private Vector2 _shootForceLimits;
    [SerializeField] private float _forceIncreasePerSecond = 5.0f;
    [SerializeField] private GameObject _ballPrefab;

    private float _targetVerticalRotation = 0.0f;
    private float _shootForce = 0.0f;

    private void Start() {
        _shootForce = _shootForceLimits.x;
    }

    private void Update() {
        Rotate();
        VerticalAiming();
        Shoot();
    }

    private void Rotate() {
        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, _raycastLayerMask)) {
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
            var ball = Instantiate(_ballPrefab, _barrel.position, Quaternion.identity);
            ball.GetComponent<Rigidbody>().AddForce(-_barrel.transform.up * _shootForce, ForceMode.Impulse);
            _shootForce = _shootForceLimits.x;
            return;
        }

        if (Input.GetKey(KeyCode.Mouse0) && _shootForce < _shootForceLimits.y) {
            _shootForce += _forceIncreasePerSecond * Time.deltaTime;
        }
    }
}
