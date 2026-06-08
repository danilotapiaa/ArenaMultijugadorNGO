using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Configuración de Vida")]
    public int maxHealth = 3;

    // Variable de red: se sincroniza a todos los clientes automáticamente
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth.Value = maxHealth;
        }

        // Escuchar cambios de vida para actualizar UI
        currentHealth.OnValueChanged += OnHealthChanged;
    }

    public override void OnNetworkDespawn()
    {
        currentHealth.OnValueChanged -= OnHealthChanged;
    }

    // Llamado desde BulletBehaviour en el servidor
    public void ReceiveDamage()
    {
        if (!IsServer) return;
        if (currentHealth.Value <= 0) return;

        currentHealth.Value--;

        if (currentHealth.Value <= 0)
        {
            // Notificar al dueño que perdió
            NotifyGameOverClientRpc();
        }
    }

    [ClientRpc]
    private void NotifyGameOverClientRpc()
    {
        // Solo el jugador dueño de este objeto ve su Game Over
        if (!IsOwner) return;

        if (GameOverUI.Instance != null)
        {
            GameOverUI.Instance.MostrarGameOver();
        }
    }

    private void OnHealthChanged(int anterior, int nuevo)
    {
        // Solo el dueño local actualiza su propia UI de vida
        if (!IsOwner) return;

        if (GameOverUI.Instance != null)
        {
            GameOverUI.Instance.ActualizarVida(nuevo, maxHealth);
        }

        Debug.Log($"[PlayerHealth] Vida: {nuevo}/{maxHealth}");
    }

    public int GetCurrentHealth() => currentHealth.Value;
    public int GetMaxHealth() => maxHealth;
}