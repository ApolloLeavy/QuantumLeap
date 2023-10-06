using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class scr : MonoBehaviour
{
    public Animator myAnime;
    public Vector2 lastDirection;
    public Rigidbody myRig;
    public float speed = 3.0f;
    public bool canJump = true;
    public bool lastJump = false;
    public float jumpSpeed = 8.0f;
    public Vector3 speedMod = new Vector3(0.0f,0.0f,0.0f);
    // Start is called before the first frame update
    void Start()
    {
        myAnime = this.GetComponent<Animator>();
        myRig = GetComponent<Rigidbody>();
        if (myRig == null)
            throw new System.Exception("Player controller needs rigidbody");
        /*
        myTarget = GameObject.FindGameObjectWithTag("Finish");
        Rigidbody targRB = myTarget.GetComponent<Rigidbody>();
        if (targRB != null)
        {
            targRB.velocity = new Vector3(0, 1, 0);
        }
        else
        {
            Debug.Log("Target had no rigidbody");
            
        }*/
    }


    public void onMove(InputAction.CallbackContext ev)
    {
        if (ev.started)
            myAnime.SetInteger("Action", 3);
        if (ev.performed)
        { lastDirection = ev.ReadValue<Vector2>();
            myAnime.SetInteger("Action", 3);
        }
        if (ev.canceled)
        {
            lastDirection = Vector2.zero;
            myAnime.SetInteger("Action", 0);
        }
    }
    public void onJump(InputAction.CallbackContext ev)
    {
        /* if(ev.started)
        {
            lastJump = true;
        } */
        if (ev.started)
            myAnime.SetInteger("Action", 2);
    }
    public void onFire(InputAction.CallbackContext ev)
    {
        if (ev.started)
            myAnime.SetInteger("Action", 1);
       
    }
    // Update is called once per frame
    void Update()
    {
        if(lastDirection != Vector2.zero)
        {
            myAnime.SetInteger("Action", 3);
        }
        Vector3 targetV = new Vector3(lastDirection.x, 0, lastDirection.y).normalized * speed;
        //myRig.velocity = Vector3.MoveTowards(myRig.velocity, targetV, 3*Time.deltaTime);
       //myRig.velocity = Vector3.Lerp(myRig.velocity, targetV, .1f);
        myRig.angularVelocity = new Vector3(0, lastDirection.x, 0)*speed;
        myRig.velocity = -1 * transform.right * speed * lastDirection.y + new Vector3(0,myRig.velocity.y,0) + speedMod;
        if (lastJump && canJump)
        {
            myRig.velocity += new Vector3(0, jumpSpeed, 0);
            lastJump = false;
            canJump = false;
        }
        else if (!canJump && myRig.velocity.y <= 0)
        {
            
            RaycastHit check;
            if(Physics.SphereCast(this.transform.position, .1f, this.transform.up * -1, out check))
            {
                if(check.distance <2.5f)
                {
                    canJump = true;
                }
            }

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        /*if (collision.gameObject.tag == "Floor")
        {
            canJump = true;
        }*/

    }
}
