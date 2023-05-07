using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float attackRate = 2f;
    private float attackCooldown;
    public Transform player;

    public ProjectileType type = ProjectileType.Default;


    // Start is called before the first frame update
    void Start()
    {
        attackCooldown = 0;
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    public void Initialize(Transform playerInit)
    {
        player = playerInit; // ?
        //player = GameObject.FindWithTag("Player").transform;
    }

    public void Fire()
    {
        if (attackCooldown <= 0)
        {
            ShootProjectile();
            attackCooldown = attackRate;
        }
    }

    private void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        float dist = Vector3.Distance(transform.position, player.position);
        Debug.Log(dist);
        

        //                                       relForce          dmg   type
        //projectileScript.Initialize(new Vector3 (400f, 250f, 0), 5f, "default");

        switch (type) {
            case ProjectileType.Default:
                Debug.Log("no projectile case: Default");
                break;
            case ProjectileType.Cannon:
                float mult = Mathf.Sqrt(dist)/3f;
                projectileScript.Initialize(new Vector3 (400f * mult, 250f * mult, 0), 20, 300f, type);
                break;
            case ProjectileType.Bullet:
                projectileScript.Initialize(new Vector3 (700f, 0, 0), 5, 0f, type);
                break;
        }
        
    }
}
