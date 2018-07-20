using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {
    [Tooltip("Use this if the scene starts with a menu, otherwise leave null")]
    public MenuLayer startingMenu = null;

    private List<MenuLayer> menuStack = new List<MenuLayer>();
    

	// Use this for initialization  
	void Start () {
        if (startingMenu)
        {
            AddToStack(startingMenu);
        }
	}
	
	// Update is called once per frame
	void Update () {

	}

    #region Functions for the stack

    public void ClearStack()
    {
        menuStack = new List<MenuLayer>();
    }

    public void SetStack(List<MenuLayer> layers)
    {
        menuStack = layers;
    }

    public void AddToStack(MenuLayer layer)
    {
        menuStack.Add(layer);
    }

    public void AddToStack(List<MenuLayer> layers)
    {
        menuStack.AddRange(layers);
    }


    public void Back(int Amount = 1)
    {
        while (Amount > 0 && menuStack.Count > 0)
        {
            Amount--;
            menuStack[menuStack.Count - 1].Deactivate();
            menuStack.RemoveAt(menuStack.Count - 1);
        }
    }
    #endregion
}
