using System;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FriendRequestsManager : MonoBehaviour
{
    public static FriendRequestsManager Instance { get; private set; }

    private DatabaseReference dbRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        dbRoot = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void EnviarSolicitud(string uidReceptor, string usernameReceptor,
        Action onSuccess, Action<string> onError)
    {
        string miUid = FirebaseManager.Instance.Auth.CurrentUser.UserId;

        FirebaseManager.Instance.ObtenerDatosUsuario(miUid,
            onSuccess: (misDatos) =>
            {
                var solicitud = new Dictionary<string, object>
                {
                    { "fromUsername", misDatos.username },
                    { "estado", "pendiente" },
                    { "timestamp", ServerValue.Timestamp }
                };

                dbRoot.Child("solicitudes").Child(uidReceptor).Child(miUid)
                    .SetValueAsync(solicitud).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled || task.IsFaulted)
                        {
                            onError?.Invoke("No se pudo enviar la solicitud.");
                            return;
                        }
                        onSuccess?.Invoke();
                    });
            },
            onError: onError);
    }

    public void EscucharSolicitudesRecibidas(Action<string, string> onNuevaSolicitud)
    {
        // onNuevaSolicitud: (uidEmisor, fromUsername)
        string miUid = FirebaseManager.Instance.Auth.CurrentUser.UserId;

        dbRoot.Child("solicitudes").Child(miUid).ValueChanged += (sender, args) =>
        {
            if (args.DatabaseError != null || args.Snapshot == null || !args.Snapshot.Exists) return;

            foreach (var hijo in args.Snapshot.Children)
            {
                string estado = hijo.Child("estado").Value?.ToString();
                if (estado == "pendiente")
                {
                    string uidEmisor = hijo.Key;
                    string fromUsername = hijo.Child("fromUsername").Value?.ToString() ?? "???";
                    onNuevaSolicitud?.Invoke(uidEmisor, fromUsername);
                }
            }
        };
    }

    public void ResponderSolicitud(string uidEmisor, bool aceptar,
        Action onSuccess, Action<string> onError)
    {
        string miUid = FirebaseManager.Instance.Auth.CurrentUser.UserId;

        if (!aceptar)
        {
            dbRoot.Child("solicitudes").Child(miUid).Child(uidEmisor).RemoveValueAsync()
                .ContinueWithOnMainThread(task => onSuccess?.Invoke());
            return;
        }

        // Al aceptar: escribimos en /amigos en ambos sentidos (acá es donde tu compañero
        // "engancha" su parte, leyendo este mismo nodo) y limpiamos la solicitud.
        var actualizaciones = new Dictionary<string, object>
        {
            { $"amigos/{miUid}/{uidEmisor}", true },
            { $"amigos/{uidEmisor}/{miUid}", true },
            { $"solicitudes/{miUid}/{uidEmisor}", null } // null borra ese nodo
        };

        dbRoot.UpdateChildrenAsync(actualizaciones).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                onError?.Invoke("No se pudo aceptar la solicitud.");
                return;
            }
            onSuccess?.Invoke();
        });
    }
}