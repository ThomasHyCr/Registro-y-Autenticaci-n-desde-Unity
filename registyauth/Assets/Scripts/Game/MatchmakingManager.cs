using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class MatchmakingManager : MonoBehaviour
{
    public static MatchmakingManager Instance { get; private set; }

    private DatabaseReference dbRoot;
    private string miPartidaId = null;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        dbRoot = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void BuscarPartida(Action<string, string> onEmparejado, Action<string> onEsperando)
    {
        string miUid = FirebaseManager.Instance.Auth.CurrentUser.UserId;

        FirebaseManager.Instance.ObtenerDatosUsuario(miUid, onSuccess: (misDatos) =>
        {
            dbRoot.Child("matchmaking").Child("cola").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted) return;

                DataSnapshot cola = task.Result;
                string rivalUid = null;

                foreach (var hijo in cola.Children)
                {
                    if (hijo.Key != miUid) { rivalUid = hijo.Key; break; }
                }

                if (rivalUid != null)
                {
                    // Había alguien esperando: armamos la partida
                    string partidaId = Guid.NewGuid().ToString();
                    var datosPartida = new Dictionary<string, object>
                    {
                        { "jugador1", rivalUid },
                        { "jugador2", miUid },
                        { "estado", "esperando" }
                    };

                    var actualizaciones = new Dictionary<string, object>
                    {
                        { $"matchmaking/partidas/{partidaId}", datosPartida },
                        { $"matchmaking/cola/{rivalUid}", null },
                        { $"matchmaking/cola/{miUid}", null }
                    };

                    dbRoot.UpdateChildrenAsync(actualizaciones).ContinueWithOnMainThread(_ =>
                    {
                        miPartidaId = partidaId;
                        onEmparejado?.Invoke(partidaId, rivalUid);
                    });
                }
                else
                {
                    // Nadie esperando: me anoto en la cola y espero
                    var miEntrada = new Dictionary<string, object>
                    {
                        { "username", misDatos.username },
                        { "timestamp", ServerValue.Timestamp }
                    };

                    dbRoot.Child("matchmaking").Child("cola").Child(miUid)
                        .SetValueAsync(miEntrada).ContinueWithOnMainThread(_ =>
                        {
                            onEsperando?.Invoke(miUid);
                            EscucharSiMeEmparejan(miUid, onEmparejado);
                        });
                }
            });
        }, onError: (err) => Debug.LogWarning(err));
    }

    private void EscucharSiMeEmparejan(string miUid, Action<string, string> onEmparejado)
    {
        dbRoot.Child("matchmaking").Child("partidas").ChildAdded += (sender, args) =>
        {
            if (args.DatabaseError != null || args.Snapshot == null) return;

            string jugador1 = args.Snapshot.Child("jugador1").Value?.ToString();
            string jugador2 = args.Snapshot.Child("jugador2").Value?.ToString();

            if (jugador1 == miUid || jugador2 == miUid)
            {
                string rival = jugador1 == miUid ? jugador2 : jugador1;
                miPartidaId = args.Snapshot.Key;
                onEmparejado?.Invoke(miPartidaId, rival);
            }
        };
    }

    public void CancelarBusqueda()
    {
        string miUid = FirebaseManager.Instance.Auth.CurrentUser.UserId;
        dbRoot.Child("matchmaking").Child("cola").Child(miUid).RemoveValueAsync();
    }
}