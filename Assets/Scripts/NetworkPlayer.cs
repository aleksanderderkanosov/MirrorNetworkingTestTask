using Mirror;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private Cannon _cannon;
    [SerializeField] private Camera _playerCamera;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();

        EnableComponentsOnLocalPlayer();
        SetLayerForWall();
    }

    private void EnableComponentsOnLocalPlayer() {
        _cannon.enabled = true;
        _playerCamera.enabled = true;
        _playerCamera.GetComponent<AudioListener>().enabled = true;
    }

    private void SetLayerForWall() {
        Vector3 direction = (_cannon.transform.position - _playerCamera.transform.position).normalized;
        Ray ray = new Ray(_playerCamera.transform.position, direction);
        if (Physics.Raycast(ray, out RaycastHit hit, 50)) {
            hit.collider.gameObject.layer = 2;
        }
    }
}
