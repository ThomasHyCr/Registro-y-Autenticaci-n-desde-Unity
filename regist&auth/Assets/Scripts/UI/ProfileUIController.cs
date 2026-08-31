using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text textScore;
    [SerializeField] private GameObject panelPerfil;
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private GameObject panelLeaderboard;
    private int scoreActual = 0;

    private void OnEnable()
    {
        scoreActual = SessionManager.Score;
        if (textScore != null)
            textScore.text = $"Score: {scoreActual}";
    }

    public void MostrarPerfil()
    {
        scoreActual = SessionManager.Score;

        if (textScore != null)
            textScore.text = $"Score: {scoreActual}";

        if (panelPerfil != null)
            panelPerfil.SetActive(true);

        if (panelLeaderboard != null)
            panelLeaderboard.SetActive(false);
    }

    public void OnClickSumarPuntos()
    {
        scoreActual += 10; // o el resultado de tu mecánica de juego
        SessionManager.GuardarScore(scoreActual);

        var data = new Dictionary<string, object> { { "score", scoreActual } };

        StartCoroutine(ApiManager.Instance.ActualizarData(
            SessionManager.Username, SessionManager.Token, data,
            onSuccess: (usuario) =>
            {
                if (textScore != null)
                    textScore.text = $"Score: {scoreActual}";
            },
            onError: (err) =>
            {
                Debug.LogWarning("Error actualizando score: " + err);
            }));
    }

    public void OnClickIrAlJuego()
    {
        SessionManager.GuardarScore(scoreActual);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void OnClickLogout()
    {
        SessionManager.GuardarScore(scoreActual);
        SessionManager.CerrarSesion();
        Debug.Log("Sesión cerrada. Volviendo a Login.");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
    }

    public void OnClickVerRanking()
    {
        if (panelPerfil != null)
            panelPerfil.SetActive(false);

        if (panelLeaderboard != null)
            panelLeaderboard.SetActive(true);
    }
}