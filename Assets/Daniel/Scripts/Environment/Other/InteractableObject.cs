using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractableObject : MonoBehaviour {
    public KeyCode interactButton = KeyCode.None;
    public UnityEvent onInteract;

    private BoxCollider2D myCollider;
    [HideInInspector]
    public bool canInteract = false;

    private void Start()
    {
        myCollider = gameObject.GetComponent<BoxCollider2D>();
        myCollider.isTrigger = true;
    }

    // Update is called once per frame
    void Update () {
        if (canInteract && Input.GetKeyDown(interactButton))
        {
            onInteract.Invoke();
        }
	}


    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            canInteract = true;
        }
    }

    private void OnTriggerStay2D(Collider2D coll)
    {
        OnTriggerEnter2D(coll);
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            canInteract = false;
        }
    }
}
