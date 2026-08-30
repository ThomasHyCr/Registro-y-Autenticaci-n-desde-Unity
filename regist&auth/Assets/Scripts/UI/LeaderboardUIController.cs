using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardUIController : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject filaPrefab;

    void OnEnable()
    {
        CargarRanking();
    }

    private void CargarRanking()
    {
        foreach (Transform child in contentParent) Destroy(child.gameObject);

        StartCoroutine(ApiManager.Instance.ListarUsuarios(
            SessionManager.Token, limit: 50, skip: 0, sort: true,
            onSuccess: (usuarios) =>
            {
                // Ordenar de mayor a menor por score, por si la API no lo hace
                var ordenados = usuarios
                    .OrderByDescending(u => ObtenerScore(u))
                    .ToList();

                for (int i = 0; i < ordenados.Count; i++)
                {
                    var fila = Instantiate(filaPrefab, contentParent);
                    var texts = fila.GetComponentsInChildren<TMPro.TMP_Text>();
                    texts[0].text = (i + 1).ToString();          // posición
                    texts[1].text = ordenados[i].username;        // nombre
                    texts[2].text = ObtenerScore(ordenados[i]).ToString(); // score
                }
            },
            onError: (err) => Debug.LogWarning("Error listando usuarios: " + err)));
    }

    private int ObtenerScore(UsuarioData u)
    {
        if (u.data != null && u.data.TryGetValue("score", out var s))
        {
            return System.Convert.ToInt32(s);
        }
        return 0;
    }
}