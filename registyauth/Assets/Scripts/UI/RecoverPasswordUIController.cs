using TMPro;
using UnityEngine;

public class RecoverPasswordUIController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputEmail;
    [SerializeField] private TMP_Text textMensaje;
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private GameObject panelRecuperar;
    [SerializeField] private GameObject panelLogin;

    private void OnEnable()
    {
        LimpiarCampos();
    }

    public void OnClickEnviar()
    {
        string email = inputEmail.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            MostrarMensaje("Escribe tu correo electrónico.");
            return;
        }

        SetLoading(true);
        FirebaseManager.Instance.RecuperarPassword(email,
            onSuccess: () =>
            {
                SetLoading(false);
                MostrarMensaje("Listo. Revisa tu correo para restablecer la contraseña.");
            },
            onError: (err) =>
            {
                SetLoading(false);
                MostrarMensaje(err);
            });
    }

    public void OnClickVolver()
    {
        LimpiarCampos();

        if (panelRecuperar != null) panelRecuperar.SetActive(false);
        if (panelLogin != null) panelLogin.SetActive(true);
    }

    private void MostrarMensaje(string msg)
    {
        if (textMensaje != null)
        {
            textMensaje.text = msg;
            textMensaje.gameObject.SetActive(true);
        }
    }

    private void SetLoading(bool loading)
    {
        if (loadingIndicator != null) loadingIndicator.SetActive(loading);
    }

    private void LimpiarCampos()
    {
        if (inputEmail != null) inputEmail.text = string.Empty;
        if (textMensaje != null)
        {
            textMensaje.text = string.Empty;
            textMensaje.gameObject.SetActive(false);
        }
    }
}