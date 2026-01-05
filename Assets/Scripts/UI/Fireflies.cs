using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fireflies : MonoBehaviour
{
    public GameObject cam;
    private UIManager ui;
    public Color[] starColors;

    public Vector3[] driftPos;
    public Vector3[] oldPos;
    public bool[] drifting;

    public float driftSpeed;

    void Awake()
    {
        oldPos = new Vector3[cam.transform.GetChild(1).childCount];
        driftPos = new Vector3[cam.transform.GetChild(1).childCount];
        drifting = new bool[cam.transform.GetChild(1).childCount];

        for (int i = 0; i < cam.transform.GetChild(1).childCount; i++)
        {
            RectTransform currentStar = cam.transform.GetChild(1).GetChild(i).gameObject.GetComponent<RectTransform>();

            float newScale = Random.Range(0.5f, 1.5f);
            currentStar.localScale = new Vector3(newScale, newScale, 1);

            int newColor = Random.Range(0, starColors.Length - 1);
            currentStar.gameObject.GetComponent<Image>().color = starColors[newColor];
        }

        for (int i = 0; i < cam.transform.GetChild(1).childCount; i++)
        {
            oldPos[i] = cam.transform.GetChild(1).GetChild(i).gameObject.GetComponent<RectTransform>().anchoredPosition;
        }

        ui = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    void FixedUpdate()
    {
        if (ui.inGame)
        {
            for (int i = 0; i < cam.transform.GetChild(1).childCount; i++)
            {
                RectTransform currentStar = cam.transform.GetChild(1).GetChild(i).gameObject.GetComponent<RectTransform>();

                currentStar.gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < cam.transform.GetChild(1).childCount; i++)
            {
                RectTransform currentStar = cam.transform.GetChild(1).GetChild(i).gameObject.GetComponent<RectTransform>();

                currentStar.gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < cam.transform.GetChild(1).childCount; i++)
        {
            RectTransform currentStar = cam.transform.GetChild(1).GetChild(i).gameObject.GetComponent<RectTransform>();

            if (drifting[i])
            {
                currentStar.anchoredPosition = Vector3.Lerp(currentStar.anchoredPosition, driftPos[i], driftSpeed);
            }
            else
            {
                currentStar.anchoredPosition = Vector3.Lerp(currentStar.anchoredPosition, oldPos[i], driftSpeed);
            }
        }

        for (int i = 0; i < cam.transform.GetChild(1).childCount; i++)
        {
            RectTransform currentStar = cam.transform.GetChild(1).GetChild(i).gameObject.GetComponent<RectTransform>();

            if (Vector3.Distance(currentStar.anchoredPosition, oldPos[i]) < 5 && !drifting[i])
            {
                float offsetX = Random.Range(-250, 250);
                float offsetY = Random.Range(-250, 250);
                driftPos[i] = new Vector3(currentStar.anchoredPosition.x + offsetX, currentStar.anchoredPosition.y + offsetY);

                drifting[i] = true;
            }

            if (Vector3.Distance(currentStar.anchoredPosition, driftPos[i]) < 5)
            {
                drifting[i] = false;
            }
        }
    }
}
