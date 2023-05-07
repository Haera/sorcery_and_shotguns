using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FPSController : MonoBehaviour
{

    public LayerMask layerMask;
    public Transform spawn;
    public GameObject camObj;
    public GameObject E_to_interact;
    private TMPro.TextMeshProUGUI eti_textMeshPro;
    public GameObject healthBar;
    private Slider healthSlider;
    public GameObject fuelBar;
    private Slider fuelSlider;
    public float lookSensitivity;

    public bool showPlayerCollision;
    public GameObject castHit;
    private GameObject tempHit;

    private int currentWeapon = 1;
    private GameObject equippedWeapon;
    private bool unlockedRapid = false;
    private bool unlockedHitscan = false;
    private bool unlockedRpg = false;
    public GameObject singleProjectile;
    public GameObject singleshot;
    public GameObject hitscanProjectile;
    public GameObject hitscan;
    public GameObject rapidProjectile;
    public GameObject rapidshot;
    public GameObject rpgProjectile;
    public GameObject rpg;
    private float currentSingleDelay = 0;
    private float currentHitscanDelay = 0;
    private float currentRapidDelay = 0;
    private float currentRpgDelay = 0;
    public float singleDelay;
    public float hitscanDelay;
    public float rapidDelay;
    public float rpgDelay;
    public int hitscanDmg = 100;
    public Transform firingPoint;

    public float jumpHeight, moveSpeed, dashSpeed, dashDuration, gravity, maxScalableAngle;
    public float health, maxHealth = 100;
    private float yVelocity, dashTime = 0f;
    public static int dashMax = 1;//total allowed air dashes, reset dashCount when hitting enemies?
    private int dashCount = dashMax;
    public static int jumpMax = 2;//total allowed air jumps
    private int jumpCount = jumpMax;
    private int jumping = 0;
    public static float fuelMax = 5f;//fuelMax/fuelBurnRate = total flight time
    private float fuel = fuelMax;
    public float fuelPower;//how far 1 fuel will get you
    public float fuelBurnRate;//rate of fuel use

    void Start()
    {
        //locks cursor into the game window
        Cursor.lockState = CursorLockMode.Confined;

        //hide cursor
        Cursor.visible = false;

        if (showPlayerCollision) {
            tempHit = Instantiate(castHit, new Vector3(0, -50f, 0), this.transform.rotation);
        }

        healthSlider = healthBar.GetComponent<Slider>();
        fuelSlider = fuelBar.GetComponent<Slider>();

        eti_textMeshPro = E_to_interact.GetComponent<TMPro.TextMeshProUGUI>();
        
        equippedWeapon = singleshot;
        equip(singleshot);
        
    }

    void Update()
    {
        //handles mouse look
        Look();
        //handles shooting
        Shoot();
        //handles all other movement
        Move();

        //displaying fuel -- 0 index inside of canvas
        //canv.GetComponentsInChildren<TMPro.TextMeshProUGUI>()[0].text = "Fuel: " + fuel.ToString();
        fuelSlider.value = fuel / fuelMax;
        //displaying hp -- 1 index in canv
        //canv.GetComponentsInChildren<TMPro.TextMeshProUGUI>()[1].text = "HP: " + health.ToString();
        healthSlider.value = health / maxHealth;

    }

    private void Look() {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 playerRotation = transform.localEulerAngles;
        playerRotation.y += mouseX * lookSensitivity;
        transform.localRotation = Quaternion.AngleAxis(playerRotation.y, Vector3.up);

        Vector3 cameraRotation = camObj.transform.localEulerAngles;
        cameraRotation.x -= mouseY * lookSensitivity;

        //clamping camera between what the editor calls 90 and -90 degrees
        if (cameraRotation.x > 90f && cameraRotation.x < 180f) {
            cameraRotation.x = 90f;
        } else if (cameraRotation.x < 270f && cameraRotation.x > 180f) {
            cameraRotation.x = 270f;
        }

        camObj.transform.localRotation = Quaternion.AngleAxis(cameraRotation.x, Vector3.right);

        //TODO CURRENTLY NEED TO ADD A LAYERMASK FOR OBELISKS
        //handling interaction prompt
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(firingPoint.position, camObj.transform.forward, out hit, 2f, layerMask)) {
            if (hit.collider.GetComponent<Interactable>() != null) {
                eti_textMeshPro.text = "[E]";
                if (Input.GetKeyDown(KeyCode.E)) {
                    hit.collider.GetComponent<Interactable>().interact(this);
                }
            }
        } else {
            eti_textMeshPro.text = "";
        }

    }

    private void Move() {
        RaycastHit hit = new RaycastHit();
        if (jumping != 0) {
            jumping--;
        }
        bool grounded = false;

        //handling jumping
        if (Physics.SphereCast(transform.position, .5f, Vector3.down, out hit, .7f) && jumping == 0 && Vector3.Angle(Vector3.up, hit.normal) < maxScalableAngle) {
            //case grounded

            yVelocity = 0f;
            jumpCount = jumpMax;
            fuel = fuelMax;
            grounded = true;

            //resetting available dashes
            if (dashCount < dashMax && dashTime <= .05f) {
                dashCount = dashMax;
            }

            //jumping
            if (Input.GetKeyDown(KeyCode.Space) && jumping == 0) {
                yVelocity = jumpHeight;
                jumping = 6;
            }

        } else {
            //case not grounded

            yVelocity += gravity * Time.deltaTime;

            //air jumping
            if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0) {
                jumpCount -= 1;
                yVelocity = jumpHeight;
            }

        }


        Vector3 speed = new Vector3(0f, 0f, 0f);

        //standard movement
        if (Input.GetKey(KeyCode.W)) {
            speed += Vector3.forward * moveSpeed;
        }
        if (Input.GetKey(KeyCode.A)) {
            speed += Vector3.left * moveSpeed;
        }
        if (Input.GetKey(KeyCode.S)) {
            speed += Vector3.back * moveSpeed;
        }
        if (Input.GetKey(KeyCode.D)) {
            speed += Vector3.right * moveSpeed;
        }

        //dashing
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashTime <= .05f && dashCount > 0) {
            dashTime += dashDuration;
            dashCount -= 1;
        } 

        //applying gravity
        speed += new Vector3(0f, yVelocity, 0f);

        //apply dash after gravity to cancel fall
        if (dashTime > .05f) {
            speed *= dashSpeed;
            yVelocity = 0;
            dashTime -= Time.deltaTime;
        }

        //flight
        if (dashTime <= .05f && Input.GetKey(KeyCode.Mouse1) && fuel > .05f) {
            if (!grounded || camObj.transform.localRotation.z < 0) {//if your on the ground you can fly down
                speed += camObj.transform.localRotation * Vector3.forward * fuelPower;
            } else {//otherwise no flying through the floor for you
                Vector3 thrust = camObj.transform.localRotation * Vector3.forward * fuelPower;
                if (thrust.y < 0) {
                    thrust.y = 0;
                }
                speed += thrust;
            }
            fuel -= Time.deltaTime;
        }

        //used for casting
        Vector3 capsuleTop = new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z);
        Vector3 capsuleBottom = new Vector3(transform.position.x, transform.position.y - .5f, transform.position.z);

        //if showing collisions then move indicator to rough location of collision
        if (Physics.CapsuleCast(capsuleTop, capsuleBottom, .5f, transform.TransformDirection(speed), out hit, speed.magnitude * Time.deltaTime) && showPlayerCollision) {
            tempHit.transform.position = this.transform.position + (speed * Time.deltaTime);
        }

        bool sloped = false;

        //case where ground is sloped
        if (Physics.SphereCast(transform.position, .5f, Vector3.down, out hit, .5f) && jumping == 0 && hit.normal != Vector3.up) {
            
            Vector3 oldSpeed = speed;
            speed = Vector3.ProjectOnPlane(this.transform.TransformVector(speed), hit.normal);
            speed = this.transform.InverseTransformVector(speed);
            if (Vector3.Angle(Vector3.up, hit.normal) < maxScalableAngle) {
                sloped = true;
            }
            
        }

        //check y dir
        if (!grounded && !sloped && Physics.CapsuleCast(capsuleTop, capsuleBottom, .5f, transform.TransformDirection(new Vector3(0, speed.y, 0)), out hit, speed.y * Time.deltaTime)) {
            speed = new Vector3(speed.x, 0, speed.z);
        }

        //check x dir
        if (Physics.CapsuleCast(capsuleTop, capsuleBottom, .5f, transform.TransformDirection(new Vector3(speed.x, 0, 0)), out hit, speed.x * Time.deltaTime)) {
            speed = new Vector3(0, speed.y, speed.z);
        }

        //check z dir
        if (Physics.CapsuleCast(capsuleTop, capsuleBottom, .5f, transform.TransformDirection(new Vector3(0, 0, speed.z)), out hit, speed.z * Time.deltaTime)) {
            speed = new Vector3(speed.x, speed.y, 0);
        }

        //actually perform the movement
        transform.Translate(speed * Time.deltaTime, Space.Self);


    }

    private void Shoot() {

        if (Input.GetKeyDown(KeyCode.Alpha1)) {//single shot
            currentWeapon = 1;
            equip(singleshot);
        } else if (Input.GetKeyDown(KeyCode.Alpha2) && unlockedRapid) {//rapid shot
            currentWeapon = 2;
            equip(rapidshot);
        } else if (Input.GetKeyDown(KeyCode.Alpha3) && unlockedHitscan) {//hitscan
            currentWeapon = 3;
            equip(hitscan);
        } else if (Input.GetKeyDown(KeyCode.Alpha4) && unlockedRpg) {//rpg
            currentWeapon = 4;
            equip(rpg);
        }

        if (Input.GetKey(KeyCode.Mouse0)) {
            if (currentWeapon == 1 && currentSingleDelay <= 0) {//single shot

                Instantiate(singleProjectile, firingPoint.position, camObj.transform.rotation);

                currentSingleDelay = singleDelay;
            } else if (currentWeapon == 2 && currentRapidDelay <= 0) {//rapid shot

                Instantiate(rapidProjectile, firingPoint.position, camObj.transform.rotation);

                currentRapidDelay = rapidDelay;
            } else if (currentWeapon == 3 && currentHitscanDelay <= 0) {//hitscan

                RaycastHit hit;
                float hitscanRange = 1000f;
                if (Physics.Raycast(firingPoint.position, firingPoint.forward, out hit, hitscanRange)) {//hit
                    //this is just the visual of the projectile
                    Vector3[] tempArray = {firingPoint.position, hit.point};
                    hitscanProjectile.GetComponent<LineRenderer>().SetPositions(tempArray);

                    //damaging enemies
                    HitDetector enemy = hit.collider.GetComponent<HitDetector>();
                    if (enemy != null) {
                        enemy.damage(hitscanDmg);
                    }

                    rpgProjectile rpg = hit.collider.GetComponent<rpgProjectile>();
                    if (rpg != null) {
                        rpg.explode();
                    }
                    

                } else {//miss
                    //this is just the visual of the projectile
                    Vector3[] tempArray = {firingPoint.position, firingPoint.forward + (firingPoint.forward * hitscanRange)};
                    hitscanProjectile.GetComponent<LineRenderer>().SetPositions(tempArray);
                }
                Instantiate(hitscanProjectile);

                currentHitscanDelay = hitscanDelay;
            } else if (currentWeapon == 4 && currentRpgDelay <= 0) {//rpg

                Instantiate(rpgProjectile, firingPoint.position, camObj.transform.rotation);

                currentRpgDelay = rpgDelay;

            }
            
        }
        currentSingleDelay -= Time.deltaTime;
        currentRapidDelay -= Time.deltaTime;
        currentHitscanDelay -= Time.deltaTime;
        currentRpgDelay -= Time.deltaTime;

    }

    public void damage(int val) {
        health -= val;
        if (health <= 0) {
            //no death screen currently
            //transform.position = spawn.position;
            //health = 100;
            //fuel = 5;
            //jumpCount = jumpMax;
            //yVelocity = 0;
            //redo enemies
        }
    }

    public void drainFuel(int val) {
        fuel -= val;
        if (fuel < 0) {
            fuel = 0;
        }
    }

    public IEnumerator slowEffect(float percent, float duration) {
        moveSpeed = moveSpeed * percent;
        yield return new WaitForSeconds(duration);
        moveSpeed = moveSpeed / 2;
    }

    public float getHealth () {
        return health;
    }
    
    public void unlockWeapon(int weapon) {
        if (weapon == 2) {
            unlockedRapid = true;
        } else if (weapon == 3) {
            unlockedHitscan = true;
        } else if (weapon == 4) {
            unlockedRpg = true;
        }
    }

    //handles making the correct weapon visible
    public void equip(GameObject equipping) {
        equippedWeapon.SetActive(false);
        equippedWeapon = equipping;
        equippedWeapon.SetActive(true);
    }

}
