using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {

    [HideInInspector]
    public Vector3 velocity; // Most current velocity
    public GameObject wavePartPrefab = null;

    private Collider2D thisCollider;
    private Collider2D playerCollider;

    private PlayerController playerController;

    private Animator animator;

    [HideInInspector]
    public float despawnAfterTime;
    [HideInInspector]
    public float enableCollisionsAfterTime;
    private bool isPaused = false;
    [HideInInspector]
    public bool ignoringCollision = false;
    [HideInInspector]
    public float maxLength = 3;
    private Vector3 reflectionWall = Vector3.zero;
    private List<TailSection> myTail = new List<TailSection>();

    private bool despawningAgainstWall = false;

    private void Awake()
    {
        thisCollider = GetComponent<Collider2D>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        playerController = playerCollider.GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        if (wavePartPrefab == null)
        {
            Debug.LogError("You need to set the \"wavePartPrefab\" variable on the Wave prefab");
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isPaused)
            return;

        
        if (reflectionWall.magnitude > 0.2f)
        {
            Reflect(reflectionWall);
            reflectionWall = Vector3.zero;
        }
        if (despawningAgainstWall)
        {
            maxLength -= velocity.magnitude * Time.deltaTime;
            if (maxLength <= 0)
            {
                DestroySelf();
            }
        }
        else
            transform.position += velocity * Time.deltaTime;//Move

        UpdateTail();
    }


    public void InitializeTail()
    {
        TailSection tailEnd = new TailSection
        {
            Pos = transform.position,
            direction = transform.right,
            distFromStart = 0
        };
        tailEnd.wavePart = Instantiate<GameObject>(wavePartPrefab);
        tailEnd.wavePart.GetComponent<WavePart>().myWave = this;
        tailEnd.wavePart.tag = "Projectile";
        tailEnd.wavePart.layer = LayerMask.NameToLayer("Projectile");
        tailEnd.wavePart.GetComponent<WavePart>().player = playerController;

        if (ignoringCollision)
        {
            playerController.itemsICantCollideWith.Add(tailEnd.wavePart.GetComponent<Collider2D>());
            Physics2D.IgnoreCollision(playerCollider, tailEnd.wavePart.GetComponent<Collider2D>(), true);
        }
        myTail.Add(tailEnd);
    }


    void UpdateTail()
    {
        if (myTail.Count == 1)
        {
            myTail[0].distFromStart = Vector3.Distance(myTail[0].Pos, transform.position);
            if (myTail[0].distFromStart > maxLength)
            {
                myTail[0].Pos -= myTail[0].direction * (maxLength - myTail[0].distFromStart);

            }
            GameObject wavePart = myTail[0].wavePart;
            wavePart.transform.position = (myTail[0].Pos + transform.position) / 2;
            wavePart.transform.localScale = new Vector3(Vector3.Distance(myTail[0].Pos, transform.position), transform.localScale.y, wavePart.transform.localScale.z);
            wavePart.transform.right = myTail[0].direction;
            return;
        }

        else if(myTail.Count < 2)
            return;

        for (int i = myTail.Count -1; i >= 0; i--)
        {
            if (i == myTail.Count-1)//closest to head
            {
                myTail[i].distFromStart = Vector3.Distance(myTail[i].Pos, transform.position);
                GameObject wavePart = myTail[i].wavePart;
                wavePart.transform.position = (myTail[i].Pos + transform.position) / 2;
                wavePart.transform.localScale = new Vector3(Vector3.Distance(myTail[i].Pos, transform.position), transform.localScale.y, wavePart.transform.localScale.z);
                wavePart.transform.right = myTail[i].direction;
            }
            else if (i == 0)//Tail
            {
                myTail[i].distFromStart = myTail[i+1].distFromStart + Vector3.Distance(myTail[i].Pos, myTail[i+1].Pos);
                if (myTail[i].distFromStart > maxLength)
                {
                    if (myTail[i+1].distFromStart < maxLength)
                    {
                        myTail[i].Pos = myTail[i+1].Pos + (-myTail[i].direction *  (maxLength - myTail[i+1].distFromStart));
                    }
                    else
                    {
                        float distLeft = myTail[i + 1].distFromStart - maxLength;
                        myTail[i].direction = myTail[i+1].direction;
                        myTail[i].Pos = myTail[i+1].Pos;
                        myTail[i].Pos += distLeft * myTail[i].direction;
                        Destroy(myTail[1].wavePart);
                        EnableCollisions();
                        myTail.RemoveAt(1);//Only okay because it is element 0
                    }
                }
                if (myTail.Count > 1)
                {
                    GameObject wavePart = myTail[i].wavePart;
                    wavePart.transform.position = (myTail[i].Pos + myTail[i + 1].Pos) / 2;
                    wavePart.transform.localScale = new Vector3(Vector3.Distance(myTail[i].Pos, myTail[i + 1].Pos), transform.localScale.y, wavePart.transform.localScale.z);
                    wavePart.transform.right = myTail[i].direction;
                }
                else
                {
                    GameObject wavePart = myTail[i].wavePart;
                    wavePart.transform.position = (myTail[i].Pos + transform.position) / 2;
                    wavePart.transform.localScale = new Vector3(Vector3.Distance(myTail[i].Pos, transform.position), transform.localScale.y, wavePart.transform.localScale.z);
                    wavePart.transform.right = myTail[i].direction;
                }

            }
            else
            {
                myTail[i].distFromStart = myTail[i+1].distFromStart + Vector3.Distance(myTail[i].Pos, myTail[i+1].Pos);

                GameObject wavePart = myTail[i].wavePart;
                wavePart.transform.position = (myTail[i].Pos + myTail[i+1].Pos) / 2;
                wavePart.transform.localScale = new Vector3(Vector3.Distance(myTail[i].Pos, myTail[i+1].Pos), transform.localScale.y, wavePart.transform.localScale.z);
                wavePart.transform.right = myTail[i].direction;
            }
        }
    }



    IEnumerator DespawnTimer(float time)
    {
        yield return new WaitForSeconds(time);
        DestroySelf();
    }

    IEnumerator EnableCollisionsTimer(float time)
    {
        yield return new WaitForSeconds(time);
        EnableCollisions();
    }

    public void EnableCollisions()
    {
        if (playerController)
        {
            ignoringCollision = false;
            Physics2D.IgnoreCollision(playerCollider, thisCollider, false);
            playerController.itemsICantCollideWith.Remove(thisCollider);
            foreach (var item in myTail)
            {
                Physics2D.IgnoreCollision(playerCollider, item.wavePart.GetComponent<Collider2D>(), false);
                playerController.itemsICantCollideWith.Remove(item.wavePart.GetComponent<Collider2D>());
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.isTrigger)
            return;
        Collider2D col;
        int layerMask = ~(1 << LayerMask.NameToLayer("Projectile"));
        RaycastHit2D hit;
        if (collision.collider.gameObject.CompareTag("Player"))
        {
            hit = Physics2D.Raycast(transform.position, (collision.collider.transform.position - transform.position).normalized, transform.lossyScale.magnitude / 2 + 0.1f, layerMask);
            col = playerCollider;
        }
        else
        {

            hit = Physics2D.Raycast(transform.position, velocity.normalized, transform.lossyScale.magnitude / 2 + 0.1f, layerMask);
            col = hit.collider;
        }

        if (hit && !hit.collider.isTrigger && hit.collider.IsTouching(thisCollider))//has hit
        {
            col.gameObject.SendMessage("FirstWaveHit", SendMessageOptions.DontRequireReceiver);

            if (col.gameObject.CompareTag("BounceableWall") && reflectionWall == Vector3.zero)
            {
                MultiTag wallTag = col.gameObject.GetComponent<MultiTag>();
                if (wallTag)//////////////////////////////////////////////////////////////////Need To Change to check for wallreflectalongX
                {
                    reflectionWall = col.transform.right;
                }
                else
                    reflectionWall = hit.normal;

                TailSection wallBounce = new TailSection();
                wallBounce.Pos = hit.point;
                wallBounce.distFromStart = Vector3.Distance(wallBounce.Pos, transform.position);
                wallBounce.wavePart = Instantiate<GameObject>(wavePartPrefab);
                wallBounce.wavePart.GetComponent<WavePart>().myWave = this;
                wallBounce.wavePart.GetComponent<WavePart>().player = playerController;
                wallBounce.wavePart.tag = "Projectile";
                wallBounce.wavePart.layer = LayerMask.NameToLayer("Projectile");
                myTail.Add(wallBounce);
            }
            else if (col.gameObject.CompareTag("Player"))
            {
                CollideWithPlayer(col);
            }
            
            else if (!col.CompareTag("Projectile") && !col.CompareTag("BounceableWall"))
            {
                //DestroySelf();
                despawningAgainstWall = true;
                //Destroy(gameObject);
            }
        }
    }

    void CollideWithPlayer(Collider2D col)
    {
        PlayerController playerController = col.gameObject.GetComponent<PlayerController>();
        if (playerController.itemsICantCollideWith.Contains(thisCollider))
        {
            return;
        }
        float dot = Vector3.Dot(velocity.normalized, Vector3.up);
        float fallingDot = Vector3.Dot(playerController.lastFrameVelocity.normalized, transform.up);
        if (dot > 0.9f)
        {
            //// Raycast left and right. Push to the area with the most leeway.
            //LayerMask layerMask = ~(1 << LayerMask.NameToLayer("Player"));
            //RaycastHit2D hitRight = Physics2D.Raycast(playerController.transform.position, playerController.transform.right, Mathf.Infinity, layerMask);
            //RaycastHit2D hitLeft = Physics2D.Raycast(playerController.transform.position, -playerController.transform.right, Mathf.Infinity, layerMask);

            //if (Vector3.Distance(playerController.transform.position, hitLeft.transform.position) < 
            //    Vector3.Distance(playerController.transform.position, hitRight.transform.position))
            //{
            //    // Move player to the right
            //    playerController.ApplyForce(new Vector2(15, 0));
            //}
            //else
            //{
            //    // Move player to the left
            //    playerController.ApplyForce(new Vector2(-15, 0));
            //}
            DestroySelf();

        }

        else if (fallingDot < 0 || fallingDot > 0.9f)
        {
            //Debug.Log("Launching");
            playerController.Jump(playerController.projectileSettings.BounceHeight / playerController.playerInput.jumpHeight);

            DestroySelf();
        }
        else
        {
            //Debug.Log("Hit straight on");
            // UnityEditor.EditorApplication.isPaused = true;
            // Should be called if it hit the player straight on
            DestroySelf();
        }
    }

    void Reflect(Vector3 normal)
    {
        if (Vector3.Dot(normal, transform.right) == 0)
        {
            despawningAgainstWall = true;
            return;
        }
        // After a bounce, make sure the collider can collide with the player if it can't already
        Physics2D.IgnoreCollision(playerCollider, thisCollider, false);
        playerController.itemsICantCollideWith.Remove(thisCollider);

        // if (col.contacts.Length > 0)
        {

            float magnitude = velocity.magnitude;
            //Vector3 normal = col.contacts[0].normal;
            float toTranslate = Vector3.Dot(velocity.normalized, Vector3.Cross(normal.normalized, Vector3.forward)) * transform.lossyScale.x;

            Vector3 velocityNormalized = velocity.normalized;
            Vector3 reflected = (2 * (Vector3.Dot(velocityNormalized, normal) * normal)) - velocityNormalized;
            velocity = -reflected;
            velocity *= magnitude;
            myTail[myTail.Count-1].direction = -reflected;
            transform.right = velocity;
            // Add height?
            transform.position += Vector3.Cross(normal.normalized, Vector3.forward) * toTranslate;
        }
    }

    public void DestroySelf()
    {
        foreach (var item in myTail)
        {
            Destroy(item.wavePart);
        }
        // Destroy self at end
        Destroy(gameObject);
    }

    Vector3 ScaleBetween(Vector3 posOne, Vector3 posTwo)
    {
        return posOne;
    }

    [System.Serializable]
    public class TailSection
    {
        public GameObject wavePart = null;
        public Vector3 direction = Vector3.zero;
        public Vector3 Pos = Vector3.zero;
        public float distFromStart = -1;
    }

    private void Pause(bool pause)
    {
        isPaused = pause;
    }

    private void OnDrawGizmos()
    {
        foreach (var item in myTail)
        {
            Gizmos.DrawSphere(item.Pos, 0.25f);
            Gizmos.DrawLine(item.Pos, item.Pos + item.direction * 0.5f);
        }
    }
}


