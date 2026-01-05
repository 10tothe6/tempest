using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Storm : MonoBehaviour
{
    private float length, startPosX;
    private float startPosY;
    public GameObject cam;
    public UIManager ui;
    public float amplitude;

    void Start()
    {
        startPosX = transform.position.x;
        //FIX THIS
        startPosY = transform.position.y - 7;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        if (ui.inGame)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(this.gameObject.GetComponent<SpriteRenderer>().color.r, this.gameObject.GetComponent<SpriteRenderer>().color.g, this.gameObject.GetComponent<SpriteRenderer>().color.b, 100);

            float temp = (cam.transform.position.x * (1 - amplitude));
            float dist = (cam.transform.position.x * amplitude);

            transform.position = new Vector3(startPosX + dist, startPosY, transform.position.z);

            if (temp > startPosX + length * 2)
            {
                startPosX += length * 3;
            }
            else if (temp < startPosX - length * 2)
            {
                startPosX -= length * 3;
            }
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(this.gameObject.GetComponent<SpriteRenderer>().color.r, this.gameObject.GetComponent<SpriteRenderer>().color.g, this.gameObject.GetComponent<SpriteRenderer>().color.b, 0);
        }
    }
}
