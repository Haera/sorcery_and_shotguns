using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDelay : MonoBehaviour
{
    public float delay;

    void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0f) {
            Destroy(this.gameObject);
        }
    }
}
