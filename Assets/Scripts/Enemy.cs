using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float shootInterval = 3.5f;
    public GameObject destination;
    private NavMeshAgent agent;
    public Bullet bulletPrefab;
    private PauseMenu pauseMenu;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip destroySound;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(shoot(shootInterval));
        pauseMenu = FindObjectOfType<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if(destination != null){
            agent.destination = destination.transform.position;
        }
    }

    IEnumerator shoot(float duration){
        if(destination!=null){
            yield return new WaitForSeconds(duration);
            shootBullet(destination.transform.position);
            yield return StartCoroutine(shoot(duration));
        }
    }

    void shootBullet(Vector3 finalDestination){
        Transform gunChild = FindDeepChild(transform, "GunBarrelBullet");
        if (gunChild == null) {
            Debug.LogError("Gun barrel not found!");
            return;
        }
        Vector3 bulletPosition = new Vector3(gunChild.transform.position.x, transform.position.y, gunChild.transform.position.z);
        Bullet tempBullet = Instantiate(bulletPrefab, bulletPosition, gunChild.transform.rotation);
        tempBullet.setDestination(finalDestination);
        tempBullet.setActiveBullet(true);

        // Play the shooting sound effect
        if (audioSource != null && shootSound != null)
        {
            audioSource.clip = shootSound;
            audioSource.Play();
        }
    }

    void OnCollisionEnter(Collision collision){
        if(collision.gameObject.tag == "Bullet"){
             // Play the shooting sound effect
            if (audioSource != null && shootSound != null)
            {
                audioSource.clip = destroySound;
                audioSource.Play();
            }
            Destroy(gameObject);
            pauseMenu.GameWon();
        }
    }

    Transform FindDeepChild(Transform parent, string childName) {
        foreach (Transform child in parent) {
            if (child.name == childName) {
                return child;
            }
            Transform result = FindDeepChild(child, childName);
            if (result != null) {
                return result;
            }
        }
        return null;
    }
}
