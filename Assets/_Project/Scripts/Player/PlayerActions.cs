using System.Collections;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class PlayerActions : NetworkBehaviour
{
    [Header("Configuración de Salto")]
    public float jumpForce = 5f;

    [Header("Configuración de Disparo")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestJumpRpc();
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Enviamos posición y rotación del firePoint local al servidor
            if (firePoint != null)
            {
                RequestShootRpc(firePoint.position, firePoint.rotation, OwnerClientId);
            }
        }
    }

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

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void RequestShootRpc(Vector3 spawnPosition, Quaternion spawnRotation, ulong shooterClientId)
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, spawnPosition, spawnRotation);

        BulletBehaviour bullet = bulletInstance.GetComponent<BulletBehaviour>();
        if (bullet != null)
        {
            bullet.shooterClientId = shooterClientId;
        }

        NetworkObject netObj = bulletInstance.GetComponent<NetworkObject>();
        netObj.Spawn();
    }
}