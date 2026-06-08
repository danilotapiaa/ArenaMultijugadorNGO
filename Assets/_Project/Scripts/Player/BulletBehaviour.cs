using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class BulletBehaviour : NetworkBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;

    // Se asigna desde PlayerActions antes del Spawn()
    [HideInInspector] public ulong shooterClientId;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
            Invoke(nameof(DestroyBullet), lifeTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Solo el servidor procesa el daño
        if (!IsServer) return;

        // Intentar obtener la vida del jugador golpeado
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        // No dañar al jugador que disparó
        NetworkObject hitNetObj = other.GetComponent<NetworkObject>();
        if (hitNetObj != null && hitNetObj.OwnerClientId == shooterClientId) return;

        // Aplicar daño y destruir bala
        health.ReceiveDamage();
        DestroyBullet();
    }

    private void DestroyBullet()
    {
        if (NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn(true);
        }
    }
}