using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnlineUsersUIController : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject filaPrefab;
    [SerializeField] private GameObject panelOnline;
    [SerializeField] private GameObject panelPerfil;

    private Dictionary<string, GameObject> filasPorUid = new Dictionary<string, GameObject>();
    private bool escuchando = false;

    void OnEnable()
    {
        if (!escuchando)
        {
            PresenceManager.Instance.EscucharUsuariosEnLinea(ActualizarFila);
            escuchando = true;
        }
    }

    private void ActualizarFila(string uid, string username, string state)
    {
        string miUid = FirebaseManager.Instance.Auth.CurrentUser.UserId;
        if (uid == miUid) return; // no te muestres a vos mismo en la lista

        if (state == "offline")
        {
            if (filasPorUid.TryGetValue(uid, out var filaVieja))
            {
                Destroy(filaVieja);
                filasPorUid.Remove(uid);
            }
            return;
        }

        if (filasPorUid.ContainsKey(uid)) return; // ya está listado

        var fila = Instantiate(filaPrefab, contentParent);
        var texts = fila.GetComponentsInChildren<TMP_Text>();
        if (texts.Length > 0) texts[0].text = username;

        var boton = fila.GetComponentInChildren<UnityEngine.UI.Button>();
        if (boton != null)
        {
            boton.onClick.AddListener(() =>
            {
                FriendRequestsManager.Instance.EnviarSolicitud(uid, username,
                    onSuccess: () => Debug.Log("Solicitud enviada a " + username),
                    onError: (err) => Debug.LogWarning(err));
            });
        }

        filasPorUid[uid] = fila;
    }

    public void OnClickVolver()
    {
        if (panelOnline != null) panelOnline.SetActive(false);
        if (panelPerfil != null) panelPerfil.SetActive(true);
    }
}