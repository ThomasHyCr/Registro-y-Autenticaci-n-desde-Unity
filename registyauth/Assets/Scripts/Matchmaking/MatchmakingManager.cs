using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class MatchmakingManager : MonoBehaviour
{
    public static MatchmakingManager Instance { get; private set; }

    private DatabaseReference Root => FirebaseDatabase.DefaultInstance.RootReference;

    private bool buscando = false;
    private bool emparejado = false;
    private string miUid;
    private string miUsername;

    private Action<string, string, string> alEmparejar; // (partidaId, rivalUid, rivalUsername)

    private DatabaseReference refMiEntradaCola;
    private DatabaseReference refMiAsignacion;
    private EventHandler<ValueChangedEventArgs> handlerAsignacion;
    private DatabaseReference refCola;
    private EventHandler<ChildChangedEventArgs> handlerCola;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (buscando) CancelarBusqueda();
        if (Instance == this) Instance = null;
    }

    public void BuscarPartida(Action<string, string, string> alEmparejarCallback,
        Action alEsperarCallback, Action<string> alErrorCallback)
    {
        if (buscando) return;
        buscando = true;
        emparejado = false;
        alEmparejar = alEmparejarCallback;

        miUid = FirebaseManager.Instance.Auth.CurrentUser.UserId;

        FirebaseManager.Instance.ObtenerDatosUsuario(miUid,
            onSuccess: (datos) =>
            {
                miUsername = datos.username;

                // 0) Borrar cualquier aviso viejo que haya quedado en mi buzón (evita falsos emparejamientos)
                Root.Child("matchmaking").Child("asignaciones").Child(miUid).RemoveValueAsync()
                    .ContinueWithOnMainThread(_ =>
                    {
                        if (!buscando) return; // se canceló mientras tanto

                        // 1) Escuchar mi buzón
                        EscucharMiAsignacion();

                        // 2) Anotarme en la cola (con auto-limpieza si me desconecto)
                        refMiEntradaCola = Root.Child("matchmaking").Child("cola").Child(miUid);
                        refMiEntradaCola.OnDisconnect().RemoveValue();

                        var entrada = new Dictionary<string, object>
                        {
                            { "username", miUsername },
                            { "timestamp", ServerValue.Timestamp }
                        };

                        refMiEntradaCola.SetValueAsync(entrada).ContinueWithOnMainThread(task =>
                        {
                            if (task.IsFaulted || task.IsCanceled)
                            {
                                CancelarBusqueda();
                                alErrorCallback?.Invoke("No se pudo entrar a la cola de matchmaking.");
                                return;
                            }

                            alEsperarCallback?.Invoke();

                            // 3) Mirar la cola para encontrar rival
                            EscucharCola();
                        });
                    });
            },
            onError: (err) =>
            {
                buscando = false;
                alErrorCallback?.Invoke(err);
            });
    }

    private void EscucharMiAsignacion()
    {
        refMiAsignacion = Root.Child("matchmaking").Child("asignaciones").Child(miUid);

        handlerAsignacion = (sender, args) =>
        {
            if (args.DatabaseError != null || args.Snapshot == null || !args.Snapshot.Exists || emparejado) return;
            emparejado = true;

            string partidaId = args.Snapshot.Child("partidaId").Value?.ToString();
            string rivalUid = args.Snapshot.Child("rivalUid").Value?.ToString();
            string rivalUsername = args.Snapshot.Child("rivalUsername").Value?.ToString() ?? "???";

            refMiAsignacion.RemoveValueAsync(); // aviso consumido
            Finalizar(partidaId, rivalUid, rivalUsername);
        };

        refMiAsignacion.ValueChanged += handlerAsignacion;
    }

    private void EscucharCola()
    {
        refCola = Root.Child("matchmaking").Child("cola");

        handlerCola = (sender, args) =>
        {
            if (args.DatabaseError != null || args.Snapshot == null || emparejado) return;

            string otroUid = args.Snapshot.Key;
            if (otroUid == miUid) return;

            // Desempate: solo el jugador con uid MAYOR crea la partida
            if (string.CompareOrdinal(miUid, otroUid) <= 0) return;

            string otroUsername = args.Snapshot.Child("username").Value?.ToString() ?? "???";
            CrearPartida(otroUid, otroUsername);
        };

        refCola.ChildAdded += handlerCola;
    }

    private void CrearPartida(string rivalUid, string rivalUsername)
    {
        if (emparejado) return;
        emparejado = true;

        string partidaId = Guid.NewGuid().ToString("N");

        // Una sola escritura atómica: o se hace todo, o no se hace nada
        var actualizaciones = new Dictionary<string, object>
        {
            { $"matchmaking/partidas/{partidaId}/jugador1", rivalUid },
            { $"matchmaking/partidas/{partidaId}/jugador2", miUid },
            { $"matchmaking/partidas/{partidaId}/estado", "esperando" },
            { $"matchmaking/cola/{rivalUid}", null },
            { $"matchmaking/cola/{miUid}", null },
            { $"matchmaking/asignaciones/{rivalUid}/partidaId", partidaId },
            { $"matchmaking/asignaciones/{rivalUid}/rivalUid", miUid },
            { $"matchmaking/asignaciones/{rivalUid}/rivalUsername", miUsername }
        };

        Root.UpdateChildrenAsync(actualizaciones).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                emparejado = false; // permitir reintento con el siguiente evento
                return;
            }

            Finalizar(partidaId, rivalUid, rivalUsername);
        });
    }

    private void Finalizar(string partidaId, string rivalUid, string rivalUsername)
    {
        refMiEntradaCola?.OnDisconnect().Cancel(); // ya no estoy en la cola
        QuitarListeners();
        buscando = false;

        alEmparejar?.Invoke(partidaId, rivalUid, rivalUsername);
    }

    public void CancelarBusqueda()
    {
        if (!buscando) return;

        QuitarListeners();

        if (refMiEntradaCola != null)
        {
            refMiEntradaCola.OnDisconnect().Cancel();
            refMiEntradaCola.RemoveValueAsync();
        }

        buscando = false;
        emparejado = false;
    }

    private void QuitarListeners()
    {
        if (refMiAsignacion != null && handlerAsignacion != null)
            refMiAsignacion.ValueChanged -= handlerAsignacion;

        if (refCola != null && handlerCola != null)
            refCola.ChildAdded -= handlerCola;

        handlerAsignacion = null;
        handlerCola = null;
    }
}