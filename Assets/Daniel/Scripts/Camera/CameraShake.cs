using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    [Range(0,3)]
    public float deflationTime = 1;
    public float shakeValue = 0;
    public float speed = 25;

    public float maxAngle = 10;
    [Header("2D Only")]
    public bool is2D = false;

    
    public float maxOffset = 2;

    private Vector3 offsetPosition = Vector3.zero;
    private Vector3 offsetRotation = Vector3.zero;
    private float perlinTime = 0;
    private bool timeIncrease = true;

    public void SetShake(float f)
    {
        shakeValue = f;
        shakeValue = Mathf.Clamp01(shakeValue);
    }

    public void AddShake(float f)
    {
        shakeValue += f;
        shakeValue = Mathf.Clamp01(shakeValue);
    }
	
    float GetRandomFloatNegOneToOne()
    {
        return Random.Range(-1.0f, 1.0f);
    }

    float GetPerlinFloatNegOneToOne(float x, float y)
    {
        return Mathf.Clamp01(Mathf.PerlinNoise(x, y)) * 2 - 1;
    }

	// Update is called once per frame
	void Update () {
        //Unshake camera
        transform.position -= offsetPosition;
        transform.eulerAngles -= offsetRotation;


        if (shakeValue > 0)
        {
            shakeValue -= Time.deltaTime / deflationTime;
            shakeValue = Mathf.Clamp01(shakeValue);
            float shake = shakeValue * shakeValue;

            if (timeIncrease)
                perlinTime += Time.deltaTime * speed;
            else
                perlinTime -= Time.deltaTime * speed;

            //if (perlinTime > 10)
            //{
            //    perlinTime = 11 - perlinTime;
            //    timeIncrease = false;
            //}
            //if (perlinTime < 0)
            //{
            //    perlinTime = -perlinTime;
            //    timeIncrease = true;
            //}

            if (is2D)//Do 2D
            {
                offsetRotation = new Vector3(0,0, maxAngle * shake * GetPerlinFloatNegOneToOne(1, perlinTime));
                offsetPosition = new Vector3(maxOffset * shake * GetPerlinFloatNegOneToOne(2, perlinTime), maxOffset * shake * GetPerlinFloatNegOneToOne(3, perlinTime), 0);
            }
            else//Do 3D
            {
                offsetRotation = new Vector3(maxAngle * shake * GetPerlinFloatNegOneToOne(1, perlinTime), maxAngle * shake * GetPerlinFloatNegOneToOne(2, perlinTime), maxAngle * shake * GetPerlinFloatNegOneToOne(3, perlinTime));
                offsetPosition = Vector3.zero;
            }
            transform.position += offsetPosition;
            transform.eulerAngles += offsetRotation;
        }
        else
        {
            offsetRotation = Vector3.zero;
            offsetPosition = Vector3.zero;
        }
    }
}
