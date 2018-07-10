using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FanTest1 : MonoBehaviour {

    public float width = 2;
    public float length = 5;
    public float strength = 5;
    public List<Rigidbody2D> objectsInFan = new List<Rigidbody2D>();


    private BoxCollider2D boxCollider = null;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        objectsInFan.Add(collision.gameObject.GetComponent<Rigidbody2D>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        objectsInFan.Remove(collision.gameObject.GetComponent<Rigidbody2D>());
    }

    private void OnValidate()
    {
        boxCollider = gameObject.GetComponent<BoxCollider2D>();
        boxCollider.offset = new Vector2(0, length / 2);
        boxCollider.size = new Vector2(width / transform.lossyScale.x, length / transform.lossyScale.y);
    }

    // Update is called once per frame
    void Update () {
        for (int i = 0; i < objectsInFan.Count; i++)
        {
            objectsInFan[i].velocity = new Vector2(objectsInFan[i].velocity.x, Mathf.Max(objectsInFan[i].velocity.y, strength));
            //objectsInFan[i].AddForce(transform.up * strength, ForceMode2D.Force);
        }
	}

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.right * width / 2);
        Gizmos.DrawLine(transform.position, transform.position + transform.up * length);
    }
}
