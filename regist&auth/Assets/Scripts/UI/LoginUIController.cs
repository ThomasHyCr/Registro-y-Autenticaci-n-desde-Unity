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
        Debug.Log($"[LOGIN] user='{username}' (len={username.Length}) pass len={password.Length}");
        SetLoading(true);
        StartCoroutine(ApiManager.Instance.Login(username, password,
            onSuccess: (usuario, token) =>
            {
                SetLoading(false);
                SessionManager.GuardarSesion(usuario.username, token);
                SessionManager.SincronizarScoreDesdeUsuario(usuario);
                LimpiarCampos();

                if (panelLogin != null)
                    panelLogin.SetActive(false);

                if (panelRegistro != null)
                    panelRegistro.SetActive(false);

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
        if (textError != null)
        {
            textError.text = msg;
            textError.gameObject.SetActive(true);
        }
    }

    public void OnClickIrARegistro()
    {
        LimpiarCampos();

        if (panelLogin != null)
            panelLogin.SetActive(false);

        if (panelRegistro != null)
            panelRegistro.SetActive(true);
    }

    private void SetLoading(bool loading)
    {
        if (loadingIndicator != null)
            loadingIndicator.SetActive(loading);
    }

    private void LimpiarCampos()
    {
        if (inputUsername != null)
            inputUsername.text = string.Empty;

        if (inputPassword != null)
            inputPassword.text = string.Empty;

        if (textError != null)
        {
            textError.text = string.Empty;
            textError.gameObject.SetActive(false);
        }
    }
}