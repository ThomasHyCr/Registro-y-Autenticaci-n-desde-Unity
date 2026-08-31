using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Score de la escena Game")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Character player;

    private void Awake()
    {
        if (player == null)
            player = FindFirstObjectByType<Character>();

        if (player != null)
            player.OnScoreChanged += ActualizarTextoScore;

        ActualizarTextoScore();
    }

    private void OnDestroy()
    {
        if (player != null)
            player.OnScoreChanged -= ActualizarTextoScore;
    }

    public void ActualizarTextoScore()
    {
        if (scoreText == null)
            return;

        int scoreActual = player != null ? player.score : 0;
        scoreText.text = scoreActual.ToString();
    }

    public void CambiarALogin()
    {
        if (player != null)
            SessionManager.GuardarScore(player.score);

        SceneManager.LoadScene("Login");
    }

    public void RecargarGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
