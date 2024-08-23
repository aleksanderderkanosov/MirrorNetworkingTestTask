using Mirror;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    [SerializeField] private Rigidbody _ballRb;
    [SerializeField] private float _timeToDestroy;

    private bool _isSleeping = false;
    private float _timerToDestroy = 0.0f;
    private NetworkPlayer _owner;

    public NetworkPlayer Owner { get => _owner; set => _owner = value; }

    private void FixedUpdate() {
        if (IsReadyToDestroy()) {
            DestroyBall();
        }
    }

    private bool IsReadyToDestroy() {
        if (!isServer) {
            return false;
        }

        if (_isSleeping != _ballRb.IsSleeping()) {
            _isSleeping = !_isSleeping;
            _timerToDestroy = 0.0f;
        }

        if (_isSleeping) {
            _timerToDestroy += Time.fixedDeltaTime;
        }

        return _timerToDestroy >= _timeToDestroy;
    }

    private void DestroyBall() {
        NetworkServer.Destroy(gameObject);
    }
}
