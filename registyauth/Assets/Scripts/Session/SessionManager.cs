using System;
using UnityEngine;

public static class SessionManager
{
    private const string TOKEN_KEY = "auth_token";
    private const string USERNAME_KEY = "auth_username";
    private const string SCORE_KEY = "game_score";

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

    public static int Score
    {
        get => PlayerPrefs.GetInt(SCORE_KEY, 0);
        private set => PlayerPrefs.SetInt(SCORE_KEY, value);
    }

    public static bool HayTokenGuardado => !string.IsNullOrEmpty(Token);

    public static void GuardarSesion(string username, string token)
    {
        Username = username;
        Token = token;
        PlayerPrefs.Save();
    }

    public static void GuardarScore(int score)
    {
        Score = score;
        PlayerPrefs.Save();
    }

    public static void SincronizarScoreDesdeUsuario(UsuarioData usuario)
    {
        int scoreActual = 0;

        if (usuario != null && usuario.data != null && usuario.data.TryGetValue("score", out var scoreValue))
        {
            try
            {
                scoreActual = Convert.ToInt32(scoreValue);
            }
            catch (Exception)
            {
                scoreActual = 0;
            }
        }

        GuardarScore(scoreActual);
    }

    public static void CerrarSesion()
    {
        PlayerPrefs.DeleteKey(TOKEN_KEY);
        PlayerPrefs.DeleteKey(USERNAME_KEY);
        PlayerPrefs.DeleteKey(SCORE_KEY);
        PlayerPrefs.Save();
    }
}