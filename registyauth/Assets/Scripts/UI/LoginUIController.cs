using TMPro;
using UnityEngine;

public class LoginUIController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputEmail;
    [SerializeField] private TMP_InputField inputPassword;
    [SerializeField] private TMP_Text textError;
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private GameObject panelRegistro;
    [SerializeField] private GameObject panelPerfil;
    [SerializeField] private GameObject panelRecuperar;

    public void OnClickLogin()
    {
        string email = inputEmail.text.Trim();
        string password = inputPassword.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            MostrarError("Completa correo y contraseña.");
            return;
        }

        SetLoading(true);
        FirebaseManager.Instance.Login(email, password,
            onSuccess: (datos) =>
            {
                SetLoading(false);
                SessionManager.GuardarScore((int)datos.score);
                LimpiarCampos();

                if (panelLogin != null) panelLogin.SetActive(false);
                if (panelRegistro != null) panelRegistro.SetActive(false);
                if (panelPerfil != null) panelPerfil.SetActive(true);

                var profileController = FindFirstObjectByType<ProfileUIController>();
                if (profileController != null)
                    profileController.MostrarPerfil();
            },
            onError: (err) =>
            {
                SetLoading(false);
                MostrarError(err);
            });
    }

    public void OnClickIrARegistro()
    {
        LimpiarCampos();
        if (panelLogin != null) panelLogin.SetActive(false);
        if (panelRegistro != null) panelRegistro.SetActive(true);
    }

    public void OnClickAbrirRecuperarPassword()
    {
        LimpiarCampos();
        if (panelLogin != null) panelLogin.SetActive(false);
        if (panelRecuperar != null) panelRecuperar.SetActive(true);
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
        if (textError != null)
        {
            textError.text = string.Empty;
            textError.gameObject.SetActive(false);
        }
    }
}