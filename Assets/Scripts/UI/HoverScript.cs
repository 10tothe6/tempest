using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverScript : MonoBehaviour
{
    public float offsetTime;
    public bool canvasObject;

    private float a;

    void Awake()
    {
        a = 0 + offsetTime;
    }

    void Update()
    {
        if (canvasObject)
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector3(GetComponent<RectTransform>().anchoredPosition.x, GetComponent<RectTransform>().anchoredPosition.y + Mathf.Sin(a) / 10);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(a) / 2000, transform.position.z);
        }

        a += 0.005f;
    }
}
