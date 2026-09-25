using System;
using System.Collections.Generic;
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

    // ---------- REGISTRO ----------
    public void Registrar(string email, string password, string username,
    Action<UsuarioFirestore> onSuccess, Action<string> onError)
    {
    Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
    {
        if (task.IsCanceled || task.IsFaulted)
        {
            onError?.Invoke(InterpretarError(task.Exception));
            return;
        }

        FirebaseUser user = task.Result.User; // si tu SDK da error acá, probá "task.Result" sin ".User"
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
                onError?.Invoke("Cuenta creada, pero falló al guardar los datos del usuario.");
                return;
            }
            onSuccess?.Invoke(datos);
        });
    });
    }

    // ---------- LOGIN ----------
    public void Login(string email, string password,
        Action<UsuarioFirestore> onSuccess, Action<string> onError)
    {
        Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                onError?.Invoke(InterpretarError(task.Exception));
                return;
            }

            string uid = Auth.CurrentUser.UserId;
            ObtenerDatosUsuario(uid, onSuccess, onError);
        });
    }

    // ---------- OBTENER DATOS DE UN USUARIO ----------
    public void ObtenerDatosUsuario(string uid,
        Action<UsuarioFirestore> onSuccess, Action<string> onError)
    {
        Db.Collection("usuarios").Document(uid).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted || !task.Result.Exists)
            {
                onError?.Invoke("No se encontraron datos del usuario.");
                return;
            }
            var datos = task.Result.ConvertTo<UsuarioFirestore>();
            onSuccess?.Invoke(datos);
        });
    }

    // ---------- RECUPERAR CONTRASEÑA ----------
    public void RecuperarPassword(string email, Action onSuccess, Action<string> onError)
    {
        Auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                onError?.Invoke(InterpretarError(task.Exception));
                return;
            }
            onSuccess?.Invoke();
        });
    }

    // ---------- ACTUALIZAR SCORE ----------
    public void ActualizarScore(int nuevoScore, Action onSuccess, Action<string> onError)
    {
        string uid = Auth.CurrentUser.UserId;
        Db.Collection("usuarios").Document(uid).UpdateAsync("score", (long)nuevoScore)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    onError?.Invoke("No se pudo guardar el puntaje.");
                    return;
                }
                onSuccess?.Invoke();
            });
    }

    // ---------- LEADERBOARD ----------
    public void ObtenerLeaderboard(int limite,
        Action<List<UsuarioFirestore>> onSuccess, Action<string> onError)
    {
        Db.Collection("usuarios")
          .OrderByDescending("score")
          .Limit(limite)
          .GetSnapshotAsync()
          .ContinueWithOnMainThread(task =>
          {
              if (task.IsCanceled || task.IsFaulted)
              {
                  onError?.Invoke("No se pudo cargar el ranking.");
                  return;
              }

              var lista = new List<UsuarioFirestore>();
              foreach (DocumentSnapshot doc in task.Result.Documents)
              {
                  lista.Add(doc.ConvertTo<UsuarioFirestore>());
              }
              onSuccess?.Invoke(lista);
          });
    }

    // ---------- ERRORES ----------
    private string InterpretarError(AggregateException exception)
    {
        if (exception == null) return "Error desconocido.";

        var firebaseEx = exception.GetBaseException() as FirebaseException;
        if (firebaseEx == null) return exception.GetBaseException().Message;

        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
        switch (errorCode)
        {
            case AuthError.InvalidEmail: return "El correo no es válido.";
            case AuthError.WrongPassword: return "Contraseña incorrecta.";
            case AuthError.UserNotFound: return "No existe una cuenta con ese correo.";
            case AuthError.EmailAlreadyInUse: return "Ese correo ya está registrado.";
            case AuthError.WeakPassword: return "La contraseña es demasiado débil (mínimo 6 caracteres).";
            default: return $"Error de autenticación: {errorCode}";
        }
    }
}