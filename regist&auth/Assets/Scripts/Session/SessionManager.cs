using UnityEngine;

public static class SessionManager
{
    private const string TOKEN_KEY = "auth_token";
    private const string USERNAME_KEY = "auth_username";

    public static string Token
    {
        get => PlayerPrefs.GetString(TOKEN_KEY, "");
        private set => PlayerPrefs.SetString(TOKEN_KEY, value);
    }

    public static string Username
    {
        get => PlayerPrefs.GetString(USERNAME_KEY, "");
        private set => PlayerPrefs.SetString(USERNAME_KEY, value);
    }

    public static bool HayTokenGuardado => !string.IsNullOrEmpty(Token);

    public static void GuardarSesion(string username, string token)
    {
        Username = username;
        Token = token;
        PlayerPrefs.Save();
    }

    public static void CerrarSesion()
    {
        PlayerPrefs.DeleteKey(TOKEN_KEY);
        PlayerPrefs.DeleteKey(USERNAME_KEY);
        PlayerPrefs.Save();
    }
}