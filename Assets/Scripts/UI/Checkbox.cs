using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Checkbox : MonoBehaviour
{
    private UIManager ui;
    private RectTransform rt;
    private Image img;

    public Sprite[] sprites;

    private bool yes;

    void Awake()
    {
        ui = GameObject.Find("UIManager").GetComponent<UIManager>();
        rt = GetComponent<RectTransform>();
        img = GetComponent<Image>();
    }

    void Update()
    {
        if (Vector3.Distance(Input.mousePosition, rt.anchoredPosition) < 200 && Input.GetMouseButtonDown(0))
        {
            ui.tutorial.enableTips = !ui.tutorial.enableTips;
            yes = !yes;
        }

        if (yes)
        {
            img.sprite = sprites[1];
        }
        else
        {
            img.sprite = sprites[0];
        }
    }
}
