using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 15f;
    private Vector3 destination;
    private Rigidbody rb;
    public int allowedCollisions = 3;
    private int currentCollisions = 0;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = (destination - transform.position).normalized* moveSpeed;
        Invoke("disableTriggerCollider", 0.05f);
    }

    public void setActiveBullet(bool active){
        gameObject.SetActive(active);
    }

    public void setDestination(Vector3 inputDestination){
        destination = inputDestination;
    }

    public void disableTriggerCollider(){
        transform.GetComponent<Collider>().isTrigger = false;
    }
    public Vector3 getDestination(){
        return destination;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

void OnCollisionEnter(Collision collision){

        if(collision.gameObject.name.Contains("Wall")){
            ContactPoint contact = collision.contacts[0];
            Vector3 direction = Vector3.Project(rb.velocity, contact.normal);
            GetComponent<Rigidbody>().AddForce(direction * moveSpeed);
            currentCollisions++;
            if(currentCollisions >= allowedCollisions){
                Destroy(gameObject);
            }
        }

        if(collision.gameObject.name == "Enemy" || collision.gameObject.name == "tankUnity"){
            Destroy(gameObject);
        } 
    } 
}
