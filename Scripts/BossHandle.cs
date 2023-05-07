using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandle : MonoBehaviour
{
    public BossDoorMgr bossDoorMgr;

    private void OnDestroy()
    {
        bossDoorMgr.OpenDoor();
    }

    public void OpenDoor()
    {
        bossDoorMgr.OpenDoor();
    }

    public void CloseDoor()
    {
        bossDoorMgr.CloseDoor();
    }
}