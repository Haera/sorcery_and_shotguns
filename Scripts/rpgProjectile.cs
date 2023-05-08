using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rpgProjectile : MonoBehaviour
{

    public float yVelocity;
    public float speed;
    public float gravity;
    public GameObject explosion;
    public float explosionSize;

    // Start is called before the first frame update
    void Start()
    {
        //lifetime?
    }

    // Update is called once per frame
    void Update()
    {
        yVelocity += gravity * Time.deltaTime;
        Vector3 vect = Vector3.forward * speed;
        Vector3 temp = this.transform.InverseTransformVector(Vector3.down * yVelocity);
        vect -= temp; 
        this.transform.Translate(vect * Time.deltaTime, Space.Self);

    }

    private void OnTriggerEnter(Collider other) {
        
        //this may look wierd but explode is called elsewhere
        explode();

    }

    public void explode() {
        explosion = Instantiate(explosion, transform.position, Quaternion.identity);
        explosion.transform.localScale = new Vector3(explosionSize, explosionSize, explosionSize);
        Destroy(this.gameObject);
    }

}
