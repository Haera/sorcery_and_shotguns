using System.Collections;
using UnityEngine;

public class Punch : MonoBehaviour
{
    public bool cl_righthand = true;
    public float armLength;

    private float cooldown;
    private Enemy parent;
    private float meleeRate;
    private Vector3 originPos;

    private void Start()
    {
        originPos = transform.localPosition;
        parent = transform.parent.GetComponent<Enemy>();
        meleeRate = parent.meleeRate * 0.75f;

        if (cl_righthand) {
            cooldown = 0;
        } else {
            cooldown = meleeRate + (meleeRate / 2);
        }
    }

    private void Update()
    {  
        cooldown += Time.deltaTime;

        float cycle = (cooldown % ((meleeRate / 2) * 2)) / ((meleeRate / 2) * 2);
        float sineValue = Mathf.Sin(cycle * 2 * 3.14159f);
        // worlds most beautiful + clamp                            \/  \/  \/  \/  \/  \/ 
        float zPosition = originPos.z + armLength * Mathf.Min(Mathf.Max(sineValue, 0), 1);
        transform.localPosition = new Vector3(originPos.x, originPos.y, zPosition);
    }

}