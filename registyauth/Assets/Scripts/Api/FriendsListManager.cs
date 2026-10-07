using System;
using Firebase.Database;
using UnityEngine;

public class FriendsListManager : MonoBehaviour
{
    public static FriendsListManager Instance { get; private set; }

    private DatabaseReference dbRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        dbRoot = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void EscucharListaAmigos(Action<string> onAmigoAgregado)
    {
        string miUid = FirebaseManager.Instance.Auth.CurrentUser.UserId;

        dbRoot.Child("amigos").Child(miUid).ChildAdded += (sender, args) =>
        {
            if (args.DatabaseError != null) return;
            onAmigoAgregado?.Invoke(args.Snapshot.Key); // uid del nuevo amigo
        };
    }
}