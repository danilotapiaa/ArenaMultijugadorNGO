# Arena Multijugador NGO

Prototipo de arena de combate multijugador en 3D hecho en Unity, usando **Netcode for GameObjects (NGO)** para la sincronización en red. Cada jugador controla una cápsula que se mueve por la arena, puede saltar y disparar balas que quitan vida a los rivales hasta eliminarlos.

El flujo es simple: desde el menú principal se elige alojar una partida (Host) o unirse como cliente (Client); una vez en la escena `MainArena`, cada jugador se mueve y dispara, la vida se sincroniza vía `NetworkVariable`, y al llegar a 0 vidas se muestra un panel de "Game Over" con opción de reintentar o volver al menú.

## Controles

- **W / A / S / D** o flechas: mover y girar el personaje
- **Espacio**: saltar
- **Clic izquierdo**: disparar

## Stack técnico

- **Unity 6000.4.9f1** (Unity 6)
- **Netcode for GameObjects** `2.12.0` (`com.unity.netcode.gameobjects`)
- `com.unity.multiplayer.tools` y `com.unity.multiplayer.center` como apoyo de desarrollo/depuración de red
- Render Pipeline: URP
- UI: uGUI + TextMesh Pro
- Nuevo Input System (`InputSystem_Actions.inputactions`), aunque el movimiento actual todavía usa la API clásica de `Input`

## Estructura relevante de `Assets/`

- `Assets/_Project/Scripts/Core/` — gestión de UI de red (botones para iniciar Host/Client)
- `Assets/_Project/Scripts/Player/` — movimiento, salto, disparo, vida, sincronización de color y UI de Game Over del jugador
- `Assets/_Project/Prefabs/` — `Player.prefab`, `Bullet.prefab`, `FirePoint.prefab`
- `Assets/_Project/Scenes/` — `MainMenu.unity` (elección Host/Client) y `MainArena.unity` (partida)
- `Assets/_Project/Materials/` — materiales de la arena y jugadores
- `Assets/DefaultNetworkPrefabs.asset` — lista de prefabs de red registrados en NGO

## Estado actual

Es un prototipo funcional pero experimental: el movimiento no usa reconciliación de cliente/servidor ni interpolación (solo mueve el `transform` local del owner), no hay validación anti-cheat en el servidor, la arena es mínima y no hay sistema de respawn tras la muerte (el juego simplemente termina para ese jugador). Sirve como base de aprendizaje de NGO más que como un juego terminado.
