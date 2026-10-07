using System;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class PresenceManager : MonoBehaviour
{
    public static PresenceManager Instance { get; private set; }

    private DatabaseReference dbRoot;
    private DatabaseReference miStatusRef;

    public event Action<string, string> OnUsuarioCambioEstado; // (uid, state)

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void IniciarPresencia(string uid, string username)
    {
        dbRoot = FirebaseDatabase.DefaultInstance.RootReference;
        miStatusRef = dbRoot.Child("status").Child(uid);

        var datosOnline = new System.Collections.Generic.Dictionary<string, object>
        {
            { "username", username },
            { "state", "online" },
            { "last_changed", ServerValue.Timestamp }
        };

        var datosOffline = new System.Collections.Generic.Dictionary<string, object>
        {
            { "username", username },
            { "state", "offline" },
            { "last_changed", ServerValue.Timestamp }
        };

        // Le decimos al SERVIDOR qué escribir si perdemos la conexión (cierre abrupto, corte de red).
        // Esto corre en el backend de Firebase, no depende de que tu app alcance a ejecutar código.
        miStatusRef.OnDisconnect().SetValue(datosOffline).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                miStatusRef.SetValueAsync(datosOnline);
            }
        });
    }

    public void MarcarComoOffline(Action onDone = null)
    {
        if (miStatusRef == null) { onDone?.Invoke(); return; }

        var datosOffline = new System.Collections.Generic.Dictionary<string, object>
        {
            { "state", "offline" },
            { "last_changed", ServerValue.Timestamp }
        };

        miStatusRef.UpdateChildrenAsync(datosOffline).ContinueWithOnMainThread(task => onDone?.Invoke());
    }

    public void EscucharUsuariosEnLinea(Action<string, string, string> onCambio)
    {
        // onCambio: (uid, username, state)
        dbRoot.Child("status").ValueChanged += (sender, args) =>
        {
            if (args.DatabaseError != null)
            {
                Debug.LogWarning("Error escuchando /status: " + args.DatabaseError.Message);
                return;
            }

            if (args.Snapshot == null || !args.Snapshot.Exists) return;

            foreach (var hijo in args.Snapshot.Children)
            {
                string uid = hijo.Key;
                string username = hijo.Child("username").Value?.ToString() ?? "???";
                string state = hijo.Child("state").Value?.ToString() ?? "offline";
                onCambio?.Invoke(uid, username, state);
            }
        };
    }

    // Tu compañero usa este método para las notificaciones de conexión/desconexión de amigos.
    // No necesitás hacer nada extra, ya queda disponible para que él lo llame desde su script.
    public void EscucharEstadoDeUsuario(string uid, Action<string> onCambioEstado)
    {
        dbRoot.Child("status").Child(uid).Child("state").ValueChanged += (sender, args) =>
        {
            if (args.DatabaseError != null) return;
            string state = args.Snapshot.Value?.ToString() ?? "offline";
            onCambioEstado?.Invoke(state);
        };
    }
}