using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Configuración de Movimiento")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 120f;

    void Update()
    {
        // Validación de red crítica:
        // Si la máquina local no es dueña de este objeto, ignorar el código.
        if (!IsOwner) return;

        // Leer entradas del teclado clásico de Unity (WASD o Flechas)
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        // Cálculo de traslación (Avance/Retroceso)
        Vector3 movement = transform.forward * moveInput * moveSpeed * Time.deltaTime;
        transform.position += movement;

        // Cálculo de rotación (Giro sobre el eje Y)
        float turn = turnInput * rotationSpeed * Time.deltaTime;
        transform.rotation *= Quaternion.Euler(0, turn, 0);
    }
}