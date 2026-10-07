using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FriendsListUIController : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject filaPrefab;
    [SerializeField] private GameObject panelAmigos;
    [SerializeField] private GameObject panelPerfil;

    private readonly Dictionary<string, GameObject> filasPorUid = new Dictionary<string, GameObject>();
    private readonly List<Action> limpiezas = new List<Action>();
    private Action detenerLista;
    private bool activo = false;

    void OnEnable()
    {
        activo = true;
        LimpiarTodo();
        detenerLista = FriendsListManager.EscucharListaAmigos(OnAmigoNuevo);
    }

    void OnDisable()
    {
        activo = false;
        LimpiarTodo();
    }

    private void OnAmigoNuevo(string uidAmigo)
    {
        if (!activo || filasPorUid.ContainsKey(uidAmigo)) return;
        filasPorUid[uidAmigo] = null; // reservamos el lugar mientras llega el username

        FirebaseManager.Instance.ObtenerDatosUsuario(uidAmigo,
            onSuccess: (datos) =>
            {
                if (!activo) return; // el panel se cerró mientras esperábamos

                GameObject fila = Instantiate(filaPrefab, contentParent);
                TMP_Text[] textos = fila.GetComponentsInChildren<TMP_Text>();

                if (textos.Length > 0) textos[0].text = datos.username;
                filasPorUid[uidAmigo] = fila;

                if (textos.Length > 1)
                {
                    TMP_Text textoEstado = textos[1];
                    limpiezas.Add(FriendsListManager.EscucharEstado(uidAmigo, (estado) =>
                    {
                        if (textoEstado == null) return;
                        textoEstado.text = estado == "online" ? "En línea" : "Desconectado";
                    }));
                }
            },
            onError: (err) =>
            {
                filasPorUid.Remove(uidAmigo);
                Debug.LogWarning("No se pudo cargar el amigo: " + err);
            });
    }

    private void LimpiarTodo()
    {
        detenerLista?.Invoke();
        detenerLista = null;

        foreach (Action limpiar in limpiezas) limpiar?.Invoke();
        limpiezas.Clear();

        filasPorUid.Clear();

        if (contentParent != null)
        {
            foreach (Transform hijo in contentParent) Destroy(hijo.gameObject);
        }
    }

    public void OnClickVolver()
    {
        if (panelAmigos != null) panelAmigos.SetActive(false);
        if (panelPerfil != null) panelPerfil.SetActive(true);
    }
}