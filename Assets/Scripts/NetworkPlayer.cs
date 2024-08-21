using Mirror;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private Cannon _cannon;
    [SerializeField] private Camera _playerCamera;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();

        _cannon.enabled = true;
        _playerCamera.enabled = true;
        _playerCamera.GetComponent<AudioListener>().enabled = true;
    }
}
