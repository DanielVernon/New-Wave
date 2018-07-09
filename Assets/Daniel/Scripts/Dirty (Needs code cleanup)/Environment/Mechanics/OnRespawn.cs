using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnRespawn : MonoBehaviour {
    public UnityEvent onRespawn;

    private void Start()
    {
        Respawn respawnScript = FindObjectOfType<Respawn>();
        if (respawnScript)
        {
            respawnScript.respawns.Add(this);
        }
        else
            Destroy(this);
    }
}
