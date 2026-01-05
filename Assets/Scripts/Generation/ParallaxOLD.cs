using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ParallaxOLD : MonoBehaviour
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
        length = GetComponent<Tilemap>().size.x;
    }

    void FixedUpdate()
    {
        if (ui.inGame)
        {
            this.gameObject.GetComponent<Tilemap>().color = new Color(this.gameObject.GetComponent<Tilemap>().color.r, this.gameObject.GetComponent<Tilemap>().color.g, this.gameObject.GetComponent<Tilemap>().color.b, 100);

            float temp = (cam.transform.position.x * (1 - amplitude));
            float dist = (cam.transform.position.x * amplitude);

            transform.position = new Vector3(startPosX + dist, startPosY, transform.position.z);

            if (transform.parent.gameObject.name == "1")
            {
                if (temp > startPosX + length / 3)
                {
                    startPosX += length - 58.75f;
                }
                else if (temp < startPosX - length / 3)
                {
                    startPosX -= length - 58.75f;
                }
            }
            else
            {
                if (temp > startPosX + length / 2)
                {
                    startPosX += length - 1;

                }
                else if (temp < startPosX - length / 2)
                {
                    startPosX -= length - 1;
                }
            }
        }
        else
        {
            this.gameObject.GetComponent<Tilemap>().color = new Color(this.gameObject.GetComponent<Tilemap>().color.r, this.gameObject.GetComponent<Tilemap>().color.g, this.gameObject.GetComponent<Tilemap>().color.b, 0);
        }
    }
}
