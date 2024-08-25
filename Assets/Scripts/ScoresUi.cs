using UnityEngine;

/// <summary>
/// Describes the player's scoreboard.
/// </summary>
public class ScoresUi : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _scoresText;
    [Tooltip("Is this scoreboard visible to the local player?")]
    [SerializeField] private bool _isVisibleOnLocal;

    private NetworkPlayer _player;

    private void OnEnable() {
        _player = GetComponentInParent<NetworkPlayer>();
        _player.OnScoresUpdates += UpdateScores;
    }

    private void OnDisable() {
        _player.OnScoresUpdates -= UpdateScores;
    }

    private void Start() {
        if (!_isVisibleOnLocal && _player.isLocalPlayer) {
            gameObject.SetActive(false);
        }
    }

    private void UpdateScores(int newValue) {
        _scoresText.text = newValue.ToString();
    }
}
