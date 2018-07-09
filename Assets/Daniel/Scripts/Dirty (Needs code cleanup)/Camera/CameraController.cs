using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraController : MonoBehaviour {
    
    public PlayerController target = null;
    public movementSettings movement = new movementSettings();

    public float cameraSpeed = 20;
    private Vector3 centerOffset = Vector2.zero;
    private Vector3 distanceFromTarget = Vector3.zero;
    private List<FocusArea> allAreas = new List<FocusArea>();

    private bool isPaused = false;
    // Use this for initialization
    void Start () {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (target == null)
            Destroy(this);
        centerOffset = transform.transform.position - target.transform.position;

        allAreas = FindObjectsOfType<FocusArea>().ToList();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        distanceFromTarget = (target.transform.position + centerOffset) - transform.position;
        Vector3 playerTargPos = target.transform.position + centerOffset;
        playerTargPos.z = 0;
        Vector3 totalPos = playerTargPos;
        float totalWeight = 1;
        foreach (var item in allAreas)
        {
            if(item.enabled)
            {
                float weight = item.SamplePosition(playerTargPos);
                totalPos += item.focusPoint.position * weight;
                totalWeight += weight;
            }
        }

        distanceFromTarget = (totalPos / totalWeight) - transform.position;
        distanceFromTarget.z = 0;
        
	}

    private void FixedUpdate()
    {
        if (isPaused)
            return;
        
        if (distanceFromTarget.y < 0 && !movement.dontMoveVertically)// Camera is above Player
        {
            float scalar = 1 - Mathf.Clamp(1 - Mathf.Abs(distanceFromTarget.y) / movement.maxUpwardDistance, -1, 1);

            scalar = scalar * scalar;// * positive;
            transform.position -= new Vector3(0, scalar * cameraSpeed * Time.fixedDeltaTime, 0);
        }
        else if(!movement.dontMoveVertically)// Camera is below Player
        {
            float scalar = 1 - Mathf.Clamp(1 - Mathf.Abs(distanceFromTarget.y) / movement.maxDownwardDistance, -1, 1);

            scalar = scalar * scalar;// * positive;
            transform.position += new Vector3(0, scalar * cameraSpeed * Time.fixedDeltaTime, 0);
        }
        if (distanceFromTarget.x != 0)
        {
            float positive = 1;
            if (distanceFromTarget.x < 0)
                positive = -1;

            float scalar = positive - Mathf.Clamp(positive - distanceFromTarget.x / movement.maxHorizontalDistance, -1, 1);

            scalar = scalar * scalar;// * positive;
            transform.position += new Vector3(positive * scalar * cameraSpeed * Time.fixedDeltaTime, 0, 0);
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }


    [System.Serializable]
    public class movementSettings
    {
        public float maxHorizontalDistance = 3.0f;
        public bool dontMoveVertically = false;
        public float maxUpwardDistance = 2.0f;
        public float maxDownwardDistance = 15.0f;
    }

    private void Pause(bool pause)
    {
        isPaused = pause;
    }
}
