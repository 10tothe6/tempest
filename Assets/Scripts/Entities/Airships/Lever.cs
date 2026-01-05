using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    public Airship airship;

    public List<Sprite> frames;
    public SpriteRenderer sr;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            airship.touchingLever = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            airship.touchingLever = false;
        }
    }

    public void SetFrame(int id)
    {
        sr.sprite = frames[id];
    }
}
