using System.Collections;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider _goalTrigger;
    [SerializeField] private Renderer _goalNetRenderer;

    [Header("Moving settings")]
    [SerializeField] private float _speed = 1.0f;
    [Tooltip("Limitations of movement along the x-axis.")]
    [SerializeField] private Vector2 _movingXLimits;

    private NetworkPlayer _owner;
    public NetworkPlayer Owner { get => _owner; set => _owner = value; }

    private IEnumerator Start() {
        _goalTrigger.enabled = true;
        float pingPongLength = Mathf.Abs(_movingXLimits.x) + Mathf.Abs(_movingXLimits.y);
        while (enabled) {
            transform.localPosition = CalculateMovement(pingPongLength);
            yield return null;
        }

        yield return null;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Ball ball)) {
            Owner.CmdRegisterGoal(ball.gameObject);
        }
    }

    /// <summary>
    /// Change the color of goal net.
    /// </summary>
    public void SetNetColor(Color netColor) {
        _goalNetRenderer.material.color = netColor;
    }

    /// <summary>
    /// Calculate the movement of the goal along the x-axis.
    /// </summary>
    /// <param name="pingPongLength">The total length of the movement line.</param>
    private Vector3 CalculateMovement(float pingPongLength) {
        float time = Mathf.PingPong(Time.time * _speed, pingPongLength);
        return Vector3.Lerp(transform.localPosition, new Vector3(time - _movingXLimits.y, transform.localPosition.y, transform.localPosition.z), Time.deltaTime);
    }
}
