using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class BulletBehaviour : NetworkBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;

    [HideInInspector] public ulong shooterClientId;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Usamos linearVelocity (estándar en versiones recientes de Unity/PhysX)
            GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
            Invoke(nameof(DestroyBullet), lifeTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null) return;

        NetworkObject hitNetObj = other.GetComponent<NetworkObject>();
        if (hitNetObj != null && hitNetObj.OwnerClientId == shooterClientId) return;

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