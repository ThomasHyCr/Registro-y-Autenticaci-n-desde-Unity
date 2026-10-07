using System;
using Firebase.Database;

public static class FriendsListManager
{
    private static DatabaseReference Root => FirebaseDatabase.DefaultInstance.RootReference;

    // Escucha los amigos confirmados del usuario actual.
    // Dispara onAmigoAgregado(uidAmigo) una vez por cada amigo existente y por cada amigo nuevo.
    // Devuelve una función: llamala para dejar de escuchar.
    public static Action EscucharListaAmigos(Action<string> onAmigoAgregado)
    {
        string miUid = FirebaseManager.Instance.Auth.CurrentUser.UserId;
        DatabaseReference refAmigos = Root.Child("amigos").Child(miUid);

        EventHandler<ChildChangedEventArgs> handler = (sender, args) =>
        {
            if (args.DatabaseError != null) return;
            onAmigoAgregado?.Invoke(args.Snapshot.Key);
        };

        refAmigos.ChildAdded += handler;
        return () => refAmigos.ChildAdded -= handler;
    }

    // Escucha el estado ("online"/"offline") de un usuario puntual.
    // El PRIMER evento trae el estado actual; los siguientes son cambios reales.
    // Devuelve una función para dejar de escuchar.
    public static Action EscucharEstado(string uid, Action<string> onEstado)
    {
        DatabaseReference refEstado = Root.Child("status").Child(uid).Child("state");

        EventHandler<ValueChangedEventArgs> handler = (sender, args) =>
        {
            if (args.DatabaseError != null) return;

            string estado = (args.Snapshot != null && args.Snapshot.Exists && args.Snapshot.Value != null)
                ? args.Snapshot.Value.ToString()
                : "offline";

            onEstado?.Invoke(estado);
        };

        refEstado.ValueChanged += handler;
        return () => refEstado.ValueChanged -= handler;
    }
}