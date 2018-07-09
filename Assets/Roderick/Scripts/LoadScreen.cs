using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadScreen : MonoBehaviour {

    AsyncOperation ao;
    public int targetToLoad;
    public GameObject Visibility;
    public Text loadingText;
    public Slider progressBar;
    public GameObject player;
    public string whatsay;
    public Text TT;
    public List<GameObject> Buttons;
    public PauseManager PauseGame;
    public bool isLoading = false;

    private GameObject Music;

    private float counter;
    private bool Ftime = true;

    public void Load()
    {
        Visibility.SetActive(true);
        if (Buttons.Count >= 0)
        {
            foreach (var item in Buttons)
            {
                item.SetActive(false);
            }
        }
        if (PauseGame)
        {
            PauseGame.LoadScreenPause();
            player.SetActive(false);
        }
        Ftime = false;
        if (counter <= 10)
        {
            TT.text = TT.text + 0 + Math.Round(counter, 2);
        }
        else if (counter >= 10)
        {
            TT.text = TT.text + Math.Round(counter, 2);
        }
        isLoading = true;
        StartCoroutine(LoadLevelA());
    }

    public void Update()
    {
        if (Ftime == true) counter += Time.unscaledDeltaTime;
    }

    public void resetTimer()
    {
        counter = 0;
    }

    IEnumerator LoadLevelA()
    {
        yield return new WaitForSecondsRealtime(1);
        ao = SceneManager.LoadSceneAsync(targetToLoad);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            progressBar.value = ao.progress;
            if (ao.progress >= 0.9f)
            {
                progressBar.value = 1f;
                loadingText.text = "Press \"Space\" or \"A\" to continue.";
                if (Input.GetKeyDown("space") || Input.GetAxisRaw("Jump") == 1)
                {
                    Music = GameObject.FindGameObjectWithTag("Music");
                    Time.timeScale = 1f;
                    Destroy(Music);
                    ao.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }
}
