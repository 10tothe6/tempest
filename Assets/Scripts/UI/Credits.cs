using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public bool runCredits;
    public bool playingCredits;
    public Vector3 startPos;

    public float scrollSpeed;

    private bool transition;

    void Update()
    {
        if (runCredits)
        {
            if (!playingCredits && !transition)
            {
                StartCoroutine("Transition");
            }
        }
        else
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
            this.transform.GetChild(1).gameObject.SetActive(false);

            playingCredits = false;
        }

        if (transition)
        {
            transform.GetChild(2).gameObject.GetComponent<Image>().color = Color.Lerp(transform.GetChild(2).gameObject.GetComponent<Image>().color, new Color(1, 1, 1, 1), 0.05f);
        }
        else
        {
            transform.GetChild(2).gameObject.GetComponent<Image>().color = Color.Lerp(transform.GetChild(2).gameObject.GetComponent<Image>().color, new Color(1, 1, 1, 0), 0.05f);
        }
    }

    IEnumerator Transition()
    {
        transition = true;
        playingCredits = true;
        transform.GetChild(2).gameObject.SetActive(true);

        yield return new WaitForSeconds(1);
        this.transform.GetChild(0).gameObject.SetActive(true);

        transition = false;

        yield return new WaitForSeconds(1);
        StartCoroutine("RunCredits");

        transform.GetChild(2).gameObject.SetActive(false);
    }

    public IEnumerator RunCredits()
    {
        if (!transition)
        {
            transition = true;
            this.transform.GetChild(1).gameObject.GetComponent<RectTransform>().anchoredPosition = startPos;
        }

        this.transform.GetChild(0).gameObject.SetActive(true);
        this.transform.GetChild(1).gameObject.SetActive(true);

        if (this.transform.GetChild(1).gameObject.GetComponent<RectTransform>().anchoredPosition.y < 100)
        {
            this.transform.GetChild(1).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(this.transform.GetChild(1).gameObject.GetComponent<RectTransform>().anchoredPosition.x, this.transform.GetChild(1).gameObject.GetComponent<RectTransform>().anchoredPosition.y + scrollSpeed, 0);
        }
        else if (this.transform.GetChild(1).gameObject.GetComponent<Text>().color.a > 0)
        {
            this.transform.GetChild(1).gameObject.GetComponent<Text>().color = new Color(this.transform.GetChild(1).gameObject.GetComponent<Text>().color.r, this.transform.GetChild(1).gameObject.GetComponent<Text>().color.g, this.transform.GetChild(1).gameObject.GetComponent<Text>().color.b, this.transform.GetChild(1).gameObject.GetComponent<Text>().color.a - 0.005f);
        }
        else
        {
            runCredits = false;

            if (GameObject.Find("UIManager").GetComponent<UIManager>().session.achievements[2] == false)
            {
                StartCoroutine(GameObject.Find("UIManager").GetComponent<UIManager>().Achievement(2));
                Debug.Log("Achievement Get!");
            }

            if (GameObject.Find("UIManager").GetComponent<UIManager>().session.achievements[13] == false && GameObject.Find("Player").GetComponent<PlayerMove>().dayCount < 4)
            {
                StartCoroutine(GameObject.Find("UIManager").GetComponent<UIManager>().Achievement(13));
                Debug.Log("Achievement Get!");
            }

            if (GameObject.Find("UIManager").GetComponent<UIManager>().session.achievements[15] == false && GameObject.Find("WorldGenerator").GetComponent<Generation>().deathCount < 1)
            {
                StartCoroutine(GameObject.Find("UIManager").GetComponent<UIManager>().Achievement(15));
                Debug.Log("Achievement Get!");
            }
        }

        yield return new WaitForSeconds(0.01f);

        if (playingCredits)
        {
            StartCoroutine("RunCredits");
        }
    }
}
