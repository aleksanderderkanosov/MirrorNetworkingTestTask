using System.Collections;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private Vector2 _movingXLimits;

    private IEnumerator Start() {
        while (enabled) {
            float time = Mathf.PingPong(Time.time * _speed, 30);
            transform.localPosition = new Vector3(time - _movingXLimits.y, transform.localPosition.y, transform.localPosition.z);
            yield return null;
        }

        yield return null;
    }
}
