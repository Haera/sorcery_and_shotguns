using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTravel : MonoBehaviour
{
    public float speed;
    public float lifetime;
    public int damage;
    
    void Update()
    {
        this.transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
        Destroy(this.gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<HitDetector>() != null) {

            other.GetComponent<HitDetector>().damage(damage);
            Destroy(this.gameObject);

        }
    }
}
