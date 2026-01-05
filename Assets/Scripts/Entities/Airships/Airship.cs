using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airship : MonoBehaviour
{
    public Rigidbody2D rb;

    public bool touchingLever;
    public bool touchingHandle;

    public Lever lever;
    public Handle handle;

    public int moveDir;
    public float moveSpeed;

    public Transform left;
    public Transform right;

    public LayerMask whatIsGround;
    public Vector3 boxDims;

    public PlayerMove player;
    public bool interact;

    public float a;
    public float a2;

    void Awake()
    {
        moveDir = 2;

        if (GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player").GetComponent<PlayerMove>();
        }
        else
        {
            player = null; 
        }

        interact = false;
    }

    void Update()
    {
        if (player != null)
        {
            bool hitLeft = Physics2D.OverlapBox(left.position, boxDims, 1, whatIsGround);
            bool hitRight = Physics2D.OverlapBox(right.position, boxDims, 1, whatIsGround);

            if (interact && touchingLever)
            {
                if (Input.GetKeyDown("a") && moveDir > 1)
                {
                    moveDir--;
                    lever.SetFrame(moveDir - 1);
                }

                if (Input.GetKeyDown("d") && moveDir < 3)
                {
                    moveDir++;
                    lever.SetFrame(moveDir - 1);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            bool hitLeft = Physics2D.OverlapBox(left.position, boxDims, 1, whatIsGround);
            bool hitRight = Physics2D.OverlapBox(right.position, boxDims, 1, whatIsGround);

            //handle
            if (interact && touchingHandle)
            {
                if (Input.GetKey("w"))
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + a2, 0);
                    a2 = Mathf.Lerp(a2, moveSpeed, 0.0002f);

                    handle.SetFrame(4);
                }
                else if (Input.GetKey("s"))
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + a2, 0);
                    a2 = Mathf.Lerp(a2, -moveSpeed, 0.0002f);

                    handle.SetFrame(0);
                }
                else
                {
                    a2 = Mathf.Lerp(a2, 0, 0.002f);
                    transform.position = new Vector3(transform.position.x, transform.position.y + a2, 0);

                    handle.SetFrame(2);
                }
            }
            else
            {
                a2 = Mathf.Lerp(a2, 0, 0.002f);
                transform.position = new Vector3(transform.position.x, transform.position.y + a2, 0);

                handle.SetFrame(2);
            }

            if (moveDir == 3 && !hitRight)
            {
                transform.position = new Vector3(transform.position.x + a, transform.position.y, 0);
                a = Mathf.Lerp(a, moveSpeed, 0.0002f);
            }
            else if (moveDir == 1 && !hitLeft)
            {
                transform.position = new Vector3(transform.position.x + a, transform.position.y, 0);
                a = Mathf.Lerp(a, -moveSpeed, 0.0002f);
            }
            else
            {
                a = Mathf.Lerp(a, 0, 0.002f);
                transform.position = new Vector3(transform.position.x + a, transform.position.y, 0);
            }
        }
        else
        {
            if (GameObject.Find("Player") != null)
            {
                player = GameObject.Find("Player").GetComponent<PlayerMove>();
            }
            else
            {
                player = null;
            }
        }
    }
}
