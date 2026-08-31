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
