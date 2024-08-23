using Mirror;
using System;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Cannon _cannon;
    [SerializeField] private Goal _goal;
    [SerializeField] private Ball _ballPrefab;

    [SyncVar(hook = nameof(UpdateScores))]
    private int _score = 0;

    [SyncVar(hook = nameof(UpdateColor))]
    private Color _playerColor = Color.clear;

    public int Score { get => _score; set => _score = value; }
    public Color PlayerColor { get => _playerColor; set => _playerColor = value; }
    public Camera PlayerCamera { get => _playerCamera; set => _playerCamera = value; }

    public Action<int> OnScoresUpdates; 

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();

        ToggleComponentsOnLocalPlayer(true);
        SetLayerForWall();
    }

    private void ToggleComponentsOnLocalPlayer(bool isEnabled) {
        _cannon.Player = this;
        _cannon.enabled = isEnabled;

        _goal.Player = this;
        _goal.enabled = isEnabled;

        PlayerCamera.enabled = isEnabled;
        PlayerCamera.GetComponent<AudioListener>().enabled = isEnabled;
    }

    private void SetLayerForWall() {
        Vector3 direction = (_cannon.transform.position - PlayerCamera.transform.position).normalized;
        Ray ray = new Ray(PlayerCamera.transform.position, direction);
        if (Physics.Raycast(ray, out RaycastHit hit, 50)) {
            hit.collider.gameObject.layer = 2;
        }
    }

    private void UpdateScores(int oldValue, int newValue) {
        OnScoresUpdates?.Invoke(newValue);
    }

    private void UpdateColor(Color oldValue, Color newValue) {
        _cannon.SetBarrelColor(newValue);
        _goal.SetNetColor(newValue);
    }

    private void Update() {
        if (!isLocalPlayer) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            PlayerColor = Color.red;
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            PlayerColor = Color.green;
        }
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
