using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class Arrow : MonoBehaviour
{
    public float damage;
    public float mult;

    public bool glow;
    public bool grav;

    public GameObject guide;
    public float velocity;

    public bool landed;

    private bool touchingCreature;
    public LayerMask creatureLayer;

    private float timer;

    void Awake()
    {
        velocity = 0.01f;

        timer = Time.time + 2;
        
        if (grav)
        {
            GetComponent<Rigidbody2D>().gravityScale = 1;
        }
        else
        {
            GetComponent<Rigidbody2D>().gravityScale = 0;
        }
    }

    void Update()
    {
        if (glow)
        {
            GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().enabled = true;
        }
        else
        {
            GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().enabled = false;
        }

        touchingCreature = Physics2D.OverlapCircle(transform.position, 0.5f, creatureLayer);

        if (guide != null)
        {
            transform.up = guide.transform.position - transform.position;
        }

        if (landed)
        {
            this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            if (guide != null)
            {
                Destroy(guide);
                guide = null;
            }
        }

        if (!grav && Time.time > timer)
        {
            if (guide != null)
            {
                Destroy(guide);
                guide = null;
            }

            Destroy(this.gameObject);
        }

        if (touchingCreature && !landed)
        {
            Physics2D.OverlapCircle(transform.position, 0.5f, creatureLayer).gameObject.GetComponent<HealthManager>().Damage(damage * mult);
            Debug.Log(damage * mult);

            if (guide != null)
            {
                Destroy(guide);
                guide = null;
            }

            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.layer == 3 || collider.gameObject.layer == 8)
        {
            this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            landed = true;
        }
    }
}
