using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(ShootWave))]
public class Turret : MonoBehaviour {

    public float shootInterval = 2;
    public float shotLength = 3;

    public Color preFireColor = Color.red;

    private float preFireTime = 0.2f;
    private float timeIsCharged = 0.4f;
    private ShootWave shootWave;
    private SpriteRenderer mySprite;
    private Color originalColor;
    private float timer = 0;
    private float colorTimer = 1;
    private bool chargingShot = true;
    private bool isPaused = false;


	// Use this for initialization
	void Start () {
        shootWave = gameObject.GetComponent<ShootWave>();
        timer = shootInterval;
        mySprite = gameObject.GetComponentInChildren<SpriteRenderer>();
        originalColor = mySprite.color;

    }
	
	// Update is called once per frame
	void Update () {
        if (isPaused)
            return;

        timer += Time.deltaTime;

        if (timer >= shootInterval - preFireTime - timeIsCharged)
        {
            colorTimer += Time.deltaTime / preFireTime;
            mySprite.color = Color.Lerp(originalColor, preFireColor, colorTimer);
            if (colorTimer >= 1)
                chargingShot = false;
        }
        else if (!chargingShot)
        {
            colorTimer -= Time.deltaTime / preFireTime;
            mySprite.color = Color.Lerp(originalColor, preFireColor, colorTimer);
            if (colorTimer < 0)
                chargingShot = true;
        }


        if (timer >= shootInterval)
        {
            Shoot();

            chargingShot = false;
            timer -= shootInterval;
        }
	}

    public void Shoot()
    {
        shootWave.Shoot(transform.position, shotLength, transform.right);
    }

    private void Pause(bool pause)
    {
        isPaused = pause;
    }
}
