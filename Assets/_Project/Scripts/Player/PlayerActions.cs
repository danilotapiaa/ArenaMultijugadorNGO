using System.Collections;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class PlayerActions : NetworkBehaviour
{
    [Header("Configuración de Salto")]
    public float jumpForce = 5f;

    [Header("Configuración de Disparo")]
    public GameObject bulletPrefab; // Referencia al proyectil
    public Transform firePoint;     // Punto exacto desde donde sale la bala

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!IsOwner) return;

        // Acción de Salto
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestJumpRpc();
        }

        // Acción de Disparo (Clic izquierdo del ratón)
        if (Input.GetMouseButtonDown(0))
        {
            RequestShootRpc();
        }
    }

    // --- LÓGICA DE SALTO ---
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void RequestJumpRpc()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        PlayJumpEffectRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void PlayJumpEffectRpc()
    {
        StartCoroutine(JumpVisualEffect());
    }

    private IEnumerator JumpVisualEffect()
    {
        Vector3 originalScale = transform.localScale;
        transform.localScale = new Vector3(originalScale.x * 1.3f, originalScale.y * 0.7f, originalScale.z * 1.3f);
        yield return new WaitForSeconds(0.1f);
        transform.localScale = new Vector3(originalScale.x * 0.8f, originalScale.y * 1.2f, originalScale.z * 0.8f);
        yield return new WaitForSeconds(0.15f);
        transform.localScale = originalScale;
    }

    // --- LÓGICA DE DISPARO ---
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void RequestShootRpc()
    {
        // 1. El servidor crea la instancia en su propia memoria
        GameObject bulletInstance = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 2. El servidor ordena a la red que distribuya este objeto a todos los clientes
        NetworkObject netObj = bulletInstance.GetComponent<NetworkObject>();
        netObj.Spawn();
    }
}