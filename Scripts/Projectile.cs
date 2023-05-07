using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifeTime = 3f;
    public int damage = 10;
    public ProjectileType type;
    private float timeCounter = 0f;
    private Vector3 launchVector;
    private float rotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    // maybe make this FixedUpdate?
    void Update()
    {
        timeCounter += Time.deltaTime / lifeTime;
        transform.Rotate(0, 0, -rotation*Time.deltaTime);
    }

    //projectilePrefab, transform.position, transform.rotation
    public void Initialize(Vector3 projectileLaunchVector, int projectileDamage, float projectileRotation, ProjectileType projectileType)
    {
        launchVector = projectileLaunchVector;
        damage = projectileDamage;
        rotation = projectileRotation;
        type = projectileType;

        transform.GetComponent<Rigidbody>().AddRelativeForce(launchVector);
    }

    // very basic damage -- need a wrapped PlayerHealth collider
    // will take damage several times, delete the object on impact
    
    private void OnCollisionEnter(Collision collision) {
        FPSController player = collision.gameObject.GetComponent<FPSController>();
        if (player != null) {
            player.damage(damage);
            Destroy(this.gameObject);
        }
    }

}
