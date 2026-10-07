using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendRequestsUIController : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject filaPrefab;
    [SerializeField] private GameObject panelSolicitudes;
    [SerializeField] private GameObject panelPerfil;

    private HashSet<string> solicitudesMostradas = new HashSet<string>();
    private bool escuchando = false;

    void OnEnable()
    {
        if (!escuchando)
        {
            FriendRequestsManager.Instance.EscucharSolicitudesRecibidas(AgregarFilaSiNueva);
            escuchando = true;
        }
    }

    private void AgregarFilaSiNueva(string uidEmisor, string fromUsername)
    {
        if (solicitudesMostradas.Contains(uidEmisor)) return;
        solicitudesMostradas.Add(uidEmisor);

        var fila = Instantiate(filaPrefab, contentParent);
        var texts = fila.GetComponentsInChildren<TMP_Text>();
        if (texts.Length > 0) texts[0].text = fromUsername;

        var botones = fila.GetComponentsInChildren<Button>();
        if (botones.Length >= 2) // [0] = Aceptar, [1] = Rechazar
        {
            botones[0].onClick.AddListener(() =>
            {
                FriendRequestsManager.Instance.ResponderSolicitud(uidEmisor, true,
                    onSuccess: () => { solicitudesMostradas.Remove(uidEmisor); Destroy(fila); },
                    onError: (err) => Debug.LogWarning(err));
            });

            botones[1].onClick.AddListener(() =>
            {
                FriendRequestsManager.Instance.ResponderSolicitud(uidEmisor, false,
                    onSuccess: () => { solicitudesMostradas.Remove(uidEmisor); Destroy(fila); },
                    onError: (err) => Debug.LogWarning(err));
            });
        }
    }

    public void OnClickVolver()
    {
        if (panelSolicitudes != null) panelSolicitudes.SetActive(false);
        if (panelPerfil != null) panelPerfil.SetActive(true);
    }
}