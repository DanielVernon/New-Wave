using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// Things that aren't done
/// Need to call disable function on all UIElements when popup appears
/// need to create MenuManager scripts to keep track of layers
/// </summary>
[RequireComponent(typeof(RectTransform))]
[HelpURL("https://en.wikipedia.org/wiki/Doxygen")]
public class MenuLayer : MonoBehaviour {
    public LayerType layerType;


    // Set interactable. This triggers UIElements to be clickable/ not clickable
    public bool Interactable
    {
        get
        {
            return interactable;
        }
        set
        {
            if (interactable != value)
            {
                if (value) // Enabling interactions in layer
                {

                }
                else // Disabling interactions in layer
                {

                }
            }
            interactable = value;
        }
    }
    public List<Transform> GetChildren(Transform @object)
    {
        List<Transform> objects = new List<Transform>();
        for (int i = 0; i < @object.transform.childCount; i++)
        {
            objects.Add(@object.GetChild(i));
        }
        return objects;
    }

    public List<GameObject> GetChildren(GameObject @object)
    {
        List<GameObject> objects = new List<GameObject>();
        for (int i = 0; i < @object.transform.childCount; i++)
        {
            objects.Add(@object.transform.GetChild(i).gameObject);
        }
        return objects;
    }


    public List<UIElement> interactables = new List<UIElement>();

    void RefreshInteractables(GameObject target = null, bool includeChildren = true)
    {
        // Start at root object if haven't started yet
        if (!target || target.GetInstanceID() == gameObject.GetInstanceID())
        {
            target = gameObject;
            interactables = new List<UIElement>();
        }
        List<GameObject> viableChildren = GetChildren(target);
        for (int i = 0; i < viableChildren.Count; i++)
        {
            MenuLayer menuLayer = viableChildren[i].GetComponent<MenuLayer>();
            if (!menuLayer)
            {
                Slider slider = viableChildren[i].GetComponent<Slider>();
                Button button = viableChildren[i].GetComponent<Button>();
                Toggle toggle = viableChildren[i].GetComponent<Toggle>();
                if (slider)
                {
                    UIElement element = new UIElement()
                    {
                        obj = viableChildren[i],
                        elementType = ElementType.Slider
                    };
                    interactables.Add(element);
                }
                else if (button)
                {
                    UIElement element = new UIElement()
                    {
                        obj = viableChildren[i],
                        elementType = ElementType.Button
                    };
                    interactables.Add(element);
                }
                else if (toggle)
                {
                    UIElement element = new UIElement()
                    {
                        obj = viableChildren[i],
                        elementType = ElementType.Toggle
                    };
                    interactables.Add(element);
                }
            }
            else
            {
                viableChildren.RemoveAt(i);
                i--;
            }
            if (viableChildren[i].transform.childCount <= 0)
            {
                viableChildren.RemoveAt(i);
                i--;
            }
        }

        if (includeChildren)
        {
            for (int i = 0; i < viableChildren.Count; i++)
            {
                RefreshInteractables(viableChildren[i].gameObject);
            }
        }
    }

    private void OnValidate()
    {
        RefreshInteractables();
    }
    private bool interactable = false;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}

    public void Activate()
    {
        Activate(layerType);
    }
    public void Activate(LayerType type)
    {
        switch (type)
        {
            case LayerType.layer:
            { // Override previous Layer, previous layer reset?
                
                break;
            }
            case LayerType.Tab:
            { // previous layer not reset
                
                break;
            }
            case LayerType.Popup:
            { // appears over other layers, doesn't
                    for (int i = 0; i < interactables.Count; i++)
                    {
                        interactables[i].obj.active = true;
                    }
                break;
            }
            default:
                break;
        }
    }

    public void Deactivate()
    {

    }

    [System.Serializable]
    public class UIElement
    {
        public GameObject obj;
        public ElementType elementType = ElementType.none;
        public void SetInteractable(bool interactable)
        {
            switch (elementType)
            {
                case ElementType.Slider:
                    obj.GetComponent<Slider>().interactable = interactable;
                    break;
                case ElementType.Button:
                    obj.GetComponent<Button>().interactable = interactable;
                    break;
                case ElementType.Toggle:
                    obj.GetComponent<Slider>().interactable = interactable;
                    break;
                case ElementType.none:
                    break;
                default:
                    break;
            }
        }
    }

    public enum ElementType
    {
        Slider,
        Button,
        Toggle,
        none
    }

    public enum LayerType
    {
        layer, // Base Layer, represents a whole screen
        Tab, // Tab, usually multiple are active within a layer
        Popup, // Pops up ontop of whatever is currently visible, background is blurred/ faded
    }
}