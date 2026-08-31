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
                LimpiarCampos();
                // El registro no devuelve token (ver diagrama 1),
                // así que volvemos al panel de Login para que el usuario inicie sesión.
                if (panelRegistro != null)
                    panelRegistro.SetActive(false);

                if (panelLogin != null)
                    panelLogin.SetActive(true);


                Debug.Log($"[REGISTRO] user='{username}' (len={username.Length}) pass='{password}' pass len={password.Length}");

            
            },
            onError: (err) =>
            {
                SetLoading(false);
                MostrarError(err);
            }));
    }

    public void OnClickVolver()
    {
        LimpiarCampos();

        if (panelRegistro != null)
            panelRegistro.SetActive(false);

        if (panelLogin != null)
            panelLogin.SetActive(true);
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