using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance { get; private set; }

    [SerializeField] private string baseUrl = "https://sid-restapi.onrender.com";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ---------- 1. REGISTRO ----------
    public IEnumerator Registrar(string username, string password,
        Action<UsuarioData> onSuccess, Action<string> onError)
    {
        var body = new CredencialesRequest { username = username, password = password };
        yield return Post($"{baseUrl}/api/usuarios", body, null,
            (json) =>
            {
                var res = JsonConvert.DeserializeObject<UsuarioResponse>(json);
                onSuccess?.Invoke(res.usuario);
            },
            onError);
    }

    // ---------- 2. LOGIN ----------
    public IEnumerator Login(string username, string password,
        Action<UsuarioData, string> onSuccess, Action<string> onError)
    {
        var body = new CredencialesRequest { username = username, password = password };
        yield return Post($"{baseUrl}/api/auth/login", body, null,
            (json) =>
            {
                var res = JsonConvert.DeserializeObject<LoginResponse>(json);
                onSuccess?.Invoke(res.usuario, res.token);
            },
            onError);
    }

    // ---------- 3. OBTENER PERFIL ----------
    public IEnumerator ObtenerPerfil(string username, string token,
        Action<UsuarioData> onSuccess, Action<string> onError)
    {
        string url = $"{baseUrl}/api/usuarios?username={UnityWebRequest.EscapeURL(username)}";
        yield return Get(url, token,
            (json) =>
            {
                var res = JsonConvert.DeserializeObject<UsuarioResponse>(json);
                onSuccess?.Invoke(res.usuario);
            },
            onError);
    }

    // ---------- 4. ACTUALIZAR DATA (p.ej. score) ----------
    public IEnumerator ActualizarData(string username, string token,
        System.Collections.Generic.Dictionary<string, object> data,
        Action<UsuarioData> onSuccess, Action<string> onError)
    {
        var body = new ActualizarDataRequest { username = username, data = data };
        yield return Patch($"{baseUrl}/api/usuarios", body, token,
            (json) =>
            {
                var res = JsonConvert.DeserializeObject<UsuarioResponse>(json);
                onSuccess?.Invoke(res.usuario);
            },
            onError);
    }

    // ---------- 5. LISTAR USUARIOS ----------
    public IEnumerator ListarUsuarios(string token, int limit, int skip, bool sort,
        Action<System.Collections.Generic.List<UsuarioData>> onSuccess, Action<string> onError)
    {
        string url = $"{baseUrl}/api/usuarios?limit={limit}&skip={skip}&sort={sort.ToString().ToLower()}";
        yield return Get(url, token,
            (json) =>
            {
                var res = JsonConvert.DeserializeObject<UsuariosListResponse>(json);
                onSuccess?.Invoke(res.usuarios);
            },
            onError);
    }

    // ================== Helpers HTTP genéricos ==================

    private IEnumerator Post(string url, object body, string token,
        Action<string> onSuccess, Action<string> onError)
        => SendRequest(url, "POST", body, token, onSuccess, onError);

    private IEnumerator Patch(string url, object body, string token,
        Action<string> onSuccess, Action<string> onError)
        => SendRequest(url, "PATCH", body, token, onSuccess, onError);

    private IEnumerator Get(string url, string token,
        Action<string> onSuccess, Action<string> onError)
        => SendRequest(url, "GET", null, token, onSuccess, onError);

    private IEnumerator SendRequest(string url, string method, object body, string token,
        Action<string> onSuccess, Action<string> onError)
    {
        using var req = new UnityWebRequest(url, method);

        if (body != null)
        {
            string json = JsonConvert.SerializeObject(body);
            byte[] raw = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(raw);
            req.SetRequestHeader("Content-Type", "application/json");
        }

        req.downloadHandler = new DownloadHandlerBuffer();

        if (!string.IsNullOrEmpty(token))
            req.SetRequestHeader("x-token", token);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(req.downloadHandler.text);
        }
        else
        {
            string msg = ExtraerMensajeError(req);
            onError?.Invoke(msg);
        }
    }

    private string ExtraerMensajeError(UnityWebRequest req)
    {
        try
        {
            var err = JsonConvert.DeserializeObject<ErrorResponse>(req.downloadHandler.text);
            if (err != null && !string.IsNullOrEmpty(err.Msg)) return err.Msg;
        }
        catch { /* el body no era JSON con Msg */ }

        return $"Error {req.responseCode}: {req.error}";
    }
}