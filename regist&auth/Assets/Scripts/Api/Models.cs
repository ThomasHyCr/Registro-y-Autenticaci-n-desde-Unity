using System;
using System.Collections.Generic;
using UnityEngine;

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
    public Dictionary<string, object> data; // requiere Newtonsoft para deserializar bien
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