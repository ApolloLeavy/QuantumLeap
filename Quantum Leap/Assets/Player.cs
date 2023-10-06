using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Animator myAnime;
    public Vector2 platform = new Vector2(0, 0);
    public Vector2 lastDirection;
    public Rigidbody2D myRig;
    public float speed = 4.0f;
    public float jumpSpeed = 8.0f;
    public bool canJump = true;
    public bool lastJump = false;
    public int portal = 1;
    GameObject[] layer;
    // Start is called before the first frame update
    void Start()
    {
        subPortal();
        portal = 2;
        subPortal();
        portal = 0;
            
        myRig = GetComponent<Rigidbody2D>();
        myAnime = GetComponent<Animator>();
        if (myRig == null)
            throw new System.Exception("Player controller needs rigidbody");

    }
    public void onLeftPortal(InputAction.CallbackContext ev)
    {
        if(ev.started)
        {
            subPortal();
            portal -= 1;
            if (portal == -1)
                portal = 2;
            addPortal();
        }
        
    }
    public void onRightPortal(InputAction.CallbackContext ev)
    {
        if (ev.started)
        {
            subPortal();
            portal += 1;
            if (portal == 3)
                portal = 0;
            addPortal();
        }
    }

    private void subPortal()
    {
        layer = FindGameObjectsInLayer(6 + portal);
        foreach (GameObject o in layer)
        {
            o.GetComponent<SpriteRenderer>().forceRenderingOff = true;
            o.GetComponent<Collider2D>().excludeLayers += LayerMask.GetMask("Player");
        }
    }
    private void addPortal()
    {
        layer = FindGameObjectsInLayer(6 + portal);
        foreach (GameObject o in layer)
        {
            o.GetComponent<SpriteRenderer>().forceRenderingOff = false;
            o.GetComponent<Collider2D>().excludeLayers -= LayerMask.GetMask("Player");
        }
    }
    public void onMove(InputAction.CallbackContext ev)
    {
        if(ev.started)
            myAnime.SetInteger("AnimState", 1);
        if (ev.performed)
        {
            lastDirection = ev.ReadValue<Vector2>();
            
            if (lastDirection.x < 0)
            {
                this.GetComponent<SpriteRenderer>().flipX = true;
            }
            if (lastDirection.x > 0)
            {
                this.GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        if (ev.canceled)
        {
            lastDirection = Vector2.zero;
            myAnime.SetInteger("AnimState", 0);
        }
    }
    public void onJump(InputAction.CallbackContext ev)
    {
        if (ev.started && canJump)
        {
            lastJump = true;
            myAnime.SetBool("Grounded", false);
            myAnime.SetBool("Jump", true);
        }
        if (ev.canceled)
        {
            
            myAnime.SetBool("Jump", false);
        }
    }
    GameObject[] FindGameObjectsInLayer(int layer)
    {
        var goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        var goList = new System.Collections.Generic.List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == layer)
            {
                goList.Add(goArray[i]);
            }
        }
        if (goList.Count == 0)
        {
            return null;
        }
        return goList.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        myRig.angularVelocity = lastDirection.x;
        myRig.velocity = new Vector2(lastDirection.x * speed, myRig.velocity.y) + platform;
        myAnime.SetFloat("AirSpeedY", myRig.velocity.y);

        if (lastJump && canJump)
        {
            myRig.velocity += new Vector2(0, jumpSpeed);
            lastJump = false;
            canJump = false;
        }
        else if (!canJump && myRig.velocity.y <= 0)
        {
            RaycastHit2D check;

            if (check = Physics2D.Raycast(this.transform.position - new Vector3(0,0.9f,0), this.transform.up * -1))
            {
                if (check.distance < 0.01f)
                {
                    canJump = true;
                    myAnime.SetBool("Grounded", true);
                }
            }

        }
    }
}