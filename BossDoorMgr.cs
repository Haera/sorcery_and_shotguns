using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoorMgr : MonoBehaviour
{
    public BossHandle bossHandle;
    public Transform bossDoor;
    public Transform downDoor, upDoor;

    public void Start() {
        bossDoor = GameObject.FindWithTag("BossDoor").transform;
    }

    public void OpenDoor()
    {
        StartCoroutine(Open());
    }

    public void CloseDoor()
    {
        StartCoroutine(Close());
    }

    IEnumerator Open()
    {
        Debug.Log("u win! opening gate...");

        float elapsedTime = 0f;

        while (elapsedTime < 5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 5f;
            bossDoor.position = Vector3.Lerp(upDoor.position, downDoor.position, t);
            yield return null;
        }

        Debug.Log("gate opened!");
    }

    IEnumerator Close()
    {
        Debug.Log("closing gate..");

        float elapsedTime = 0f;

        while (elapsedTime < 5f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 5f;
            bossDoor.position = Vector3.Lerp(downDoor.position, upDoor.position, t);
            yield return null;
        }

        Debug.Log("gate closed!");
    }
}