using UnityEngine;
using Unity.Netcode;

public class PlayerColorSync : NetworkBehaviour
{
    [Header("Estado de Red")]
    // Definimos una variable de red para el color. Lectura pública, escritura solo servidor.
    public NetworkVariable<Color> playerColor = new NetworkVariable<Color>(
        Color.white,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private MeshRenderer meshRenderer;

    void Awake()
    {
        // Obtenemos la referencia al renderizador de la cápsula
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Este método reemplaza al Start() tradicional en NGO
    public override void OnNetworkSpawn()
    {
        // 1. Nos suscribimos al evento para escuchar futuros cambios de color en la red
        playerColor.OnValueChanged += OnColorChanged;

        // 2. Aplicamos el color actual al nacer (crucial para clientes Late Join)
        ApplyColor(playerColor.Value);

        // 3. El servidor decide y asigna el color inicial
        if (IsServer)
        {
            // Genera un color aleatorio llamativo
            playerColor.Value = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }
    }

    // Limpieza de memoria requerida en sistemas de eventos
    public override void OnNetworkDespawn()
    {
        playerColor.OnValueChanged -= OnColorChanged;
    }

    // Método disparado automáticamente cuando el valor de la red se actualiza
    private void OnColorChanged(Color previousValue, Color newValue)
    {
        ApplyColor(newValue);
    }

    // Lógica local para cambiar el material
    private void ApplyColor(Color c)
    {
        if (meshRenderer != null)
        {
            // Modificamos el color base del material de URP
            meshRenderer.material.color = c;
        }
    }
}