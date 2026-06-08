using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkUIManager : MonoBehaviour
{
    [Header("Referencias de Interfaz")]
    public Button hostButton;
    public Button clientButton;

    void Awake()
    {
        // Vinculamos mediante código la acción de clic al componente de red global
        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            OcultarInterfaz();
        });

        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            OcultarInterfaz();
        });
    }

    private void OcultarInterfaz()
    {
        // Desactiva el Canvas completo para limpiar la pantalla durante el juego
        gameObject.SetActive(false);
    }
}