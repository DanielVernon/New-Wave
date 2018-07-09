using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class InteractWithController : MonoBehaviour {
    public string overrideString = "";

    private InteractableObject intObj;
	// Use this for initialization
	void Start () {
        intObj = gameObject.GetComponent<InteractableObject>();
	}
	
	// Update is called once per frame
	void Update () {
        if (intObj.canInteract && Input.GetButtonDown(overrideString))
        {
            intObj.onInteract.Invoke();
        }
	}
}
