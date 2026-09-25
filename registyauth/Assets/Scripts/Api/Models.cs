using System;
using System.Collections.Generic;
using Firebase.Firestore;
using UnityEngine;

// ===================== Modelo para Firebase (Firestore) =====================

[FirestoreData]
public class UsuarioFirestore
{
    [FirestoreProperty] public string username { get; set; }
    [FirestoreProperty] public string email { get; set; }
    [FirestoreProperty] public long score { get; set; }
}

// ===================== Modelos viejos de la API REST =====================
// Se pueden borrar una vez que confirmes que toda la app funciona con Firebase
// y elimines también ApiManager.cs.

[Serializable]
public class CredencialesRequest
{
    public string username;
    public string password;
}

[Serializable]
public class UsuarioData
{
    public string uid;
    public string username;
    public string state;
    public Dictionary<string, object> data;
}

[Serializable]
public class LoginResponse
{
    public UsuarioData usuario;
    public string token;
}

[Serializable]
public class UsuarioResponse
{
    public UsuarioData usuario;
}

[Serializable]
public class UsuariosListResponse
{
    public List<UsuarioData> usuarios;
}

[Serializable]
public class ActualizarDataRequest
{
    public string username;
    public Dictionary<string, object> data;
}

[Serializable]
public class ErrorResponse
{
    public string Msg;
}