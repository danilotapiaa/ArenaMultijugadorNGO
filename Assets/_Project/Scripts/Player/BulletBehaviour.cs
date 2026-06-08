using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody))]
public class BulletBehaviour : NetworkBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;

    public override void OnNetworkSpawn()
    {
        // Solo el servidor tiene la autoridad para mover y destruir la entidad crítica
        if (IsServer)
        {
            // Asignar velocidad constante hacia adelante
            GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;

            // Programar la destrucción del objeto para liberar memoria
            Invoke(nameof(DestroyBullet), lifeTime);
        }
    }

    private void DestroyBullet()
    {
        // Verificar que el objeto sigue activo en la red antes de destruirlo
        if (NetworkObject.IsSpawned)
        {
            // Despawn remueve el objeto de la red y, por defecto, destruye el GameObject
            NetworkObject.Despawn(true);
        }
    }
}