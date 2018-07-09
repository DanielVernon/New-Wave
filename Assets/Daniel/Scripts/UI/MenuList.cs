using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Linq;

public enum UIType
{
    Horizontal,
    Vertical
}
public class MenuList : MonoBehaviour
{
    public UIType UIdirection = UIType.Horizontal;
    public GameObject Pointer;
    
    private bool paused;

    [Tooltip("must be ordered left-right / Up-Down")]
    public List<GUIElement> UIElements = new List<GUIElement>();
    public int enumerator = 0;


    private void Awake()
    {
        paused = false;
        //Player = GameObject.FindGameObjectWithTag("Player");

        for (int i = 0; i < UIElements.Count; i++)
        {
            GameObject obj = UIElements[i].obj;
            if (obj.GetComponent<Slider>())
            {
                UIElements[i].type = ElementType.Slider;
            }
            else if(obj.GetComponent<Button>())
            {
                UIElements[i].type = ElementType.Button;
            }
            else if (obj.GetComponent<Toggle>())
            {
                UIElements[i].type = ElementType.Toggle;
            }
        }

       // UIElements.OrderBy((emp1, emp2) >= (emp1.obj.transform.position.y == emp2.obj.transform.position.y));
    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            TriggerLeftRightInput(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            TriggerLeftRightInput(1);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleOffOn();
        }
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
            TriggerUpDownInput(-1);
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)))
            TriggerUpDownInput(1);

        //Scroll
        if(Pointer != null)
        Pointer.transform.position = new Vector3(Pointer.transform.position.x, UIElements[enumerator].obj.transform.position.y);
    }

    public void Enumerate(int enumerationAmount)
    {
        enumerator += enumerationAmount;
        while (enumerator < 0)
            enumerator += UIElements.Count;
        while (enumerator >= UIElements.Count)
            enumerator -= UIElements.Count;
        if (UIElements[enumerator].type == ElementType.none)
            return;

        if (UIElements[enumerator].type == ElementType.Slider)
        {
            Slider slider = UIElements[enumerator].obj.GetComponent<Slider>();
            slider.Select();
        }
        else if (UIElements[enumerator].type == ElementType.Toggle)
        {
            Toggle toggle = UIElements[enumerator].obj.GetComponent<Toggle>();
            toggle.Select();
        }
        if (UIElements[enumerator].type == ElementType.Button)
        {
            Button button = UIElements[enumerator].obj.GetComponent<Button>();
            button.Select();
        }
    }

    /// <summary>
    /// Set left/ right input (gets clamped between -1 and 1)
    /// </summary>
    /// <param name="right"> input in the right direction (negative is left)</param>
    public void TriggerLeftRightInput(float right)
    {
        switch (UIdirection)
        {
            case UIType.Horizontal:
                {
                    Enumerate((int)right);
                }
                break;
            case UIType.Vertical:
                {
                    if (UIElements[enumerator].type == ElementType.none)
                        return;

                    if (UIElements[enumerator].type == ElementType.Slider)
                    {
                        Slider slider = UIElements[enumerator].obj.GetComponent<Slider>();
                        slider.value = Mathf.Clamp(slider.value + right/* * UIElements[enumerator].sliderSensitivity*/, slider.minValue, slider.maxValue);
                    }
                    else if (UIElements[enumerator].type == ElementType.Toggle)
                    {
                        Toggle toggle = UIElements[enumerator].obj.GetComponent<Toggle>();
                        toggle.isOn = !toggle.isOn;
                    }
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Set up/ down input (gets clamped between -1 and 1)
    /// </summary>
    /// <param name="down"> input in the down direction (negative is up)</param>
    public void TriggerUpDownInput(float down)
    {
        switch (UIdirection)
        {
            case UIType.Horizontal:
                {
                    if (UIElements[enumerator].type == ElementType.none)
                        return;

                    if (UIElements[enumerator].type == ElementType.Slider)
                    {
                        Slider slider = UIElements[enumerator].obj.GetComponent<Slider>();
                        slider.value = Mathf.Clamp(slider.value + -down/* * UIElements[enumerator].sliderSensitivity*/, slider.minValue, slider.maxValue);
                    }
                    else if (UIElements[enumerator].type == ElementType.Toggle)
                    {
                        Toggle toggle = UIElements[enumerator].obj.GetComponent<Toggle>();
                        toggle.isOn = !toggle.isOn;
                    }
                }
                break;
            case UIType.Vertical:
                {
                    Enumerate((int)down);
                }
                break;
            default:
                break;
        }
    }
    
    public void ToggleOffOn()
    {
        if (UIElements[enumerator].type == ElementType.none)
            return;

        if (UIElements[enumerator].type == ElementType.Button)
        {
            Button button = UIElements[enumerator].obj.GetComponent<Button>();
            button.onClick.Invoke();
        }
        else if (UIElements[enumerator].type == ElementType.Toggle)
        {
            Toggle toggle = UIElements[enumerator].obj.GetComponent<Toggle>();
            toggle.isOn = !toggle.isOn;
            toggle.onValueChanged.Invoke(toggle.isOn);
        }
    }

    [System.Serializable]
    public class GUIElement
    {
        public GameObject obj;
        [HideInInspector]
        public ElementType type = ElementType.none;

        //[HideInInspector]
        public float sliderSensitivity = 1.0f;
        
    }
    public enum ElementType
    {
        Slider,
        Button,
        Toggle,
        none
    }

    public void Deactivate(bool keepPointer = false)
    {
        gameObject.SetActive(false);

        if (!keepPointer)
            enumerator = 0;
    }
}