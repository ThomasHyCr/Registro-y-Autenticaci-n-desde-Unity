using TMPro;
using UnityEngine;

public class LeaderboardUIController : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject filaPrefab;
    [SerializeField] private GameObject panelLeaderboard;
    [SerializeField] private GameObject panelPerfil;

    void OnEnable()
    {
        CargarRanking();
    }

    private void CargarRanking()
    {
        foreach (Transform child in contentParent) Destroy(child.gameObject);

        FirebaseManager.Instance.ObtenerLeaderboard(50,
            onSuccess: (usuarios) =>
            {
                for (int i = 0; i < usuarios.Count; i++)
                {
                    var fila = Instantiate(filaPrefab, contentParent);
                    var texts = fila.GetComponentsInChildren<TMP_Text>();
                    texts[0].text = (i + 1).ToString();
                    texts[1].text = usuarios[i].username;
                    texts[2].text = usuarios[i].score.ToString();
                }
            },
            onError: (err) => Debug.LogWarning("Error listando usuarios: " + err));
    }

    public void OnClickVolver()
    {
        if (panelLeaderboard != null) panelLeaderboard.SetActive(false);
        if (panelPerfil != null) panelPerfil.SetActive(true);
    }
}