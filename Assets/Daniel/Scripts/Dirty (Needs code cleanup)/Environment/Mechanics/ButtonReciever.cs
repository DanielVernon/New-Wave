using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonReciever : MonoBehaviour {

    public UnityEvent onActivate = new UnityEvent();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FirstWaveHit()
    {
        if (onActivate != null)
        {
            onActivate.Invoke();
        }
    }
}
