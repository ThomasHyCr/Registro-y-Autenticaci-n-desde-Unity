using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text textScore;
    [SerializeField] private GameObject panelPerfil;
    [SerializeField] private GameObject panelLeaderboard;
    private int scoreActual = 0;

    public void MostrarPerfil()
    {
        if (panelPerfil != null)
            panelPerfil.SetActive(true);

        if (panelLeaderboard != null)
            panelLeaderboard.SetActive(false);
    }

    public void OnClickSumarPuntos()
    {
        scoreActual += 10; // o el resultado de tu mecánica de juego

        var data = new Dictionary<string, object> { { "score", scoreActual } };

        StartCoroutine(ApiManager.Instance.ActualizarData(
            SessionManager.Username, SessionManager.Token, data,
            onSuccess: (usuario) =>
            {
                textScore.text = $"Score: {scoreActual}";
            },
            onError: (err) =>
            {
                Debug.LogWarning("Error actualizando score: " + err);
            }));
    }

    public void OnClickLogout()
    {
        SessionManager.GuardarScore(scoreActual);
        SessionManager.CerrarSesion();
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