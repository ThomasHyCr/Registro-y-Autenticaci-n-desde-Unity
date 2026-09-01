using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public event System.Action OnScoreChanged;

    [SerializeField] public GameObject gameOverPanel;
    [SerializeField] private GameObject koro;
    [SerializeField] private int initialScore = 0;

    private int _score;
    public int score
    {
        get => _score;
        set
        {
            if (_score == value)
                return;

            _score = value;
            SessionManager.GuardarScore(_score);

            if (!string.IsNullOrEmpty(SessionManager.Username) && !string.IsNullOrEmpty(SessionManager.Token) && ApiManager.Instance != null)
            {
                var data = new Dictionary<string, object> { { "score", _score } };
                StartCoroutine(ApiManager.Instance.ActualizarData(
                    SessionManager.Username,
                    SessionManager.Token,
                    data,
                    onSuccess: (usuario) =>
                    {
                        Debug.Log($"[Score sincronizado] usuario={usuario.username}, score={_score}");
                    },
                    onError: (err) =>
                    {
                        Debug.LogWarning("Error sincronizando score del juego: " + err);
                    }));
            }

            OnScoreChanged?.Invoke();
        }
    }

    public void toque()
    {
        if (gameOverPanel != null && gameOverPanel.activeSelf)
            return;

        gameOverPanel.SetActive(true);
        koro.GetComponent<SpriteRenderer>().enabled = false;
        Debug.Log("toque | score: " + score);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = SessionManager.Score;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
