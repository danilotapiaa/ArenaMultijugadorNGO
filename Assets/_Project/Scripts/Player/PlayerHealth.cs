using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Configuración de Vida")]
    public int maxHealth = 3;
    public GameObject gameOverPanel;

    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        if (IsServer) currentHealth.Value = maxHealth;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        currentHealth.OnValueChanged += OnHealthChanged;
    }

    public void ReceiveDamage()
    {
        if (!IsServer || currentHealth.Value <= 0) return;

        currentHealth.Value--;

        if (currentHealth.Value <= 0)
        {
            NotifyGameOverClientRpc();
        }
    }

    [Rpc(SendTo.Owner)]
    private void NotifyGameOverClientRpc()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0; // Pausa el juego
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnHealthChanged(int anterior, int nuevo)
    {
        // Lógica de actualización de UI de vida aquí si la necesitas
    }
}