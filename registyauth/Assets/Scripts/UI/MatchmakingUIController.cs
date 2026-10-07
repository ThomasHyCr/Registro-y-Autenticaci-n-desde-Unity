using TMPro;
using UnityEngine;

public class MatchmakingUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text textEstado;
    [SerializeField] private GameObject botonBuscar;
    [SerializeField] private GameObject botonCancelar;
    [SerializeField] private GameObject panelMatchmaking;
    [SerializeField] private GameObject panelPerfil;

    void OnEnable()
    {
        Reiniciar();
    }

    private void Reiniciar()
    {
        if (textEstado != null) textEstado.text = "Presiona \"Buscar partida\"";
        if (botonBuscar != null) botonBuscar.SetActive(true);
        if (botonCancelar != null) botonCancelar.SetActive(false);
    }

    public void OnClickBuscar()
    {
        textEstado.text = "Conectando...";
        botonBuscar.SetActive(false);
        botonCancelar.SetActive(true);

        MatchmakingManager.Instance.BuscarPartida(
            alEmparejarCallback: (partidaId, rivalUid, rivalUsername) =>
            {
                if (this == null) return;
                string idCorto = partidaId.Length > 6 ? partidaId.Substring(0, 6) : partidaId;
                textEstado.text = $"¡Emparejado con {rivalUsername}!\nPartida: {idCorto}";
                botonCancelar.SetActive(false);
                botonBuscar.SetActive(true); // permite buscar otra partida
            },
            alEsperarCallback: () =>
            {
                if (this == null) return;
                textEstado.text = "Buscando rival...";
            },
            alErrorCallback: (err) =>
            {
                if (this == null) return;
                textEstado.text = err;
                Reiniciar();
            });
    }

    public void OnClickCancelar()
    {
        MatchmakingManager.Instance.CancelarBusqueda();
        Reiniciar();
    }

    public void OnClickVolver()
    {
        MatchmakingManager.Instance.CancelarBusqueda();
        if (panelMatchmaking != null) panelMatchmaking.SetActive(false);
        if (panelPerfil != null) panelPerfil.SetActive(true);
    }
}