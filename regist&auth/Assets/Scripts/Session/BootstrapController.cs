using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapController : MonoBehaviour
{
    void Start()
    {
        if (SessionManager.HayTokenGuardado)
        {
            StartCoroutine(ApiManager.Instance.ObtenerPerfil(
                SessionManager.Username, SessionManager.Token,
                onSuccess: (usuario) => SceneManager.LoadScene("Game"),
                onError: (err) =>
                {
                    SessionManager.CerrarSesion();
                    SceneManager.LoadScene("Login");
                }));
        }
        else
        {
            SceneManager.LoadScene("Login");
        }
    }
}