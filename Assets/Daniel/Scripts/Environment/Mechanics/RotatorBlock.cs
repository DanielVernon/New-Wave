using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorBlock : MonoBehaviour {
    public bool active = false;
    [Header("Only while Active")]
    public float timeTwixRotation = 1;
    public float rotationSpeed = 5;
    public bool clockwise = true;


    private float waitTimer = 0;
    private bool rotating = false;

    public void Activate()//for send message
    {
        if (!rotating)
            StartCoroutine(Rotate());
    }

    private void Update()
    {
        if (active)
        {
            if (!rotating)
            {
                waitTimer += Time.deltaTime;
            }
            if (active && !rotating && waitTimer >= timeTwixRotation)
            {
                waitTimer = 0;
                StartCoroutine(Rotate());
            }
        }
    }

    IEnumerator Rotate()
    {
        rotating = true;
        int rotationFlip = 1;
        if (!clockwise)
            rotationFlip = -1;
        float timer = 0;
        while(rotating)
        {
            if (timer >= 90 / rotationSpeed)
            {

                transform.Rotate(Vector3.forward, rotationFlip * rotationSpeed * (90 / rotationSpeed - timer));
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.RoundToInt(transform.eulerAngles.z));
                rotating = false;
                waitTimer = 0;

            }
            else
                transform.Rotate(Vector3.forward, rotationFlip * rotationSpeed * Time.fixedDeltaTime);

            timer += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
    }
}
