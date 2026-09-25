using UnityEngine;

public static class SessionManager
{
    private const string SCORE_KEY = "game_score";

    public static int Score
    {
        get => PlayerPrefs.GetInt(SCORE_KEY, 0);
        private set => PlayerPrefs.SetInt(SCORE_KEY, value);
    }

    public static void GuardarScore(int score)
    {
        Score = score;
        PlayerPrefs.Save();
    }

    public static void LimpiarScore()
    {
        PlayerPrefs.DeleteKey(SCORE_KEY);
        PlayerPrefs.Save();
    }
}