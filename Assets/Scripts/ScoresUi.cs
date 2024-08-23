using UnityEngine;

public class ScoresUi : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _scoresText;
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
