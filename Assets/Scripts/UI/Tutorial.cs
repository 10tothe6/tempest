using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public UIManager ui;
    public Transform player;
    public bool enableTips;
    public GameObject[] tips;
    public GameObject[] uiTips;
    public Vector3[] offsets;

    public bool[] tasks;
    private float a;

    void Awake()
    {
        ui = GameObject.Find("UIManager").GetComponent<UIManager>();
        tasks = new bool[3];

        a = 0;
    }

    void Update()
    {
        if (enableTips)
        {
            if (!ui.inGame)
            {
                for (int i = 0; i < tips.Length; i++)
                {
                    tips[i].SetActive(true);
                }

                for (int i = 0; i < uiTips.Length; i++)
                {
                    uiTips[i].SetActive(true);
                }
            }

            if (ui.inGame)
            {
                if (Input.GetKey("e"))
                {
                    uiTips[4].SetActive(false);
                }

                if (Input.GetKey("c"))
                {
                    uiTips[6].SetActive(false);
                }

                if (Input.GetKey("r"))
                {
                    uiTips[5].SetActive(false);
                }

                if (ui.menuArray[8].activeSelf || ui.menuArray[11].activeSelf || ui.menuArray[16].activeSelf)
                {
                    if (ui.inventory.held != null || ui.hotbar.held != null)
                    {
                        uiTips[0].SetActive(false);

                        uiTips[1].SetActive(false);

                        uiTips[2].SetActive(false);
                    }
                }

                if (ui.menuArray[12].activeSelf)
                {
                    if (Input.GetKey("d") || Input.GetKey("a"))
                    {
                        uiTips[3].SetActive(false);
                    }
                }

                for (int i = 0; i < tips.Length; i++)
                {
                    tips[i].transform.position = new Vector3(player.position.x + offsets[i].x, player.position.y + offsets[i].y + Mathf.Sin(a + i) / 25, player.position.z + offsets[i].z);
                }

                for (int i = 0; i < uiTips.Length; i++)
                {
                    uiTips[i].transform.position = new Vector3(uiTips[i].transform.position.x, uiTips[i].transform.position.y + Mathf.Sin(a + i) / 10, uiTips[i].transform.position.z);
                }

                if (Input.GetKey("a") || Input.GetKey("d") || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
                {
                    tasks[0] = true;
                }

                if (Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Space))
                {
                    tasks[1] = true;
                }

                if (Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftControl))
                {
                    tasks[2] = true;
                }

                if (tasks[0])
                {
                    tips[0].SetActive(false);
                }

                if (tasks[1])
                {
                    tips[1].SetActive(false);
                }

                if (tasks[2])
                {
                    tips[2].SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < tips.Length; i++)
            {
                tips[i].SetActive(false);
            }

            for (int i = 0; i < uiTips.Length; i++)
            {
                uiTips[i].SetActive(false);
            }
        }

        a += 0.02f;
    }
}
