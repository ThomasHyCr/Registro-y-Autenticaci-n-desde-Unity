using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfileUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text textScore;
    [SerializeField] private GameObject panelPerfil;
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private GameObject panelLeaderboard;

    private void OnEnable()
    {
        ActualizarTextoScore();
    }

    public void MostrarPerfil()
    {
        ActualizarTextoScore();

        if (panelPerfil != null) panelPerfil.SetActive(true);
        if (panelLeaderboard != null) panelLeaderboard.SetActive(false);
    }

    private void ActualizarTextoScore()
    {
        if (textScore != null)
            textScore.text = $"Score: {SessionManager.Score}";
    }

    public void OnClickIrAlJuego()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnClickLogout()
    {
        if (FirebaseManager.Instance != null)
            FirebaseManager.Instance.Auth.SignOut();

        SessionManager.LimpiarScore();
        Debug.Log("Sesión cerrada. Volviendo a Login.");
        SceneManager.LoadScene("Login");
    }

    public void OnClickVerRanking()
    {
        if (panelPerfil != null) panelPerfil.SetActive(false);
        if (panelLeaderboard != null) panelLeaderboard.SetActive(true);
    }
}