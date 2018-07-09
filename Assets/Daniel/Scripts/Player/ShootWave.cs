using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootWave : MonoBehaviour
{
    public GameObject wave;
    private Collider2D playerCollider;
    private PlayerController playerController;

    public float speed = 7;
    public float despawnTime = 60;
    
    // Use this for initialization
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerCollider = playerController.gameObject.GetComponent<Collider2D>();
    }

    public void Shoot(Vector3 pos, float Length, Vector3 dir, float enableCollisonsTime = 0)
    {
        GameObject newWave = Instantiate(wave, new Vector3(0, 1000, 0), Quaternion.identity);
        if (playerController)
        {
            playerController.itemsICantCollideWith.Add(newWave.GetComponent<Collider2D>());
            Physics2D.IgnoreCollision(playerCollider, newWave.GetComponent<Collider2D>(), true);
        }
        

        newWave.transform.position = pos;
        Vector3 velocity = dir * speed;
        velocity.z = 0;
        Wave waveScript = newWave.GetComponent<Wave>();
        waveScript.velocity = velocity;
        newWave.transform.right = velocity;
        waveScript.maxLength = Length;
        waveScript.StartCoroutine("DespawnTimer", despawnTime);
        waveScript.StartCoroutine("EnableCollisionsTimer", enableCollisonsTime);
        waveScript.ignoringCollision = true;
        waveScript.InitializeTail();
    }
}
