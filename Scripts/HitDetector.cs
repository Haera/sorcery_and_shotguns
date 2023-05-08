using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HitDetector : MonoBehaviour
{

   public int health;
   public int maxHealth;

   public Slider healthBar;

   //private ProgressBar healthBar;

   //current implementation means each enemy has exactly one HitDetector on it, could be changed using a extra enemy stats script if we want to

   void Start() {
      if (healthBar != null) healthBar.maxValue = maxHealth;
   }

   void Update() {
      if (healthBar != null) healthBar.value = health;
   }

   public void damage(int val) {
      health -= val;
      if (health <= 0) {
         Destroy(this.gameObject);
      }
   }

}

