using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
/// <summary>
/// What this script is meant to be managing
/// - pausing game
/// - respawning player
/// - keeping track of highscores and stats
/// - saving and loading
/// </summary>

public class GameManager : MonoBehaviour {
    public Tilemap mainTileMap;
    static GameManager manager = new GameManager();
    public GameManager Manager
    {
        get
        {
            if (!manager)
            {
                manager = new GameObject("GameManager").AddComponent<GameManager>();
            }
            return manager;
        }
    }
    
	// Use this for initialization
	void Awake () {
        if (manager != null)
        {
            //gameObject.GetComponent<MenuManager>() = manager;
            Destroy(gameObject);
        }
        else
        {
            manager = this;
        }
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
