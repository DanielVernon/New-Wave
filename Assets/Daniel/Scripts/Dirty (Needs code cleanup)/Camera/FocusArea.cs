using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusArea : MonoBehaviour {

    public enum FocusType
    {
        Circular,
        XPos,
        Box,
    }

    public FocusType focusType = FocusType.Circular;
    public AnimationCurve focusFalloff = AnimationCurve.EaseInOut(0,0,1,1);//new AnimationCurve(new Keyframe[] {new Keyframe(0,0), new Keyframe(1.0f, 1.0f, 5, 0)});
    //[Range(0,1)]
    public float focusPercent = 0.5f;
    [Tooltip("If not set will use this Transform")]
    public Transform focusPoint;
    public Circular circular = new Circular();
    public Box box = new Box();
    public Xpos xPos = new Xpos();



    // Use this for initialization
    void Start () {
        if (!focusPoint)
            focusPoint = transform;
        focusPoint.position = new Vector3(focusPoint.position.x, focusPoint.position.y, 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public float SamplePosition(Vector3 samplePos)
    {
        samplePos.z = 0;
        float returnVal = 0;

        switch (focusType)
        {
            case FocusType.Circular:
                returnVal = SampleCircular(samplePos);
                break;
            case FocusType.XPos:
                returnVal = SampleXPos(samplePos);
                break;
            case FocusType.Box:
                returnVal = SampleBox(samplePos);
                break;
            default:
                break;
        }

        return returnVal;
    }


    private float SampleCircular(Vector3 samplePos)
    {
        float distPercent = Vector3.Distance(focusPoint.position, samplePos);
        distPercent = 1 - ((distPercent - circular.innerRadius) / (circular.outerRadius - circular.innerRadius));
        distPercent = Mathf.Clamp01(distPercent);

        //return Vector3.Lerp(samplePos, focusPoint.position, distPercent * focusPercent);
        return focusFalloff.Evaluate(distPercent) * focusPercent;
    }

    private float SampleXPos(Vector3 samplePos)
    {
        //Left Side 0-1
        float totalPercent = Mathf.Clamp01(1 - ((samplePos.x - (focusPoint.position.x + xPos.innerLeft)) / ((focusPoint.position.x + xPos.outerLeft) - (focusPoint.position.x + xPos.innerLeft))));
        //Right Side 0-1
        totalPercent *= Mathf.Clamp01(1 - ((samplePos.x - (focusPoint.position.x + xPos.innerRight)) / ((focusPoint.position.x + xPos.outerRight) - (focusPoint.position.x + xPos.innerRight))));


        //return Vector3.Lerp(samplePos, focusPoint.position, totalPercent * focusPercent);
        return focusFalloff.Evaluate(totalPercent) * focusPercent;
    }

    private float SampleBox(Vector3 samplePos)
    {
        //Left Side 0-1
        float totalPercent = Mathf.Clamp01(1 - ((samplePos.x - (focusPoint.position.x + box.innerLeft)) / ((focusPoint.position.x + box.outerLeft) - (focusPoint.position.x + box.innerLeft))));
        //Right Side 0-1
        totalPercent *= Mathf.Clamp01(1 - ((samplePos.x - (focusPoint.position.x + box.innerRight)) / ((focusPoint.position.x + box.outerRight) - (focusPoint.position.x + box.innerRight))));
        //Bottom Side 0-1
        totalPercent *= Mathf.Clamp01(1 - ((samplePos.y - (focusPoint.position.y + box.innerDown)) / ((focusPoint.position.y + box.outerDown) - (focusPoint.position.y + box.innerDown))));
        //Top Side 0-1
        totalPercent *= Mathf.Clamp01(1 - ((samplePos.y - (focusPoint.position.y + box.innerUp)) / ((focusPoint.position.y + box.outerUp) - (focusPoint.position.y + box.innerUp))));

        //return Vector3.Lerp(samplePos, focusPoint.position, totalPercent * focusPercent);
        return focusFalloff.Evaluate(totalPercent) * focusPercent;
    }


    [System.Serializable]
    public class Circular
    {
        public float outerRadius = 10;
        public float innerRadius = 0;
    }

    [System.Serializable]
    public class Xpos
    {
        [Range(-30, 0)]
        public float innerLeft = -10;
        [Range(-30, 0)]
        public float outerLeft = -20;
        [Space(10)]
        [Range(0, 30)]
        public float innerRight = 10;
        [Range(0, 30)]
        public float outerRight = 20;
    }

    [System.Serializable]
    public class Box
    {
        [Range(-30, 0)]
        public float innerLeft = -10;
        [Range(-30, 0)]
        public float outerLeft = -20;
        [Space(10)]
        [Range(0, 30)]
        public float innerRight = 10;
        [Range(0, 30)]
        public float outerRight = 20;
        [Space(10)]
        [Range(0, 20)]
        public float innerUp = 10;
        [Range(0, 20)]
        public float outerUp = 15;
        [Space(10)]
        [Range(-20, 0)]
        public float innerDown = -10;
        [Range(-20, 0)]
        public float outerDown = -15;
    }

    private void OnDrawGizmos()
    {
        switch (focusType)
        {
            case FocusType.Circular:
                DrawCircularGizmo();
                break;
            case FocusType.XPos:
                DrawXposGizmo();
                break;
            case FocusType.Box:
                DrawBoxGizmo();
                break;
            default:
                break;
        }
    }

    void DrawCircularGizmo()
    {
        Vector3 focusPos = transform.position;
        if (focusPoint)
            focusPos = focusPoint.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(focusPos, circular.innerRadius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(focusPos, circular.outerRadius);
    }

    void DrawXposGizmo()
    {
        Vector3 focusPos = transform.position;
        if (focusPoint)
            focusPos = focusPoint.position;

        Gizmos.color = Color.red;
        Vector3 LinePos = focusPos + Vector3.right * xPos.innerLeft;
        Gizmos.DrawLine(LinePos + Vector3.down * 10, LinePos + Vector3.up * 10);
        LinePos = focusPos + Vector3.right * xPos.innerRight;
        Gizmos.DrawLine(LinePos + Vector3.down * 10, LinePos + Vector3.up * 10);

        Gizmos.color = Color.white;
        LinePos = focusPos + Vector3.right * xPos.outerLeft;
        Gizmos.DrawLine(LinePos + Vector3.down * 10, LinePos + Vector3.up * 10);
        LinePos = focusPos + Vector3.right * xPos.outerRight;
        Gizmos.DrawLine(LinePos + Vector3.down * 10, LinePos + Vector3.up * 10);
    }

    void DrawBoxGizmo()
    {
        Vector3 focusPos = transform.position;
        if (focusPoint)
            focusPos = focusPoint.position;

        Vector3 outerScale = Vector3.zero;
        outerScale.x = Mathf.Abs(box.outerLeft) + Mathf.Abs(box.outerRight);
        outerScale.y = Mathf.Abs(box.outerUp) + Mathf.Abs(box.outerDown);
        Vector3 outerCenter = new Vector3(focusPos.x + (box.outerLeft + box.outerRight)/2, focusPos.y + (box.outerUp + box.outerDown)/2, 0);


        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(outerCenter, outerScale);

        Vector3 innerScale = Vector3.zero;
        innerScale.x = Mathf.Abs(box.innerLeft) + Mathf.Abs(box.innerRight);
        innerScale.y = Mathf.Abs(box.innerUp) + Mathf.Abs(box.innerDown);

        Vector3 innerCenter = new Vector3(focusPos.x + (box.innerLeft + box.innerRight) / 2, focusPos.y + (box.innerUp + box.innerDown) / 2, 0);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(innerCenter, innerScale);
    }

}
