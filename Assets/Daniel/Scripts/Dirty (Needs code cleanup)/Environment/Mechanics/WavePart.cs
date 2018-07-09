using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePart : MonoBehaviour {
    [HideInInspector]
    public Wave myWave = null;
    [HideInInspector]
    public PlayerController player;



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (myWave && collision.collider.CompareTag("Player"))
            myWave.SendMessage("CollideWithPlayer", collision.collider, SendMessageOptions.DontRequireReceiver);
    }

}
