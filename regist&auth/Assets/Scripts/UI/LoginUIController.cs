using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginUIController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputUsername;
    [SerializeField] private TMP_InputField inputPassword;
    [SerializeField] private TMP_Text textError;
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private GameObject panelRegistro;
    [SerializeField] private GameObject panelPerfil;

    public void OnClickLogin()
    {
        string username = inputUsername.text.Trim();
        string password = inputPassword.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MostrarError("Completa usuario y contraseña.");
            return;
        }

        SetLoading(true);
        StartCoroutine(ApiManager.Instance.Login(username, password,
            onSuccess: (usuario, token) =>
            {
                SetLoading(false);
                SessionManager.GuardarSesion(usuario.username, token);

                if (panelLogin != null)
                    panelLogin.SetActive(false);

                if (panelPerfil != null)
                    panelPerfil.SetActive(true);
            },
            onError: (err) =>
            {
                SetLoading(false);
                MostrarError(err);
            }));
    }

    private void MostrarError(string msg)
    {
        textError.text = msg;
        textError.gameObject.SetActive(true);
    }

    public void OnClickIrARegistro()
    {
    panelLogin.SetActive(false);
    panelRegistro.SetActive(true);
    }

    private void SetLoading(bool loading)
    {
        loadingIndicator.SetActive(loading);
    }
}