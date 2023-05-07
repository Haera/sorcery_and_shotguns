using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    public int damage;
    public bool affectsPlayer;

    private void OnTriggerEnter(Collider other) {
        
        if (other.GetComponent<FPSController>() != null && affectsPlayer) {

            other.GetComponent<FPSController>().damage(damage);
            Destroy(this.gameObject);

        }

        if (other.GetComponent<HitDetector>() != null) {
            other.GetComponent<HitDetector>().damage(damage);
        }

    }
}
