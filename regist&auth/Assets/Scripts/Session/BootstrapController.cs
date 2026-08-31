using UnityEngine;

public class BootstrapController : MonoBehaviour
{
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private GameObject panelPerfil;

    void Start()
    {
        if (SessionManager.HayTokenGuardado)
        {
            if (panelLogin != null)
                panelLogin.SetActive(false);

            if (panelPerfil != null)
                panelPerfil.SetActive(true);

            StartCoroutine(ApiManager.Instance.ObtenerPerfil(
                SessionManager.Username, SessionManager.Token,
                onSuccess: (usuario) =>
                {
                    var profileController = FindFirstObjectByType<ProfileUIController>();
                    if (profileController != null)
                    {
                        profileController.MostrarPerfil();
                    }
                },
                onError: (err) =>
                {
                    SessionManager.CerrarSesion();

                    if (panelPerfil != null)
                        panelPerfil.SetActive(false);

                    if (panelLogin != null)
                        panelLogin.SetActive(true);
                }));
        }
        else
        {
            if (panelPerfil != null)
                panelPerfil.SetActive(false);

            if (panelLogin != null)
                panelLogin.SetActive(true);
        }
    }
}