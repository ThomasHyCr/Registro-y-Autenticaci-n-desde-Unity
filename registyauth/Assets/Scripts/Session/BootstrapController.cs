using UnityEngine;

public class BootstrapController : MonoBehaviour
{
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private GameObject panelPerfil;

    void Start()
    {
        if (FirebaseManager.Instance != null && FirebaseManager.Instance.Listo)
        {
            VerificarSesion();
        }
        else if (FirebaseManager.Instance != null)
        {
            FirebaseManager.Instance.OnFirebaseListo += VerificarSesion;
        }
        else
        {
            MostrarLogin();
        }
    }

    private void OnDestroy()
    {
        if (FirebaseManager.Instance != null)
            FirebaseManager.Instance.OnFirebaseListo -= VerificarSesion;
    }

    private void VerificarSesion()
    {
        if (FirebaseManager.Instance.Auth.CurrentUser != null)
        {
            string uid = FirebaseManager.Instance.Auth.CurrentUser.UserId;
            FirebaseManager.Instance.ObtenerDatosUsuario(uid,
                onSuccess: (datos) =>
                {
                    SessionManager.GuardarScore((int)datos.score);
                    MostrarPerfil();

                    var profileController = FindFirstObjectByType<ProfileUIController>();
                    if (profileController != null)
                        profileController.MostrarPerfil();
                },
                onError: (err) =>
                {
                    FirebaseManager.Instance.Auth.SignOut();
                    MostrarLogin();
                });
        }
        else
        {
            MostrarLogin();
        }
    }

    private void MostrarLogin()
    {
        if (panelLogin != null) panelLogin.SetActive(true);
        if (panelPerfil != null) panelPerfil.SetActive(false);
    }

    private void MostrarPerfil()
    {
        if (panelLogin != null) panelLogin.SetActive(false);
        if (panelPerfil != null) panelPerfil.SetActive(true);
    }
}