using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform camTrans;

    void Start()
    {
        camTrans = GameObject.FindWithTag("MainCamera").transform;
    }

    // called AFTER update frame
    void LateUpdate()
    {
        Vector3 target = transform.position - (camTrans.position - transform.position);
        transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
    }
}
