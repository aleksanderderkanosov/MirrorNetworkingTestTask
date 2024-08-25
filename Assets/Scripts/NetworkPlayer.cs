using Mirror;
using System;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Cannon _cannon;
    [SerializeField] private Goal _goal;
    [SerializeField] private Ball _ballPrefab;

    [SyncVar(hook = nameof(UpdateScores))]
    private int _score = 0;

    [SyncVar(hook = nameof(UpdateColor))]
    private Color _playerColor = Color.clear;

    private Transform _spawnPosition;

    public int Score { get => _score; set => _score = value; }
    public Color PlayerColor { get => _playerColor; set => _playerColor = value; }
    public Camera PlayerCamera { get => _playerCamera; set => _playerCamera = value; }

    /// <summary>
    /// Blocks one of the <see cref="NetworkManager.startPositions" /> occupied by the player.
    /// </summary>
    public Transform SpawnPosition {
        get => _spawnPosition;
        set {
            NetworkManager.UnRegisterStartPosition(value);
            if (_spawnPosition != null) {
                NetworkManager.RegisterStartPosition(_spawnPosition);
            }
            
            _spawnPosition = value;
        }
    }

    /// <summary>
    /// Event triggered when a player's <see cref="Score"/> changes.
    /// </summary>
    public Action<int> OnScoresUpdates; 

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();

        ToggleComponentsOnLocalPlayer(true);
        SetLayerForWall(3);
    }

    public override void OnStopLocalPlayer() {
        base.OnStopLocalPlayer();

        SetLayerForWall(0);
    }

    /// <summary>
    /// Toggles components depending on the player's <see cref="NetworkBehaviour.isLocalPlayer"></see> property.
    /// </summary>
    private void ToggleComponentsOnLocalPlayer(bool isEnabled) {
        _cannon.Owner = this;
        _cannon.enabled = isEnabled;

        _goal.Owner = this;
        _goal.enabled = isEnabled;

        PlayerCamera.enabled = isEnabled;
        PlayerCamera.GetComponent<AudioListener>().enabled = isEnabled;
    }

    /// <summary>
    /// Make the nearest wall transparent/opaque for <see cref="_cannon"/> aiming.
    /// </summary>
    /// <param name="layer">Layer number: 0 for Default, 3 for "transparent" PlayerWall.</param>
    private void SetLayerForWall(int layer) {
        Vector3 direction = (_cannon.transform.position - PlayerCamera.transform.position).normalized;
        Ray ray = new Ray(PlayerCamera.transform.position, direction);
        if (Physics.Raycast(ray, out RaycastHit hit, 50)) {
            hit.collider.gameObject.layer = layer;
        }
    }

    /// <summary>
    /// Hooks the <see cref="Score"/> networking changes.
    /// </summary>
    private void UpdateScores(int oldValue, int newValue) {
        OnScoresUpdates?.Invoke(newValue);
    }

    /// <summary>
    /// Hooks the <see cref="PlayerColor"/> networking changes.
    /// </summary>
    private void UpdateColor(Color oldValue, Color newValue) {
        _cannon.SetBarrelColor(newValue);
        _goal.SetNetColor(newValue);
    }

    /// <summary>
    /// Calculating points after a goal. If it is an own goal, no points are added.
    /// </summary>
    private void CalculateScores(NetworkPlayer loser, NetworkPlayer scorer) {
        loser.Score--;

        if (loser != scorer) {
            scorer.Score++;
        }
    }

    /// <summary>
    /// Command for server to shoot: spawn the new ball and give it a kick.
    /// </summary>
    /// <param name="origin">Spawn position</param>
    /// <param name="force">Kick force</param>
    /// <param name="sender">Client-shooter</param>
    [Command]
    public void CmdRegisterShoot(Vector3 origin, Vector3 force, NetworkConnectionToClient sender = null) {
        var ball = Instantiate(_ballPrefab, origin, Quaternion.identity);
        NetworkServer.Spawn(ball.gameObject);
        ball.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        ball.Owner = sender.identity.GetComponent<NetworkPlayer>();
    }

    /// <summary>
    /// Command for server to register a goal and despawn scored ball.
    /// </summary>
    /// <param name="ball">Scored ball</param>
    /// <param name="sender">The client-loser who missed the goal.</param>
    [Command]
    public void CmdRegisterGoal(GameObject ball, NetworkConnectionToClient sender = null) {
        CalculateScores(sender.identity.GetComponent<NetworkPlayer>(), ball.GetComponent<Ball>().Owner);

        NetworkServer.Destroy(ball);
    }
}
