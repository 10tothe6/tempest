using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayCounter : MonoBehaviour
{
    public PlayerMove player;

    private float x;
    private float y;

    private float dayCount;
    public Sprite[] numbers;

    public Image display1;
    public Image display2;

    void Update()
    {
        dayCount = player.dayCount;

        if (Mathf.RoundToInt(dayCount / 10) * 10 <= dayCount)
        {
            x = Mathf.RoundToInt(dayCount / 10) * 10;
        }
        else
        {
            x = Mathf.RoundToInt(dayCount / 10) * 10 - 10;
        }

        y = dayCount - x;

        display1.sprite = numbers[(int)x / 10];
        display2.sprite = numbers[(int)y];
    }
}
