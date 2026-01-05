using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handle : MonoBehaviour
{
    public Airship airship;

    public List<Sprite> frames;
    public SpriteRenderer sr;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            airship.touchingHandle = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            airship.touchingHandle = false;
        }
    }

    public void SetFrame(int id)
    {
        sr.sprite = frames[id];
    }
}
