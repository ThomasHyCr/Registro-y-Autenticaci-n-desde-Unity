using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    public FirebaseAuth Auth { get; private set; }
    public FirebaseFirestore Db { get; private set; }
    public bool Listo { get; private set; } = false;

    public event Action OnFirebaseListo;
    public event Action<string> OnFirebaseError;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InicializarFirebase();
    }

    private void InicializarFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                Auth = FirebaseAuth.DefaultInstance;
                Db = FirebaseFirestore.DefaultInstance;
                Listo = true;
                Debug.Log("Firebase inicializado correctamente.");
                OnFirebaseListo?.Invoke();
            }
            else
            {
                string msg = $"No se pudieron resolver las dependencias de Firebase: {status}";
                Debug.LogError(msg);
                OnFirebaseError?.Invoke(msg);
            }
        });
    }

public void Registrar(string email, string password, string username, string datoAdicional,
    Action<UsuarioFirestore> onSuccess, Action<string> onError)
{
    Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
    {
        if (task.IsCanceled || task.IsFaulted)
        {
            onError?.Invoke(InterpretarError(task.Exception));
            return;
        }

        FirebaseUser user = task.Result.User; // en SDKs recientes task.Result es AuthResult
        string uid = user.UserId;

        var datos = new UsuarioFirestore
        {
            username = username,
            email = email,
            score = 0
        };

        Db.Collection("usuarios").Document(uid).SetAsync(datos).ContinueWithOnMainThread(setTask =>
        {
            if (setTask.IsCanceled || setTask.IsFaulted)
            {
                onError?.Invoke("Cuenta creada, pero falló al guardar los datos adicionales.");
                return;
            }
            onSuccess?.Invoke(datos);
        });
    });
}
}

