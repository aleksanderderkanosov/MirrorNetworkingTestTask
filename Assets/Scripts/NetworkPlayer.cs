using Mirror;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private Cannon _cannon;
    [SerializeField] private Goal _goal;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Ball _ballPrefab;

    [SyncVar(hook = nameof(UpdateScores))] private int _score = 0;

    public int Score { get => _score; set => _score = value; }

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();

        EnableComponentsOnLocalPlayer();
        SetLayerForWall();
    }

    private void EnableComponentsOnLocalPlayer() {
        _cannon.enabled = true;
        _goal.enabled = true;
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

    private void UpdateScores(int oldValue, int newValue) {
        print($"Scores: {newValue}");
    } 

    [Command]
    public void CmdShoot(Vector3 origin, Vector3 force, NetworkConnectionToClient sender = null) {
        var ball = Instantiate(_ballPrefab, origin, Quaternion.identity);
        NetworkServer.Spawn(ball.gameObject);
        ball.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        ball.Owner = sender.identity.GetComponent<NetworkPlayer>();
    }

    [Command]
    public void CmdGoal(GameObject ball, NetworkConnectionToClient sender = null) {
        NetworkPlayer _loser = sender.identity.GetComponent<NetworkPlayer>();
        _loser.Score--;

        NetworkPlayer _scorer = ball.GetComponent<Ball>().Owner;
        if (_loser != _scorer) {
            _scorer.Score++;
        }

        NetworkServer.Destroy(ball);
    }
}
