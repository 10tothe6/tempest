using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breaker : MonoBehaviour
{
    public Sprite[] frames;

    public void SetFrame(int i)
    {
        if (i == 9)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = null;
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = frames[i];
        }
    }
}
