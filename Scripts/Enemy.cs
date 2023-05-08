using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    protected Vector3 dst;
    protected bool dstFound, playerSpotted;
    public float dstRange, visRange, shootRange, meleeRange, meleeRate, turnSpeed;
    protected float meleeCooldown;
    private Vector3 original_pos;
    public EnemyState currentState = EnemyState.Idle;
    public LaunchProjectile rifle;
    public GameObject playerObj;
    private FPSController fpsc;
    private bool isAggro = false;

    public string sound_enemy1_1 = "Enemy1_1.wav";
    public string sound_enemy1_2 = "Enemy1_2.wav";
    public string sound_enemy1_3 = "Enemy1_3.wav";
    public string sound_enemy1_4 = "Enemy1_4.wav";
    public string sound_enemy1_5 = "Enemy1_5.wav";
    public string sound_enemy2_1 = "Enemy2_1.wav";
    public string sound_enemy2_2 = "Enemy2_2.wav";
    public string sound_enemy2_3 = "Enemy2_3.wav";
    public string sound_enemy2_4 = "Enemy2_4.wav";
    public string sound_enemy2_5 = "Enemy2_5.wav";
    public string sound_drone1 = "Drone_Passive.wav";
    public string sound_drone2 = "Drone_Aggressive.wav";

    protected virtual void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        rifle = GetComponentInChildren<LaunchProjectile>();
        player = GameObject.FindWithTag("Player").transform;
        fpsc = playerObj.GetComponent<FPSController>();
        original_pos = transform.position;
        //currentState = EnemyState.Patrol;
    }

    protected virtual void Update()
    {
        // update visibility booleans
        playerSpotted = Physics.CheckSphere(transform.position, visRange, whatIsPlayer);
        updateState(playerSpotted);

        switch (currentState) {
            case EnemyState.Idle:
                break;
            case EnemyState.Patrol:
                patrol();
                break;
            case EnemyState.Chase:
                chase();
                break;
            case EnemyState.Shoot:
                shoot();
                break;
            case EnemyState.Melee:
                if (meleeCooldown <= 0) {
                    melee();
                    meleeCooldown = meleeRate;
                }
                break;
            case EnemyState.Flee:
                break;
        }
        
        if (meleeCooldown > 0)
        {
            meleeCooldown -= Time.deltaTime;
        }
    }

    // update curState
    protected virtual void updateState(bool pSpotted) 
    {  
        // short circuit if idle -- must be manually set
        if (currentState == EnemyState.Idle) return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (!pSpotted) {
            currentState = EnemyState.Patrol;
        } else {
            if (dist <= meleeRange) {
                currentState = EnemyState.Melee;
            } else if (dist <= shootRange) {
                currentState = EnemyState.Shoot;
            } else {
                currentState = EnemyState.Chase;
            }
        }
    }

    protected void findDst()
    {
        // this might be uniform? idk check l8r 
        // https://forum.unity.com/threads/centre-of-sphere-for-random-insideunitsphere.83824/
        Vector3 randOff = Random.insideUnitSphere * dstRange;
        randOff.y = 0f;
        dst = original_pos + randOff;

        if (Physics.Raycast(dst, -transform.up, 2f, whatIsGround)) dstFound = true;
    }

    protected void patrol()
    {
        if(isAggro){
            fpsc.aggroCount--;
            isAggro = false;
        }
        if (!dstFound) findDst();
        if (gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled == true) agent.SetDestination(dst);

        if ((transform.position - dst).magnitude < 1f) dstFound = false;
    }

    protected virtual void chase()
    {
        if(!isAggro){
            fpsc.aggroCount++;
            isAggro = true;
        }
        //if (gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled == true) 
        agent.SetDestination(player.position);
        if (gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled == true) agent.SetDestination(player.position);
        // smooth this later..
        //transform.LookAt(player);

        // did it!
        TurnTo(player, turnSpeed);
    }

    protected virtual void shoot()
    {
        //transform.LookAt(player);
        transform.LookAt(player);
        // launchProjectile
        if(rifle == null) {
            Debug.Log("no rifle!");
        } else {
            rifle.Fire();
        }
    }

    protected void melee()
    {
        transform.LookAt(player);
        FPSController playerController = player.gameObject.GetComponent<FPSController>();
        playerController.damage(25);
    }

    // it's like transform.LookAt but way cooler (and smoother..)
    public void TurnTo(Transform target, float turnRate) {
        Vector3 dir = target.position - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnRate);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, dstRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }

    public void playSound (string clipName) {
        AudioClip clip = (AudioClip)Resources.Load("Lewis Sounds/Sound FX/"+clipName);
        GetComponent<AudioSource>().PlayOneShot(clip, 1f);
    }
    
}