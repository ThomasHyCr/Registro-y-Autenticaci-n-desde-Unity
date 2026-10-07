using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfileUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text textScore;
    [SerializeField] private GameObject panelPerfil;
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private GameObject panelLeaderboard;
    [SerializeField] private GameObject panelUsuariosOnline;
    [SerializeField] private GameObject panelSolicitudes;

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
        PresenceManager.Instance.MarcarComoOffline(() =>
        {
            FirebaseManager.Instance.Auth.SignOut();
            SessionManager.LimpiarScore();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
        });
    }

    public void OnClickVerRanking()
    {
        if (panelPerfil != null) panelPerfil.SetActive(false);
        if (panelLeaderboard != null) panelLeaderboard.SetActive(true);
    }
    public void OnClickVerUsuariosOnline()
    {
        if (panelPerfil != null) panelPerfil.SetActive(false);
        if (panelUsuariosOnline != null) panelUsuariosOnline.SetActive(true);
    }

    public void OnClickVerSolicitudes()
    {
        if (panelPerfil != null) panelPerfil.SetActive(false);
        if (panelSolicitudes != null) panelSolicitudes.SetActive(true);
    }
}