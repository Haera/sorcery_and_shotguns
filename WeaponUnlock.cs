using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUnlock : MonoBehaviour, Interactable
{

    public int weaponNumber;
    public void interact(FPSController player) {
        player.unlockWeapon(weaponNumber);
        Destroy(this.gameObject);
    }
}
