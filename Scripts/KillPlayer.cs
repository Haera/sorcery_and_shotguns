using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    public GameObject player;
    private FPSController fpsc; 
    void Start () {
        fpsc = player.GetComponent<FPSController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hiiii");
        if (other.CompareTag(player.tag))
        {
            Debug.Log("Player collided with lava.");
            fpsc.damage(9999);
        }
    }
}