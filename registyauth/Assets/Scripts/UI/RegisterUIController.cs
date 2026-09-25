using TMPro;
using UnityEngine;

public class RegisterUIController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputEmail;
    [SerializeField] private TMP_InputField inputPassword;
    [SerializeField] private TMP_InputField inputUsername;
    [SerializeField] private TMP_Text textError;
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private GameObject panelRegistro;
    [SerializeField] private GameObject panelLogin;

    public void OnClickRegistrar()
    {
        string email = inputEmail.text.Trim();
        string password = inputPassword.text;
        string username = inputUsername.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(username))
        {
            MostrarError("Completa correo, usuario y contraseña.");
            return;
        }

        SetLoading(true);
        FirebaseManager.Instance.Registrar(email, password, username,
            onSuccess: (datos) =>
            {
                SetLoading(false);
                LimpiarCampos();

                if (panelRegistro != null) panelRegistro.SetActive(false);
                if (panelLogin != null) panelLogin.SetActive(true);
            },
            onError: (err) =>
            {
                SetLoading(false);
                MostrarError(err);
            });
    }

    public void OnClickVolver()
    {
        LimpiarCampos();
        if (panelRegistro != null) panelRegistro.SetActive(false);
        if (panelLogin != null) panelLogin.SetActive(true);
    }

    private void MostrarError(string msg)
    {
        if (textError != null)
        {
            textError.text = msg;
            textError.gameObject.SetActive(true);
        }
    }

    private void SetLoading(bool loading)
    {
        if (loadingIndicator != null) loadingIndicator.SetActive(loading);
    }

    private void LimpiarCampos()
    {
        if (inputEmail != null) inputEmail.text = string.Empty;
        if (inputPassword != null) inputPassword.text = string.Empty;
        if (inputUsername != null) inputUsername.text = string.Empty;
        if (textError != null)
        {
            textError.text = string.Empty;
            textError.gameObject.SetActive(false);
        }
    }
}