using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;

    [Header("Panel Game Over")]
    public GameObject gameOverPanel;
    public Button botonReintentar;
    public Button botonSalir;

    [Header("Indicador de Vida (opcional)")]
    public TextMeshProUGUI textoVida; // Ej: "Vida: 2/3"

    void Awake()
    {
        // Singleton para acceso desde PlayerHealth
        Instance = this;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Start()
    {
        if (botonReintentar != null)
            botonReintentar.onClick.AddListener(Reintentar);

        if (botonSalir != null)
            botonSalir.onClick.AddListener(SalirAlMenu);
    }

    // Llamado cuando el jugador local muere
    public void MostrarGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Desbloquear cursor para poder hacer clic en los botones
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Actualiza el texto de vida en pantalla
    public void ActualizarVida(int actual, int maximo)
    {
        if (textoVida != null)
            textoVida.text = $"Vida: {actual}/{maximo}";
    }

    private void Reintentar()
    {
        // Apagar la red y recargar la escena actual
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.Shutdown();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void SalirAlMenu()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.Shutdown();

        // Asegúrate de que tu escena del menú se llame "MainMenu"
        SceneManager.LoadScene("MainMenu");
    }
}