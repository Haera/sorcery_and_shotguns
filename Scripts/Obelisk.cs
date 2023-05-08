using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obelisk : MonoBehaviour, Interactable
{

    public void interact(FPSController player) {
        player.health = 100;
        this.gameObject.layer = 0;
        Destroy(this);
    }

}
