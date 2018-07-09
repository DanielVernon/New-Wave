using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class RubrixFollower : MonoBehaviour {
    private PlayerController player;

    public float firingTime = 1;
    public bool isFiring = false;

    public float maxSpeed = 3.0f;
    public float maxDistance = 0.2f;
    public AnimationCurve returnToPlayer = AnimationCurve.EaseInOut(0,0,1,1);
    private Vector3 velocity = Vector3.zero;
    private Vector3 distanceFromTarget = Vector3.zero;
    private Vector3 firingPos = Vector3.zero;
    private bool returningToPlayer = false;
    private float firingTimer = 0;
    private float returnTimer = 0;
    private Vector3 lerpPos = Vector3.zero;
    private Animator myAnimator;
    private bool canShoot = true;
    private Vector3 respawnPos;
    public float grayScale = 0;
    private Material myMaterial;
	// Use this for initialization
	void Start () {
        respawnPos = transform.position;
        player = FindObjectOfType<PlayerController>();
        player.rubrixFollower = this;
        myAnimator = gameObject.GetComponent<Animator>();
        myMaterial = gameObject.GetComponent<Renderer>().material;
        if (!player.playerInput.canShoot)
        {
            myAnimator.SetBool("Crying", true);
            canShoot = false;
        }
        else
        {

            transform.SetParent(player.transform);
            transform.position = player.transform.position;
       }
    }
    
    private void FixedUpdate()
    {
        if(!returningToPlayer)
            transform.Translate(velocity * Time.fixedDeltaTime);
        
        //if (velocity.magnitude * Time.fixedDeltaTime > distanceFromTarget.magnitude)
        //{
        //    transform.position += distanceFromTarget;
        //    distanceFromTarget = Vector3.zero;
        //}
    }

    // Update is called once per frame
    void Update () {
        if(!canShoot)
        {
            if (player.playerInput.canShoot)
            {
                canShoot = true;
                myAnimator.SetBool("Crying", false);
            }
            else
            {
                transform.parent = null;
                return;
            }
        }

        if (isFiring)
        {
            firingTime = player.projectileSettings.projectileLength / player.shootWave.speed;

            firingTimer += Time.deltaTime;
            velocity = FlyToTarget(firingPos);
            if (firingTimer >= firingTime)
            {
                myAnimator.SetBool("Shooting", false);
                isFiring = false;
                firingTimer = 0;
                lerpPos = player.transform.position + player.aimDirection - Vector3.forward;
                returningToPlayer = true;
            }
        }
        else if (returningToPlayer)
        {
            lerpPos += FlyToTarget(player.transform.position + player.aimDirection) * Time.deltaTime;

            returnTimer += Time.deltaTime / player.playerInput.shootCooldown;
            transform.position = Vector3.Lerp(firingPos, lerpPos, returnToPlayer.Evaluate(player.shootTimer / player.playerInput.shootCooldown));
            if (returnTimer >= 1)
            {
                returningToPlayer = false;
                returnTimer = 0;
                transform.parent = player.transform;
            }
        }
        else
           velocity = FlyToTarget(player.transform.position + player.aimDirection);

    }


    private Vector3 FlyToTarget(Vector3 target)
    {
        distanceFromTarget = target - transform.position;
        distanceFromTarget.z = 0;
        float scalar = 1 - Mathf.Clamp(1 - Mathf.Abs(distanceFromTarget.magnitude) / maxDistance, -1, 1);
        scalar = scalar * scalar;
        //scalar = curve.Evaluate(scalar);
        return distanceFromTarget.normalized * maxSpeed * scalar;
        
    }

    public void Respawn(bool crying = false)
    {
        transform.position = respawnPos;
        if (crying)
        {
            myAnimator.SetBool("Crying", true);
            velocity = Vector3.zero;
            canShoot = false;
        }
    }
    public void SetGrayScale(float gs)
    {
        StopCoroutine("LerpGrayScale");
        StartCoroutine(LerpGrayScale(gs));
    }
    IEnumerator LerpGrayScale(float gs, float time = 0.2f)
    {
        float timer = 0;

        while (timer < time)
        {
            grayScale = Mathf.Lerp(grayScale, gs, timer/ time);
            myMaterial.SetFloat("_GrayScale", grayScale);
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
        }
        grayScale = gs;
        myMaterial.SetFloat("_GrayScale", grayScale);
    }

    public void Fire()
    {
        myAnimator.SetBool("Shooting", true);
        isFiring = true;
        transform.parent = null;
        firingPos = player.transform.position + player.aimDirection - Vector3.forward;
    }
}
