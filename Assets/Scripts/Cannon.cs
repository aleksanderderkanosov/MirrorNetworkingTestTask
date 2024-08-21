using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private Camera _playerCamera;

    [Header("Barrel settings")]
    [SerializeField] private Transform _barrel;
    [SerializeField] private float _barrelAimSmoothing = 10.0f;
    [SerializeField] private Vector2 _barrelVerticalRotationConstraints;

    private float _targetVerticalRotation = 0.0f;

    private void Update() {

        Rotate();
        VerticalAiming();
    }

    private void Rotate() {
        Ray ray = _playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100)) {
            Vector3 lookRotation = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(lookRotation);
        }
    }

    private void VerticalAiming() {
        _targetVerticalRotation = Mathf.Clamp(_targetVerticalRotation + Input.mouseScrollDelta.y, _barrelVerticalRotationConstraints.x, _barrelVerticalRotationConstraints.y);
        var targetRotation = Quaternion.Euler(Vector3.right * _targetVerticalRotation);
        _barrel.transform.localRotation = Quaternion.Lerp(_barrel.transform.localRotation, targetRotation, _barrelAimSmoothing * Time.deltaTime);
    }
}
