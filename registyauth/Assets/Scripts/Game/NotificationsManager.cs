using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationsManager : MonoBehaviour
{
    public static NotificationsManager Instance { get; private set; }

    [SerializeField] private GameObject panelNotificacion;
    [SerializeField] private TMP_Text textNotificacion;
    [SerializeField] private float duracionSegundos = 3f;

    private HashSet<string> amigosEscuchados = new HashSet<string>();
    private Dictionary<string, string> ultimoEstadoConocido = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (panelNotificacion != null) panelNotificacion.SetActive(false);
    }

    public void EscucharAmigo(string uidAmigo, string usernameAmigo)
    {
        if (amigosEscuchados.Contains(uidAmigo)) return;
        amigosEscuchados.Add(uidAmigo);

        // PresenceManager lo programa tu compañero — vos solo lo usás, ya expone
        // exactamente este método para que lo consumas sin tocar su código.
        PresenceManager.Instance.EscucharEstadoDeUsuario(uidAmigo, (nuevoEstado) =>
        {
            if (ultimoEstadoConocido.TryGetValue(uidAmigo, out var estadoAnterior) && estadoAnterior == nuevoEstado)
                return; // sin cambio real

            ultimoEstadoConocido[uidAmigo] = nuevoEstado;

            string mensaje = nuevoEstado == "online"
                ? $"{usernameAmigo} se conectó"
                : $"{usernameAmigo} se desconectó";

            MostrarNotificacion(mensaje);
        });
    }

    private void MostrarNotificacion(string mensaje)
    {
        if (textNotificacion != null) textNotificacion.text = mensaje;
        if (panelNotificacion != null) panelNotificacion.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(OcultarDespuesDe(duracionSegundos));
    }

    private IEnumerator OcultarDespuesDe(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        if (panelNotificacion != null) panelNotificacion.SetActive(false);
    }
}