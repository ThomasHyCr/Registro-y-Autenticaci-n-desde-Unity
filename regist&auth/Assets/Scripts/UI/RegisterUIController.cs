using TMPro;
using UnityEngine;

public class RegisterUIController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputUsername;
    [SerializeField] private TMP_InputField inputPassword;
    [SerializeField] private TMP_Text textError;
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private GameObject panelRegistro;
    [SerializeField] private GameObject panelLogin;

    public void OnClickRegistrar()
    {
        string username = inputUsername.text.Trim();
        string password = inputPassword.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MostrarError("Completa usuario y contraseña.");
            return;
        }

        SetLoading(true);
        StartCoroutine(ApiManager.Instance.Registrar(username, password,
            onSuccess: (usuario) =>
            {
                SetLoading(false);
                // El registro no devuelve token (ver diagrama 1),
                // así que volvemos al panel de Login para que el usuario inicie sesión.
                panelRegistro.SetActive(false);
                panelLogin.SetActive(true);
            },
            onError: (err) =>
            {
                SetLoading(false);
                MostrarError(err);
            }));
    }

    public void OnClickVolver()
    {
        panelRegistro.SetActive(false);
        panelLogin.SetActive(true);
    }

    private void MostrarError(string msg)
    {
        textError.text = msg;
        textError.gameObject.SetActive(true);
    }

    private void SetLoading(bool loading)
    {
        loadingIndicator.SetActive(loading);
    }
}