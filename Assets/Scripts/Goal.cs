using System.Collections;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private NetworkPlayer _player;
    [SerializeField] private Collider _goalTrigger;

    [Header("Moving settings")]
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private Vector2 _movingXLimits;

    private IEnumerator Start() {
        _goalTrigger.enabled = true;
        float pingPongLength = Mathf.Abs(_movingXLimits.x) + Mathf.Abs(_movingXLimits.y);
        while (enabled) {
            float time = Mathf.PingPong(Time.time * _speed, pingPongLength);
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(time - _movingXLimits.y, transform.localPosition.y, transform.localPosition.z), Time.deltaTime);
            yield return null;
        }

        yield return null;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Ball ball)) {
            _player.CmdGoal(ball.gameObject);
        }
    }
}
