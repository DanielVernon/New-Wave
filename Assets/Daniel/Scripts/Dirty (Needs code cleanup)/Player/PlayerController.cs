using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TODO:
/// - Setup the input manager to work for controllers or buy rewired
/// </summary>

[RequireComponent(typeof(Rigidbody2D), typeof(ShootWave))]
public class PlayerController : MonoBehaviour {
    public PlayerInput playerInput = new PlayerInput();
    public Sounds sounds = new Sounds();
    public ProjectileSettings projectileSettings = new ProjectileSettings();
    [Range(0, -40)]
    public float gravityForce = -9.81f;
    [Range(0.5f,3.0f)]
    public float fallMultiplier = 1.0f;

    //HideInInspector
    [HideInInspector]
    public List<Collider2D> itemsICantCollideWith = new List<Collider2D>();
    [HideInInspector]
    public Vector2 lastFrameVelocity = Vector2.zero;
    [HideInInspector]
    public bool grounded = false;
    [HideInInspector]
    public Animator myAnimator = null;
    [HideInInspector]
    public Rigidbody2D myRbody = null;
    [HideInInspector]
    public Vector3 aimDirection = Vector3.zero;
    [HideInInspector]
    public ShootWave shootWave;
    /// <summary>
    /// True when you are holding jump after
    /// you have jumped, not being used
    /// </summary>
    [HideInInspector]
    public bool holdJump = false;
    [HideInInspector]
    public RubrixFollower rubrixFollower;
    //Private
    private bool isPaused = false;
    private AudioSource myAudio = null;
    private Camera mainCam = null;
    private Vector2 forceToApply = Vector2.zero;
    private Vector3 mousePos = Vector2.zero;
    private SpriteRenderer spriteRenderer;
    private Vector3 respawnPos = Vector3.zero;
    private bool couldShootAtStart = true;
    //Timers
    //[HideInInspector]
    public float shootTimer = 100;
    private float canJumpTimer = 0;
	// Use this for initialization
	void Start () {
        couldShootAtStart = playerInput.canShoot;
        respawnPos = transform.position;
        mainCam = Camera.main;
        myRbody = gameObject.GetComponent<Rigidbody2D>();
        shootWave = gameObject.GetComponent<ShootWave>();
        myAnimator = gameObject.GetComponentInChildren<Animator>();
        myAudio = gameObject.GetComponent<AudioSource>();
        aimDirection = Vector3.right;
        spriteRenderer = gameObject.GetComponentInChildren<Animator>().gameObject.GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        float fallForce = gravityForce;
        if (myRbody.velocity.y < 0)
            fallForce *= fallMultiplier;

        if (!grounded)
            myRbody.velocity += new Vector2(0, fallForce * Time.fixedDeltaTime);
    }
    bool jumpReset = true;
    bool jump2Reset = true;
    bool shootReset = true;
    bool shoot2Reset = true;
    // Update is called once per frame
    void Update () {
        if (isPaused)
            return;

        CheckGrounded();
        UpdateAimDirection();
        Vector2 controllerRightStick = Vector2.zero;

        float controllerMovement = Input.GetAxis("DpadHorizontal"); 
        if (controllerMovement == 0)
        {
            controllerMovement = Input.GetAxis("Horizontal");
        }

        float horizontalMovement = myRbody.velocity.x;
        if (!grounded)
            canJumpTimer += Time.deltaTime;
        else
            canJumpTimer = 0;

        if (Input.GetAxis("ShootJump") >= 0)
        {
            shootReset = true;
        }
        if (Input.GetAxis("ShootJump") <= 0)
        {
            jumpReset = true;
        }

        if (Input.GetAxis("Jump") == 0)
            jump2Reset = true;
            
        if (Input.GetAxis("Shoot") == 0)
            shoot2Reset = true;


        if ((Input.GetKeyDown(KeyCode.W) || (Input.GetAxis("Jump") > 0.1f && jump2Reset) || (Input.GetAxis("ShootJump") > 0.1f && jumpReset)) && (grounded || canJumpTimer < 0.1f))
        {
            if (Input.GetAxis("ShootJump") > 0.1f)
                jumpReset = false;
            if (Input.GetAxis("Jump") > 0.1f)
                jump2Reset = false;

            Jump();
            holdJump = true;
        }
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W) || myRbody.velocity.y < 0)
        {
            holdJump = false;
        }
        if (Input.GetKey(KeyCode.A) || controllerMovement < 0)
        {
            if (grounded)
            {
                //myAnimator.SetBool("Landing", false);
                myAnimator.SetBool("Walking", true);
            }
            else
                myAnimator.SetBool("Walking", false);
            spriteRenderer.transform.localScale = new Vector3(-Mathf.Abs(spriteRenderer.transform.localScale.x), spriteRenderer.transform.localScale.y, 0);
            spriteRenderer.transform.localPosition = new Vector3(Mathf.Abs(spriteRenderer.transform.localPosition.x), spriteRenderer.transform.localPosition.y, 0);

            horizontalMovement -= playerInput.Acceleration * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || controllerMovement > 0)
        {
            if(grounded)
            {
                //myAnimator.SetBool("Landing", false);
                myAnimator.SetBool("Walking", true);
            }
            else
                myAnimator.SetBool("Walking", false);

            spriteRenderer.transform.localScale = new Vector3(Mathf.Abs(spriteRenderer.transform.localScale.x), spriteRenderer.transform.localScale.y, 0);
            spriteRenderer.transform.localPosition = new Vector3(-Mathf.Abs(spriteRenderer.transform.localPosition.x), spriteRenderer.transform.localPosition.y, 0);

            horizontalMovement += playerInput.Acceleration * Time.deltaTime;
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            myAnimator.SetBool("Walking", false);
            if (grounded)
            {
                if (horizontalMovement < 0)
                {
                    horizontalMovement += Mathf.Min(playerInput.drag * Time.deltaTime, Mathf.Abs(horizontalMovement));
                }
                else
                    horizontalMovement -= Mathf.Min(playerInput.drag * Time.deltaTime, Mathf.Abs(horizontalMovement));
            }
            else
            {
                horizontalMovement -= Mathf.Min(playerInput.airDrag * Time.deltaTime, Mathf.Abs(horizontalMovement));
            }


        }
        shootTimer += Time.deltaTime;
        if (rubrixFollower)
        {
            if (shootTimer >= playerInput.shootCooldown - 0.3f && rubrixFollower.grayScale == 1)
            {
                rubrixFollower.SetGrayScale(0);
            }

            if (shootTimer >= 0.2f  && shootTimer < playerInput.shootCooldown - 0.1f && rubrixFollower.grayScale == 0)
            {
                rubrixFollower.SetGrayScale(1);
            }
        }
        if ((Input.GetMouseButton(0) || Input.GetKeyDown(KeyCode.Space) || (Input.GetAxis("Shoot") > 0.2f && shoot2Reset) || (Input.GetAxis("ShootJump") < -0.1f && shootReset))&& shootTimer >= playerInput.shootCooldown && playerInput.canShoot)
        {
            //rubrixFollower.SetGrayScale(1);
            if (Input.GetAxis("ShootJump") < -0.1f)
                shootReset = false;

            if (Input.GetAxis("Shoot") > 0.2f)
                shoot2Reset = false;

            //mainCam.SendMessage("AddShake", 0.4f, SendMessageOptions.DontRequireReceiver);
            //gameObject.SendMessage("AddShake", 0.7f, SendMessageOptions.DontRequireReceiver);

            //shootWave.startOffset = aimDirection.normalized *  ((transform.lossyScale.x/2 * Mathf.Sqrt(2)) + (shootWave.wave.transform.lossyScale.magnitude/2) + 0.01f);
            int layerMask = ~(1 << LayerMask.NameToLayer("Projectile") | 1 << LayerMask.NameToLayer("Player"));
            Vector3 offset = aimDirection;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, aimDirection.magnitude, layerMask);
            if (hit && hit.collider.gameObject)
            {
                offset = (hit.point + (Vector2)(-aimDirection.normalized * shootWave.wave.transform.lossyScale.magnitude/2) - (Vector2)transform.position);
                //Debug.Log("alternate");
            }
            if(rubrixFollower)
                rubrixFollower.Fire();

            if (!hit || hit.collider.CompareTag("BounceableWall") || hit.collider.isTrigger)
                shootWave.Shoot(transform.position + offset, projectileSettings.projectileLength, aimDirection, 2);
            myAudio.PlayOneShot(sounds.laserSound, sounds.laserVolume);
            shootTimer = 0;
        }

        if (!grounded && myRbody.velocity.y < 0)
        {
            myAnimator.SetBool("Falling", true);
            myAnimator.SetBool("Jumping", false);
        }

        horizontalMovement = Mathf.Clamp(horizontalMovement, -playerInput.movementSpeed, playerInput.movementSpeed);
        myRbody.velocity = new Vector2(horizontalMovement, Mathf.Min(myRbody.velocity.y, playerInput.terminalVelocity)) + forceToApply;
        lastFrameVelocity = myRbody.velocity;
        forceToApply = Vector2.zero;
	}

    /// <summary>
    /// Checks whether or not the player is grounded.
    /// And calls the land function if needed.
    /// </summary>
    void CheckGrounded()
    {
        List<Vector2> raycastPositions = new List<Vector2>();
        raycastPositions.Add(transform.position);
        raycastPositions.Add(transform.position + new Vector3(transform.lossyScale.x / 2 - 0.001f, 0, 0));
        raycastPositions.Add(transform.position - new Vector3(transform.lossyScale.x / 2 - 0.001f, 0, 0));
        bool hashit = false;
        foreach (Vector3 raycastPos in raycastPositions)
        {
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = false;
            contactFilter.layerMask = ~(1 << LayerMask.NameToLayer("Player") & 1 << LayerMask.NameToLayer("Projectile"));
            
            RaycastHit2D[] hits = new RaycastHit2D[15];
            Physics2D.Raycast((Vector2)raycastPos, Vector2.down, contactFilter, hits, transform.lossyScale.y / 2 + 0.1f);

            foreach (var item in hits)
            {
                if (item.collider != null && !itemsICantCollideWith.Contains(item.collider) &&
                    LayerMask.LayerToName(item.collider.gameObject.layer) != "Projectile" && LayerMask.LayerToName(item.collider.gameObject.layer) != "Player")
                {
                    hashit = true;
                    break;
                }
            }

            ////Raycast each one
            //if (hit)//Raycast hit true a
            //{
            //    hashit = true;
            //    break;
            //}
        }
        if (hashit && !grounded && myRbody.velocity.y <= 0)
        {
            Land();
        }
        else if (grounded && !hashit && myRbody.velocity.y <= 0)
        {
            myAnimator.SetBool("Falling", true);
        }
        grounded = hashit;
    }

    void Land()
    {
        if (myRbody != null && lastFrameVelocity.y != 0)
        {
            float landForce = -lastFrameVelocity.y;
            float fallTime = landForce / (-gravityForce * fallMultiplier);
            float fallDistance = -gravityForce * fallMultiplier * (fallTime * fallTime) / 2;
            //Debug.Log(landForce);
            float camShakeVal = scale01(fallDistance, 1f, 6.0f);

            mainCam.SendMessage("AddShake", camShakeVal, SendMessageOptions.DontRequireReceiver);

            myAudio.PlayOneShot(sounds.landSound, camShakeVal);
            myAnimator.SetBool("Falling", false);
            myAnimator.SetBool("Jumping", false);

            if (fallDistance > 0.5f)
            {
                myAnimator.SetBool("Landing", true);
                myAnimator.Play("Land", -1, 0);
                //if (myAnimator.GetBool(""))
                //{

                //}

                //myAnimator.SetBool("Landing", true);
                //myAnimator.Play("Land");
            }
            else
            {

            }
        }
    }

    public void Jump(float multiplier = 1)
    {
        canJumpTimer = 2;
        myRbody.velocity = new Vector2(myRbody.velocity.x, Mathf.Sqrt(-2.0f * gravityForce * playerInput.jumpHeight * multiplier));
        myAudio.PlayOneShot(sounds.jumpSound, sounds.jumpVolume);
        if(myAnimator.GetBool("Jumping"))
        {
            myAnimator.SetBool("Jumping", true);
            myAnimator.SetBool("Landing", false);
            myAnimator.Play("Jump", -1,0);
        }
        else
        {
            myAnimator.SetBool("Jumping", true);
            myAnimator.SetBool("Landing", false);
        }
        //myAnimator.Play("Jump");
    }
    
    public void ApplyForce(Vector2 force)
    {
        forceToApply += force;
    }

    void UpdateAimDirection()
    {
        // mousePos = getMousePos();
        UpdateMousePos();
        if (mousePos.magnitude < playerInput.innerMouseRadius)
            return;
        aimDirection = mousePos.normalized;
        switch (playerInput.aimMode)
        {
            case AimType.freeAim:
                {
                    //aimDirection = (getMousePos() - transform.position).normalized;
                    break;
                }
            case AimType.FourDirectional:
                {
                    //aimDirection = (getMousePos() - transform.position).normalized;
                    if (Mathf.Abs(aimDirection.x) > Mathf.Abs(aimDirection.y))
                    {
                        aimDirection.y = 0;
                    }
                    else
                        aimDirection.x = 0;
                    aimDirection.Normalize();
                    break;
                }
            case AimType.EightDirectional:
                {
                    //aimDirection = (getMousePos() - transform.position).normalized;
                    //Need to do eight directions
                    aimDirection.x = Mathf.RoundToInt(aimDirection.x);
                    aimDirection.y = Mathf.RoundToInt(aimDirection.y);
                    aimDirection.Normalize();
                    break;
                }
            default:
                break;
        }
    }
    private float mouseMovementTimer = 0;
    void UpdateMousePos()
    {
        if (mainCam)
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            if (x + y == 0) //&& mousePos.magnitude > playerInput.innerMouseRadius)
            {
                mouseMovementTimer += Time.deltaTime;
                if (mouseMovementTimer > 0.4f)
                {
                   // mousePos = Vector3.zero;
                    mouseMovementTimer = 0;
                    //mousePos = aimDirection;
                }
            }
            else
            {
                mouseMovementTimer = 0;
                mousePos += new Vector3(x,y,0);
            }
            mousePos.z = 0;
            mousePos.x = Mathf.Clamp(mousePos.x, -playerInput.mouseRadius, playerInput.mouseRadius);
            mousePos.y = Mathf.Clamp(mousePos.y, -playerInput.mouseRadius, playerInput.mouseRadius);
            //mousePos = Vector3.ClampMagnitude(mousePos, playerInput.mouseRadius);
            Vector3 returnVal = transform.position + mousePos;
            //Vector3 returnVal = mainCam.ScreenToWorldPoint(mousePos);
            returnVal.z = transform.position.z;
            
        }
        else
            mousePos = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //if (Input.GetKey(KeyCode.UpArrow))
            //{
            //    mousePos = new Vector3(1,1,0).normalized * playerInput.mouseRadius;
            //}
            //else if (Input.GetKey(KeyCode.DownArrow))
            //{
            //    mousePos = new Vector3(1, -1, 0).normalized * playerInput.mouseRadius;
            //}
            //else
                mousePos = Vector3.right * playerInput.mouseRadius;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //if (Input.GetKey(KeyCode.UpArrow))
            //{
            //    mousePos = new Vector3(-1, 1, 0).normalized * playerInput.mouseRadius;
            //}
            //else if (Input.GetKey(KeyCode.DownArrow))
            //{
            //    mousePos = new Vector3(-1, -1, 0).normalized * playerInput.mouseRadius;
            //}
            //else
                mousePos = -Vector3.right * playerInput.mouseRadius;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            mousePos = Vector3.up * playerInput.mouseRadius;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            mousePos = -Vector3.up * playerInput.mouseRadius;
        }

        Vector2 controllerRightStick = Vector2.zero;
        controllerRightStick.x = Input.GetAxis("HorizontalRight");
        controllerRightStick.y = Input.GetAxis("VerticalRight");

        if (controllerRightStick.magnitude > 0.5f)
        {
            mousePos = controllerRightStick.normalized * playerInput.mouseRadius;
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one * playerInput.mouseRadius * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + mousePos, Vector3.one * 0.4f);
        Gizmos.DrawWireSphere(transform.position, playerInput.innerMouseRadius);
        Gizmos.color = Color.white;
        if (mainCam != null)
        {
            //Gizmos.DrawCube(getMousePos(), new Vector3(0.1f, 0.1f, 0.1f));



            Gizmos.DrawLine(transform.position, transform.position + aimDirection * 2);
        }
    }

    public void OnValidate()
    {
        float fallTime = Mathf.Sqrt((playerInput.jumpHeight * 2) / (-gravityForce * -gravityForce));
        float fallDistance = -gravityForce * fallMultiplier * (fallTime * fallTime) / 2;
        playerInput.jumpDistance = playerInput.movementSpeed * (fallTime * fallTime) / 2;
    }
    [System.Serializable]
    public class PlayerInput
    {
        public bool canShoot = true;
        [Range(0, 10)]
        public float movementSpeed = 5.0f;
        [Range(0, 100)]
        public float Acceleration = 40;
        [Range(0, 20)]
        public float jumpHeight = 2.0f;

        [ReadOnly]
        public float jumpDistance = 0;
        [Range(0, 15)]
        public float airDrag = 2.0f;
        [Range(0, 100)]
        public float drag = 40.0f;
        [Range(0, 10)]
        public float shootCooldown = 1;
        public float terminalVelocity = 30;

        [Range(0,5)]
        public float mouseRadius = 2;
        [Range(0, 3)]
        public float innerMouseRadius = 1f;

        public AimType aimMode = AimType.EightDirectional;
    }

    public enum AimType
    {
        freeAim,
        FourDirectional,
        EightDirectional
    }

    [System.Serializable]
    public class Sounds
    {
        public AudioClip laserSound = null;
        [Range(0.0f, 1.0f)]
        public float laserVolume = 1.0f;
        public AudioClip jumpSound = null;
        [Range(0.0f, 1.0f)]
        public float jumpVolume = 1.0f;
        public AudioClip landSound = null;
    }

    [System.Serializable]
    public class ProjectileSettings
    {
        [Range(0,10)]
        public float projectileLength = 3;
        [Range(0,20)]
        public float BounceHeight = 2.0f;
    }


    public static float scale01(float value, float min, float max)
    {
        return Mathf.Clamp01(((value - min) * value) / ((max * value) - (min * value)));
    }

    private void Pause(bool pause)
    {
        isPaused = pause;
    }

    public void SetCanShoot(bool canShoot)
    {
        StartCoroutine(SetCanShootDelay(0.01f, canShoot));
        //playerInput.canShoot = canShoot;
    }

    public IEnumerator SetCanShootDelay(float delay, bool canShoot)
    {
        yield return new WaitForSeconds(delay);
        playerInput.canShoot = canShoot;
    }


    public void Respawn()
    {
        transform.position = respawnPos;
        playerInput.canShoot = couldShootAtStart;
        if (!playerInput.canShoot && rubrixFollower)
        {
            rubrixFollower.Respawn(true);
        }
    }
}
