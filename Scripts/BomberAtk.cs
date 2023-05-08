using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberAtk : Enemy
{
    public float detonateRange;
    public float detonateTime = 5.0f;
    public float explodeRange;
    private float elapsedTime;
    private bool isDetonating = false;
    private Renderer objectRenderer;
    private bool lockedOn = false;
    public Color initialColor;
    public Color pulseColor;

    public Vector3 originalPos;
    public float hopHeight = 1.0f;
    public float spinSpeed = 360.0f;
    public float speedMultiplier = 2.0f;

    private FPSController playerCtrl;
    private HitDetector hitDetector;

    protected override void Start()
    {
        base.Start();
        objectRenderer = GetComponentInChildren<Renderer>();
        initialColor = objectRenderer.material.color;
        originalPos = transform.position;
        playerCtrl = player.gameObject.GetComponent<FPSController>();
        hitDetector = gameObject.GetComponent<HitDetector>();
    }

    protected override void updateState(bool pSpotted)
    {
        // short circuit if idle -- must be manually set
        //transform.position = Vector3.MoveTowards(transform.position, player.position, 3f * Time.deltaTime);
        if (currentState == EnemyState.Idle || transform.childCount <= 0) return;

        float dist = Vector3.Distance(player.position, transform.GetChild(0).position);

        if (!pSpotted && !lockedOn)
        {
            currentState = EnemyState.Patrol;
            spin(0);
        }
        else
        {
            if (dist <= detonateRange || lockedOn)
            {
                detonate(dist);
                gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                currentState = EnemyState.Boom;
                lockedOn = true;
            } else {
                gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
                currentState = EnemyState.Chase;
                spin(0.5f);
            }
        }
    }

    protected override void chase()
    {
        //if (gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled == true) 
        agent.SetDestination(player.position);

        transform.LookAt(player);
    }

    private void detonate(float dist)
    {
        elapsedTime += Time.deltaTime;
        float pulseSpeed = elapsedTime / detonateTime;
        
        //hop(pulseSpeed);
        spin(4); // accelerate with 4u/s
        // Pulse color using a sine wave
        float colorPulse = (Mathf.Sin(2 * Mathf.PI * pulseSpeed * 2) + 1) / 2;
        objectRenderer.material.color = Color.Lerp(initialColor, pulseColor, colorPulse);

        if (elapsedTime >= detonateTime && dist < explodeRange)
        {
            hitDetector.damage(999);
            playerCtrl.damage(Mathf.Min((int) playerCtrl.maxHealth, Mathf.FloorToInt(100f / dist)));
            isDetonating = false;
            elapsedTime = 0;
            objectRenderer.material.color = initialColor;
            //agent.speed = baseSpeed; // Reset speed to normal
        } else {
            // stop pursuing if boom time
            transform.GetChild(0).position = Vector3.MoveTowards(transform.GetChild(0).position, player.position, 5f * Time.deltaTime);
        }
    }

    private void spin(float accel)
    {
       transform.GetChild(0).Rotate(new Vector3(0, spinSpeed * Time.deltaTime, 0), Space.World);
       if (spinSpeed <= 1200) spinSpeed += accel; // FASTER
    }

    protected override void OnDrawGizmosSelected() {
        if (transform.childCount <= 0) return;
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.GetChild(0).position, dstRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.GetChild(0).position, visRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.GetChild(0).position, detonateRange);
    }
}