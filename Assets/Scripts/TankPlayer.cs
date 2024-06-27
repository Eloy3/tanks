using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPlayer : MonoBehaviour
{
    private float moveSpeed = 4f;
    private float rotationSpeed = 10f;
    private float rotationTankSpeed = 3f;
    [SerializeField]
    private Bullet bulletPrefab;
    public GameObject turret;

    private PauseMenu pauseMenu;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip destroySound;

    // Cooldown variables
    public float shootCooldown = 2f; // Cooldown duration in seconds
    private float shootTimer;

    void Start()
    {
        pauseMenu = FindObjectOfType<PauseMenu>();
        shootTimer = shootCooldown; // Initialize the timer to the cooldown duration so the player can shoot immediately at the start
    }

    void Update()
    {
        // Check if the game is paused
        if (pauseMenu != null && pauseMenu.IsPaused())
        {
            return;
        }

        // Update the shoot timer
        shootTimer += Time.deltaTime;

        // Obtener la entrada del usuario
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Crear la dirección de movimiento
        Vector3 movementDirection;
        if (Mathf.Abs(verticalInput) > Mathf.Abs(horizontalInput))
        {
            movementDirection = new Vector3(verticalInput, 0f, 0f).normalized;
        }
        else
        {
            movementDirection = new Vector3(0f, 0f, -horizontalInput).normalized;
        }

        // Mover el tanque
        transform.Translate(movementDirection * moveSpeed * Time.deltaTime, Space.World);

        // Rotar el tanque si hay alguna entrada de movimiento
        if (movementDirection != Vector3.zero)
        {
            // Calcular el ángulo entre la dirección actual del tanque y la dirección de movimiento
            Vector3 currentForward = transform.forward;
            float angle = Vector3.Angle(currentForward, movementDirection);

            // Calcular el producto cruzado para determinar el lado del ángulo
            Vector3 cross = Vector3.Cross(currentForward, movementDirection);

            // Si el producto cruzado tiene un componente Y negativo, el ángulo es mayor de 180 grados
            if (cross.y < 0)
            {
                angle = 360f - angle;
                movementDirection = - movementDirection;
            }

            // Comprobar si el ángulo es mayor de 180 grados
            if (angle != 180f && angle != 0)
            {
                Quaternion rotation = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            }
        }

        // Obtener la posición del ratón
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.transform.position.y; // Asegurar que el ratón tenga la misma altura que la cámara
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);

        // Convertir la posición del ratón en un vector
        Vector3 direction = worldMousePos - turret.transform.position;
        direction.y = 0; // Ignorar el componente Y para rotar solo en el eje Y

        // Rotar la torreta para que mire en la dirección del ratón
        if (direction != Vector3.zero)
        {
            Quaternion rotationTurret = Quaternion.LookRotation(direction);
            // Mantener la rotación solo en el eje Y
            Vector3 eulerRotation = rotationTurret.eulerAngles;
            eulerRotation.x = turret.transform.rotation.eulerAngles.x;
            eulerRotation.y = rotationTurret.eulerAngles.y;
            eulerRotation.z = turret.transform.rotation.eulerAngles.z;
            rotationTurret = Quaternion.Euler(eulerRotation);

            // Aplicar la rotación suavizada
            turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, rotationTurret, rotationTankSpeed * Time.deltaTime);
        }

        // Check for shooting input and cooldown
        if (Input.GetMouseButtonDown(0) && shootTimer >= shootCooldown)
        {
            Vector3 bulletFuturePos = Input.mousePosition;
            bulletFuturePos.z = Camera.main.transform.position.y;
            Vector3 finalDestination = Camera.main.ScreenToWorldPoint(bulletFuturePos);
            shootBullet(finalDestination);
            shootTimer = 0f; // Reset the shoot timer
        }
    }

    void shootBullet(Vector3 finalDestination)
    {
        Transform gunChild = FindDeepChild(transform, "GunBarrelBullet");
        if (gunChild == null)
        {
            Debug.LogError("Gun barrel not found!");
            return;
        }
        Vector3 bulletPosition = new Vector3(gunChild.transform.position.x, transform.position.y, gunChild.transform.position.z);
        Bullet tempBullet = Instantiate(bulletPrefab, bulletPosition, gunChild.transform.rotation);
        tempBullet.setDestination(finalDestination);
        tempBullet.setActiveBullet(true);

        if (audioSource != null && shootSound != null)
        {
            audioSource.clip = shootSound;
            audioSource.Play();
        }
    }

    Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            Transform result = FindDeepChild(child, childName);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            if (audioSource != null && shootSound != null)
            {
                audioSource.clip = destroySound;
                audioSource.Play();
            }
            Destroy(gameObject);
            pauseMenu.GameOver();
        }
    }
}
