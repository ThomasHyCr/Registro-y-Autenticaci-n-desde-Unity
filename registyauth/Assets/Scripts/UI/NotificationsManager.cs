using System;
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

    private readonly List<Action> limpiezas = new List<Action>();
    private readonly HashSet<string> amigosEscuchados = new HashSet<string>();
    private readonly Dictionary<string, string> ultimoEstado = new Dictionary<string, string>();
    private bool escuchando = false;
    private Coroutine rutinaOcultar;

    private void Awake()
    {
        Instance = this;
        if (panelNotificacion != null) panelNotificacion.SetActive(false);
    }

    private void OnDestroy()
    {
        DetenerEscucha();
        if (Instance == this) Instance = null;
    }

    // Llamar una vez después de loguearse (o de reabrir la app con sesión guardada).
    public void IniciarEscuchaAmigos()
    {
        DetenerEscucha();
        escuchando = true;
        limpiezas.Add(FriendsListManager.EscucharListaAmigos(OnAmigoDetectado));
    }

    // Llamar antes de cerrar sesión.
    public void DetenerEscucha()
    {
        escuchando = false;

        foreach (Action limpiar in limpiezas) limpiar?.Invoke();
        limpiezas.Clear();
        amigosEscuchados.Clear();
        ultimoEstado.Clear();
    }

    private void OnAmigoDetectado(string uidAmigo)
    {
        if (!escuchando || !amigosEscuchados.Add(uidAmigo)) return;

        FirebaseManager.Instance.ObtenerDatosUsuario(uidAmigo,
            onSuccess: (datos) =>
            {
                if (this == null || !escuchando) return;

                limpiezas.Add(FriendsListManager.EscucharEstado(uidAmigo,
                    (estado) => OnEstadoAmigo(uidAmigo, datos.username, estado)));
            },
            onError: (err) => Debug.LogWarning("No se pudo cargar el amigo para notificaciones: " + err));
    }

    private void OnEstadoAmigo(string uid, string username, string estado)
    {
        // El primer valor que llega es el estado ACTUAL, no un cambio: lo guardamos sin avisar.
        if (!ultimoEstado.TryGetValue(uid, out string anterior))
        {
            ultimoEstado[uid] = estado;
            return;
        }

        if (anterior == estado) return;
        ultimoEstado[uid] = estado;

        Mostrar(estado == "online" ? $"{username} se conectó" : $"{username} se desconectó");
    }

    private void Mostrar(string mensaje)
    {
        if (textNotificacion != null) textNotificacion.text = mensaje;
        if (panelNotificacion != null) panelNotificacion.SetActive(true);

        if (rutinaOcultar != null) StopCoroutine(rutinaOcultar);
        rutinaOcultar = StartCoroutine(OcultarDespues(duracionSegundos));
    }

    private IEnumerator OcultarDespues(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        if (panelNotificacion != null) panelNotificacion.SetActive(false);
    }
}