using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class PauseManager : MonoBehaviour {

    public AudioMixer Master;
    public Slider MasterSlider;
    public Slider MusicSlider;
    public Slider SFXSlider;

    public GameObject Display;
    public GameObject MenuCheck;
    public GameObject Pointer;
    public GameObject Pointer2;
    public LoadScreen loadScreen;
    [Tooltip("Pointer dummies")]
    public GameObject Resume;
    [Tooltip("Pointer dummies")]
    public GameObject MainMenu;
    [Tooltip("Pointer dummies")]
    public GameObject Yes;
    [Tooltip("Pointer dummies")]
    public GameObject No;

    public AudioMixerSnapshot Paused;
    public AudioMixerSnapshot UnPaused;

    private GameObject Player;
    private GameObject Music;

    private bool paused;
    private bool check;

    private int tracker;
 

    private void Awake()
    {
        paused = false;
        float a = 0;
        float b = 0;
        float c = 0;
        Master.GetFloat("MasterSound", out a);
        Master.GetFloat("MusicSound", out b);
        Master.GetFloat("SFXSound", out c);
        MasterSlider.value = a;
        MusicSlider.value = b;
        SFXSlider.value = c;
        loadScreen = GameObject.FindObjectOfType<LoadScreen>();
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        tracker = 0;
        Display.SetActive(true);
        Paused.TransitionTo(.01f);
        Player.GetComponent<ShootWave>().enabled = false;
        Player.GetComponent<PlayerController>().enabled = false;
        Pointer.transform.position = new Vector3(Pointer.transform.position.x, MasterSlider.transform.position.y);
    }

    public void Unpause()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        Player.SetActive(true);
        Display.SetActive(false);
        UnPaused.TransitionTo(.01f);
        Player.GetComponent<ShootWave>().enabled = true;
        Player.GetComponent<PlayerController>().enabled = true;
    }

    public void LoadScreenPause()
    {
        Time.timeScale = 0f;
        gameObject.SetActive(false);
        Player.SetActive(false);
    }

    public void ConfirmCheckMainMenuLoad()
    {
        MenuCheck.SetActive(true);
        check = true;
        Pointer2.SetActive(true);
        Pointer.SetActive(false);
    }

    public void DenyCheck()
    {
        check = false;
        MenuCheck.SetActive(false);
        Pointer2.SetActive(false);
        Pointer.SetActive(true);
        tracker = 4;
    }

    public void LoadMainMenu()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 1f;
        Music = GameObject.FindGameObjectWithTag("Music");
        Destroy(Music);
        SceneManager.LoadScene(0);
    }
    private bool resetHorizontal = true;
    private float horizontalTimer = 0;
    private bool resetVertical = true;
    private float verticalTimer = 0;
    private void Update()
    {
        float horizontal = Input.GetAxis("DpadHorizontal");

        if (Mathf.Abs(horizontal) < 0.05f)
        {
            horizontalTimer = 0;
            resetHorizontal = true;
        }
        if (!resetHorizontal)
        {
            horizontalTimer += Time.unscaledDeltaTime;
            if (horizontalTimer > 0.3f)
            {
                horizontalTimer = 0;
                resetHorizontal = true;
            }
        }
        if ((Input.GetKeyDown(KeyCode.Escape) || (Input.GetButtonDown("Start"))) && !paused && !loadScreen.isLoading)
        {
            paused = true;
            Pause();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) || (Input.GetButtonDown("Start") || Input.GetButtonDown("controllerB")) && paused && !check)
        {
            paused = false;
            Unpause();
        }
        if (!paused)
            return;
        if (check)
        {


            if (Input.GetKeyDown("left") || (resetHorizontal && horizontal < -0.5f))
            {

                resetHorizontal = false;
                if (tracker == 8)
                {
                    tracker++;
                    Pointer2.transform.position = new Vector3(No.transform.position.x, Pointer2.transform.position.y);
                }
                else if (tracker == 9)
                {
                    tracker--;
                    Pointer2.transform.position = new Vector3(Yes.transform.position.x, Pointer2.transform.position.y);
                }
            }
            if (Input.GetKeyDown("right") || (resetHorizontal && horizontal > 0.5f))
            {
                resetHorizontal = false;
                if (tracker == 8)
                {
                    tracker++;
                    Pointer2.transform.position = new Vector3(No.transform.position.x, Pointer2.transform.position.y);
                }
                else if (tracker == 9)
                {
                    tracker--;
                    Pointer2.transform.position = new Vector3(Yes.transform.position.x, Pointer2.transform.position.y);
                }
            }
            if (Input.GetKeyDown("space") || Input.GetButtonDown("controllerA"))
            {
                if (tracker == 8)
                    LoadMainMenu();
                else
                    DenyCheck();
            }
            if (Input.GetKeyDown("escape") || Input.GetButtonDown("controllerB"))// || Input.GetAxisRaw("Shoot") == 1)
            {
                DenyCheck();
            }
        }
        else if (!check)
        {
            float vertical = Input.GetAxis("DpadVertical");

            if (Mathf.Abs(vertical) < 0.05f)
            {
                verticalTimer = 0;
                resetVertical = true;
            }
            if (!resetHorizontal)
            {
                verticalTimer += Time.unscaledDeltaTime;
                if (verticalTimer > 0.3f)
                {
                    verticalTimer = 0;
                    resetVertical = true;
                }
            }

            if ((Input.GetKeyDown("s") || Input.GetKeyDown("down") || (resetVertical && vertical < -0.5f)) && paused)
            {
                resetVertical = false;
                tracker++;
                if (tracker == 0)
                    Pointer.transform.position = new Vector3(Pointer.transform.position.x, MasterSlider.transform.position.y);
                else if (tracker == 1)
                    Pointer.transform.position = new Vector3(Pointer.transform.position.x, MusicSlider.transform.position.y);
                else if (tracker == 2)
                    Pointer.transform.position = new Vector3(Pointer.transform.position.x, SFXSlider.transform.position.y);
                else if (tracker == 3)
                    Pointer.transform.position = new Vector3(Pointer.transform.position.x, Resume.transform.position.y);
                else if (tracker == 4)
                    Pointer.transform.position = new Vector3(Pointer.transform.position.x, MainMenu.transform.position.y);
                else if (tracker == 5)
                {
                    tracker = 0;
                    Pointer.transform.position = new Vector3(Pointer.transform.position.x, MasterSlider.transform.position.y);
                }
            }
            if ((Input.GetKeyDown("w") || Input.GetKeyDown("up") || (resetVertical && vertical > 0.5f)) && paused)
            {
                resetVertical = false;
                tracker--;
                if (tracker == 0)
                    Pointer.transform.position = new Vector3(Pointer.transform.position.x, MasterSlider.transform.position.y);
                else if (tracker == 1)
                    Pointer.transform.position = new Vector3(Pointer.transform.position.x, MusicSlider.transform.position.y);
                else if (tracker == 2)
                    Pointer.transform.position = new Vector3(Pointer.transform.position.x, SFXSlider.transform.position.y);
                else if (tracker == 3)
                    Pointer.transform.position = new Vector3(Pointer.transform.position.x, Resume.transform.position.y);
                else if (tracker == 4)
                    Pointer.transform.position = new Vector3(Pointer.transform.position.x, MainMenu.transform.position.y);
                else if (tracker == -1)
                {
                    tracker = 4;
                    Pointer.transform.position = new Vector3(Pointer.transform.position.x, MainMenu.transform.position.y);
                }
            }
            if ((Input.GetKeyDown("a") || Input.GetKeyDown("left") || (resetHorizontal && horizontal < -0.5f)) && paused)
            {
                if (tracker == 0)
                {
                    if (MasterSlider.value >= -80)
                        MasterSlider.value -= 5;
                }
                if (tracker == 1)
                {
                    if (MusicSlider.value >= -80)
                        MusicSlider.value -= 5;
                }
                if (tracker == 2)
                {
                    if (SFXSlider.value >= -80)
                        SFXSlider.value -= 5;
                }
            }
            if ((Input.GetKeyDown("d") || Input.GetKeyDown("right") || (resetHorizontal && horizontal > 0.5f)) && paused)
            {
                if (tracker == 0)
                {
                    if (MasterSlider.value <= 20)
                        MasterSlider.value += 5;
                }
                if (tracker == 1)
                {
                    if (MusicSlider.value <= 20)
                        MusicSlider.value += 5;
                }
                if (tracker == 2)
                {
                    if (SFXSlider.value <= 20)
                        SFXSlider.value += 5;
                }
            }
            if (Input.GetKeyDown("space") || Input.GetButtonDown("controllerA"))
            {
                if (tracker == 3)
                {
                    paused = false;
                    Unpause();
                }
                if (tracker == 4)
                {
                    tracker = 9;
                    ConfirmCheckMainMenuLoad();
                }
            }
        }
        if (paused)
        {
            Master.SetFloat("MasterSound", MasterSlider.value);
            Master.SetFloat("MusicSound", MusicSlider.value);
            Master.SetFloat("SFXSound", SFXSlider.value);
        }
    }
}
