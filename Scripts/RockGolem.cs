using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockGolem : Enemy
{
    public enum GolemState { Hidden, Rising, Fighting }
    public GolemState bossState = GolemState.Hidden;
    public Transform hiddenTransform;
    public Transform risenTransform;

    public GameObject bomberPrefab;

    public float riseSpeed = 1f;
    public int hp, maxHealth;
    public float fireDelay = 0.5f;
    public float spawnInterval = 5f;
    public float spawnRadius = 6f;
    
    private float risingElapsedTime = 0f;
    private HitDetector hitDetector;
    private FPSController playerCtrl;
    private bool isSummoning = false;

    protected List<LaunchProjectile> rifles;

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindWithTag("Player").transform;
        transform.position = hiddenTransform.position;
        hitDetector = gameObject.GetComponent<HitDetector>();
        hp = maxHealth = hitDetector.health;
        gameObject.GetComponentInChildren<Canvas>().enabled = false;
        playerCtrl = player.gameObject.GetComponent<FPSController>();
        rifles = new List<LaunchProjectile>(GetComponentsInChildren<LaunchProjectile>());
    }

    protected override void Update()
    {
        hp = hitDetector.health;
        switch (bossState)
        {
            case GolemState.Hidden:
                HideAndRise();
                break;
            case GolemState.Rising:
                Rising();
                break;
            case GolemState.Fighting:
                FightPlayer();
                break;
        }
    }

    private void HideAndRise()
    {
        float distance = Vector3.Distance(player.position, hiddenTransform.position);
        Debug.Log(distance);
        if (distance <= visRange)
        {
            bossState = GolemState.Rising;
        }
    }

    private void Rising()
    {
        risingElapsedTime += Time.deltaTime;
        float t = risingElapsedTime / riseSpeed;
        transform.position = Vector3.Lerp(hiddenTransform.position, risenTransform.position, t);

        if (t >= 1f)
        {
            bossState = GolemState.Fighting;
        }
    }

    private void FightPlayer()
    {
        gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        agent.SetDestination(player.position);
        float distance = Vector3.Distance(player.position, hiddenTransform.position);

        hp = hitDetector.health;
        float healthPercentage = (float)hp / maxHealth;

        if (healthPercentage < 0.25f)
        {
            // Stage 3: Charge
            charge();
        }
        else if (healthPercentage < 0.65f)
        {
            // Stage 2: Summon minions
            summonMinions();
        }
        else
        {
            // Stage 1: Shoot
            shoot();
        }
    }

    protected override void shoot() {
        // launchProjectile
        StartCoroutine(ShootRiflesWithDelay());
    }

    private void summonMinions()
    {
        if (!isSummoning) StartCoroutine(Summon(bomberPrefab));
    }

    private void charge()
    {

    }

    private IEnumerator ShootRiflesWithDelay()
    {
        for (int i = 0; i < rifles.Count; i++)
        {
            rifles[i].Fire();
            yield return new WaitForSeconds(fireDelay);
        }
    }

    private IEnumerator Summon(GameObject minionPrefab)
    {
        isSummoning = true;

        yield return new WaitForSeconds(spawnInterval);

        Vector3 spawnPosition = transform.position + (Random.insideUnitSphere * spawnRadius);
        spawnPosition.y = transform.position.y;

        Instantiate(minionPrefab, spawnPosition, Quaternion.identity);

        isSummoning = false;
    }

    private IEnumerator Charge()
    {
        isCharging = true;
        float elapsedTime = 0f;

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        while (elapsedTime < 2f && Vector3.Distance(transform.position, player.position) > 0.5f)
        {
            transform.position += dir * 15f * Time.deltaTime;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(stunDuration);
        isCharging = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerCtrl != null) playerCtrl.damage(5);
        }
    }
}

