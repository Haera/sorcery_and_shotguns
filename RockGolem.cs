using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockGolem : Enemy
{
    public enum GolemState { Hidden, Rising, Fighting }
    public GolemState bossState = GolemState.Hidden;
    public Transform hiddenTransform, risenTransform;

    public GameObject[] minionPrefabs;
    public float[] minionSpawnWeights;

    public float riseSpeed = 1f;
    public int hp, maxHealth;
    public float fireDelay = 0.5f;
    public float spawnInterval = 5f;
    public float spawnRadius = 6f;
    public float stunDuration = 2f;
    
    private float risingElapsedTime = 0f;
    private HitDetector hitDetector;
    private FPSController playerCtrl;
    private bool isSummoning = false;
    private bool isCharging = false;

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
        
        // raise the gates!
        gameObject.GetComponent<BossHandle>().CloseDoor();

        transform.Rotate(new Vector3(0, 90f / t * Time.deltaTime, 0), Space.World);

        if (t >= 1f)
        {
            bossState = GolemState.Fighting;
            gameObject.GetComponentInChildren<Canvas>().enabled = true;
        }
    }

    private void FightPlayer()
    {
        float distance = Vector3.Distance(player.position, hiddenTransform.position);

        hp = hitDetector.health;
        float hpPct = (float)hp / maxHealth;

        if (hpPct < 0.25f)
        {
            // Stage 3: Charge
            gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            if (!isCharging) StartCoroutine(Charge());
        }
        else if (hpPct < 0.65f)
        {
            // Stage 2: Summon minions
            gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
            agent.SetDestination(player.position);
            if (!isSummoning) StartCoroutine(Summon());
        }
        else
        {
            // Stage 1: Shoot
            gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
            agent.SetDestination(player.position);
            StartCoroutine(ShootRiflesWithDelay());
        }
    }

    private IEnumerator ShootRiflesWithDelay()
    {
        for (int i = 0; i < rifles.Count; i++)
        {
            rifles[i].Fire();
            yield return new WaitForSeconds(fireDelay);
        }
    }

    private IEnumerator Summon()
    {
        isSummoning = true;

        yield return new WaitForSeconds(spawnInterval);

        float totalWeight = 100; // weights need to sum to 100

        float randomWeight = Random.Range(0, totalWeight);
        float acc = 0;
        int idx = 0;

        for (int i = 0; i < minionPrefabs.Length; i++)
        {
            acc += minionSpawnWeights[i];
            if (randomWeight <= acc)
            {
                idx = i;
                break;
            }
        }

        GameObject chosenPrefab = minionPrefabs[idx];

        Vector3 spawnPosition = transform.position + (Random.onUnitSphere * spawnRadius);
        spawnPosition.y = transform.position.y;

        Instantiate(chosenPrefab, spawnPosition, Quaternion.identity);

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

