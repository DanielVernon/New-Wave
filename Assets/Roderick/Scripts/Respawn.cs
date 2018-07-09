using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour {

    float timer = 0;
    private PauseManager pauseManager;
    [HideInInspector]
    public List<OnRespawn> respawns = new List<OnRespawn>();
    PlayerController playerController;
    CameraController mainCamera;
    private void Start()
    {
        pauseManager = GameObject.Find("PauseMenuCanvas").GetComponent<PauseManager>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        mainCamera = Camera.main.GetComponent<CameraController>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            //playerController.myRbody.velocity = Vector2.zero;
            playerController.enabled = false;
            //playerController.myAnimator.enabled = false;
            playerController.myAnimator.Play("Death");
            StartCoroutine(PlayerRespawn());

        }
    }

    IEnumerator PlayerRespawn()
    {
        
        yield return new WaitForSeconds(0.8f);
        for (int i = 0; i < respawns.Count; i++)
        {
            respawns[i].onRespawn.Invoke();
        }
        playerController.myRbody.velocity = Vector2.zero;
        playerController.myRbody.velocity = Vector3.zero;

        
        Vector3 camPos = mainCamera.transform.position;
        camPos.z = 0;
        float time = 0;
        while (Vector3.Distance(playerController.transform.position, camPos) > 6 && time < 5)
        {
            yield return new WaitForSeconds(0.1f);
            time += 0.1f;
            camPos = mainCamera.transform.position;
            camPos.z = 0;
        }

        //float time = Vector3.Distance(playerController.transform.position, camPos) / mainCamera.cameraSpeed;
        //yield return new WaitForSeconds(time);

        playerController.lastFrameVelocity = Vector2.zero;
        playerController.myRbody.velocity = Vector3.zero;
        pauseManager.Unpause();
        //string scene = SceneManager.GetActiveScene().name;
        //SceneManager.LoadScene(scene);
    }
}
