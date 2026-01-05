using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Clouds : MonoBehaviour
{
    public GameObject cloudPrefab;
    public GameObject balloonPrefab;

    public Sprite[] balloonSprites;
    public Sprite[] prideBalloonSprites;

    public GameObject backdrop;
    public Transform canvas;
    public UIManager ui;

    private List<GameObject> clouds;
    private List<GameObject> balloons;

    private float cloudRate;
    private float nextCloud;

    private float balloonRate;
    private float nextBalloon;

    private float cycleRate;
    private float nextCycle;

    void Awake()
    {
        clouds = new List<GameObject>();
        balloons = new List<GameObject>();

        cloudRate = UnityEngine.Random.Range(0.3f, 0.9f);
        balloonRate = UnityEngine.Random.Range(3, 9);
        cycleRate = 50;

        nextCycle = Time.time + cycleRate;
    }

    void FixedUpdate()
    {
        GameObject balloonRemove = null;
        GameObject cloudRemove = null;
        if (!ui.inGame)
        {
            foreach (GameObject currentCloud in clouds)
            {
                currentCloud.transform.position = new Vector3(currentCloud.transform.position.x + 0.05f, currentCloud.transform.position.y, 0);
                currentCloud.transform.SetSiblingIndex(1);

                if (Vector3.Distance(currentCloud.transform.position, new Vector3(0, Screen.height / 2, 0)) > Screen.width * 2)
                {
                    cloudRemove = currentCloud;
                }
            }

            foreach (GameObject currentBalloon in balloons)
            {
                currentBalloon.transform.position = new Vector3(currentBalloon.transform.position.x, currentBalloon.transform.position.y + 0.3f, 0);
                currentBalloon.transform.SetSiblingIndex(1 + clouds.Count);

                if (Vector3.Distance(currentBalloon.transform.position, new Vector3(Screen.width / 2, 0, 0)) > Screen.height * 2)
                {
                    balloonRemove = currentBalloon;
                }
            }
        }

        if (cloudRemove != null)
        {
            Destroy(cloudRemove);
            clouds.Remove(cloudRemove);
            cloudRemove = null;
        }

        if (balloonRemove != null)
        {
            Destroy(balloonRemove);
            balloons.Remove(balloonRemove);
            balloonRemove = null;
        }
    }

    void Update()
    {
        DateTime dt = DateTime.Now;

        if (Time.time > nextCloud && ui.currentMenu != -1 && !ui.inGame)
        {
            //GameObject cloud = Instantiate(cloudPrefab, new Vector3(-Screen.width / 2, UnityEngine.Random.Range(-Screen.height, Screen.height / 2), 0), Quaternion.identity);

            //cloud.transform.SetParent(canvas.transform);
            //cloud.transform.localScale = new Vector3(2, 2, 1);
            //clouds.Add(cloud);
            //cloudRate = UnityEngine.Random.Range(0.3f, 0.9f);
            //nextCloud = Time.time + cloudRate;
        }

        if (Time.time > nextBalloon && ui.currentMenu != -1 && !ui.inGame)
        {
            //GameObject balloon = null;
            //balloon = Instantiate(balloonPrefab, new Vector3(UnityEngine.Random.Range(-Screen.width, Screen.width), -Screen.height / 2, 0), Quaternion.identity);
            
            if (dt.Month != 6)
            {
                //balloon.GetComponent<Image>().sprite = balloonSprites[UnityEngine.Random.Range(1, balloonSprites.Length) - 1];
            }
            else
            {
                //balloon.GetComponent<Image>().sprite = prideBalloonSprites[UnityEngine.Random.Range(1, prideBalloonSprites.Length) - 1];
            }

            //balloon.transform.SetParent(canvas.transform);
            //balloon.transform.localScale = new Vector3(3, 3, 1);
            //balloons.Add(balloon);
            //balloonRate = UnityEngine.Random.Range(3, 9);
            //nextBalloon = Time.time + balloonRate;
        }

        if (ui.inGame)
        {
            foreach (GameObject currentCloud in clouds)
            {
                Destroy(currentCloud);
            }
            clouds.Clear();

            foreach (GameObject currentBalloon in balloons)
            {
                Destroy(currentBalloon);
            }
            balloons.Clear();

            backdrop.SetActive(false);
        }
        else
        {
            backdrop.SetActive(true);
        }
    }
}
