using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public Animator myAnime;
    Animator jumpBar;
    public Vector2 platform = new Vector2(0, 0);
    public Vector2 lastDirection;
    public Rigidbody2D myRig;
    public float speed = 2.0f;
    public float jumpSpeed = 6.0f;
    public bool canJump = true;
    public bool lastJump = false;
    public int portal = 1;
    public int jumpCharge = 0;
    public float jumpValue = 1.0f;
    GameObject[] layer;
    public bool grounded = true;
    GameObject camera;
    float aspect;
    float camSize;
    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Main Camera");
        aspect = camera.GetComponent<Camera>().aspect;
        camSize = camera.GetComponent<Camera>().orthographicSize;
        jumpBar = GameObject.Find("JumpBar").GetComponent<Animator>();
        subPortal();
        portal = 2;
        subPortal();
        portal = 0;
            
        myRig = GetComponent<Rigidbody2D>();
        myAnime = GetComponent<Animator>();
        if (myRig == null)
            throw new System.Exception("Player controller needs rigidbody");

    }
    public void onRed(InputAction.CallbackContext ev)
    {
        if(ev.started)
        { 
            jumpCharge = 0; 
        }  
    }
    public void onBlue(InputAction.CallbackContext ev)
    {
        if (ev.started)
        {
            jumpCharge = 1;
        }
    }
    public void onGreen(InputAction.CallbackContext ev)
    {
        if (ev.started)
        {
            jumpCharge = 2;
        }
    }


    private void subPortal()
    {
        layer = FindGameObjectsInLayer(6 + portal);
        foreach (GameObject o in layer)
        {
            if (o.GetComponent<SpriteRenderer>())
                o.GetComponent<SpriteRenderer>().forceRenderingOff = true;
            if (o.GetComponent<TilemapRenderer>())
            o.GetComponent<TilemapRenderer>().forceRenderingOff = true;
            if(o.GetComponent<TilemapCollider2D>())
            o.GetComponent<TilemapCollider2D>().excludeLayers += LayerMask.GetMask("Player");
        }
    }
    private void addPortal()
    {
        layer = FindGameObjectsInLayer(6 + portal);
        foreach (GameObject o in layer)
        {
            if (o.GetComponent<SpriteRenderer>())
                o.GetComponent<SpriteRenderer>().forceRenderingOff = false;
            if (o.GetComponent<TilemapRenderer>())
                o.GetComponent<TilemapRenderer>().forceRenderingOff = false;
            if (o.GetComponent<TilemapCollider2D>())
                o.GetComponent<TilemapCollider2D>().excludeLayers -= LayerMask.GetMask("Player");
        }
    }
    public void onMove(InputAction.CallbackContext ev)
    {
        if(ev.started && grounded)
            myAnime.SetInteger("AnimState", 1);

        if (ev.performed && grounded)
        {
            lastDirection = ev.ReadValue<Vector2>();
            if(grounded)
            if (lastDirection.x < 0)
            {
                this.GetComponent<SpriteRenderer>().flipX = true;
            }
            if (lastDirection.x > 0)
            {
                this.GetComponent<SpriteRenderer>().flipX = false;
            }
            
        }
        else if(ev.performed)
        {
            lastDirection = ev.ReadValue<Vector2>();
        }
        if (ev.canceled)
        {
            lastDirection = Vector2.zero;
            myAnime.SetInteger("AnimState", 0);
        }
    }
    public void onJump(InputAction.CallbackContext ev)
    {
        if (ev.started && canJump && grounded)
        {
            
            lastJump = true;
            grounded = false;
            myRig.velocity = Vector2.zero;
            
            
            
            myAnime.SetBool("Grounded", false);
            myAnime.SetBool("Jump", true);
        }
      
        if (ev.canceled)
        {
            lastJump = false;
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
        if (this.transform.position.y - camera.transform.position.y > camSize)
            camera.transform.position += new Vector3(0, 10, 0);
        else if (this.transform.position.y - camera.transform.position.y < -1.0f * camSize)
            camera.transform.position -= new Vector3(0, 10, 0);
        if (this.transform.position.x - camera.transform.position.x > aspect * camSize)
            camera.transform.position += new Vector3(10 * aspect, 0, 0);
        else if (this.transform.position.x - camera.transform.position.x < aspect * -1.0f * camSize)
            camera.transform.position -= new Vector3(10 * aspect, 0, 0);

        

        if (grounded)
        myRig.velocity = new Vector2(lastDirection.x * speed, myRig.velocity.y) + platform;
        myAnime.SetFloat("AirSpeedY", myRig.velocity.y);
        
        if (lastJump && canJump && lastJump)
        {

            if (jumpValue < 10.0f)
            {
                jumpValue += 0.1f;
                jumpBar.SetFloat("Power", jumpValue);
            }
                
            speed = 0.0f;
        }
        else if(!lastJump && jumpValue > 1.0f)
        {
            speed = 5.0f;
            jumpBar.SetFloat("Power", 0.0f);

            lastDirection = new Vector2(4* lastDirection.x/jumpValue, 0.5f + 0.07f * jumpValue).normalized;
            myRig.velocity = new Vector2(lastDirection.x * jumpValue, lastDirection.y * jumpValue);
            jumpValue = 1.0f;
            canJump = false;
            subPortal();
            portal = jumpCharge;
            addPortal();
            
        }
        else if (!canJump && myRig.velocity.y <= 0)
        {
            RaycastHit2D[] checks = Physics2D.RaycastAll(this.transform.position - new Vector3(0, 0.1f, 0), this.transform.up * -1, 0.1f);

            if (checks != null)
            {foreach(RaycastHit2D check in checks)
                    if(check.collider.gameObject.GetComponent<TilemapRenderer>() != null && check.collider.gameObject.tag != "Background")
                if(check.collider.gameObject.GetComponent<TilemapRenderer>().forceRenderingOff == false)
                if (check.distance < 0.1f)
                {
                    canJump = true;
                    
                    myAnime.SetBool("Grounded", true);
                    grounded = true;
                    
                    
                }
            }

        }
    }
}