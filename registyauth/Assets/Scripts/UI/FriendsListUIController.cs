using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FriendsListUIController : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject filaPrefab;
    [SerializeField] private GameObject panelAmigos;
    [SerializeField] private GameObject panelPerfil;

    private Dictionary<string, GameObject> filasPorUid = new Dictionary<string, GameObject>();
    private bool escuchando = false;

    void OnEnable()
    {
        if (!escuchando)
        {
            FriendsListManager.Instance.EscucharListaAmigos(OnAmigoNuevo);
            escuchando = true;
        }
    }

    private void OnAmigoNuevo(string uidAmigo)
    {
        if (filasPorUid.ContainsKey(uidAmigo)) return;

        FirebaseManager.Instance.ObtenerDatosUsuario(uidAmigo,
            onSuccess: (datos) =>
            {
                var fila = Instantiate(filaPrefab, contentParent);
                var texts = fila.GetComponentsInChildren<TMP_Text>();
                if (texts.Length > 0) texts[0].text = datos.username;

                filasPorUid[uidAmigo] = fila;

                // Enganche con tu sistema de notificaciones (sección 5)
                NotificationsManager.Instance?.EscucharAmigo(uidAmigo, datos.username);
            },
            onError: (err) => Debug.LogWarning(err));
    }

    public void OnClickVolver()
    {
        if (panelAmigos != null) panelAmigos.SetActive(false);
        if (panelPerfil != null) panelPerfil.SetActive(true);
    }
}