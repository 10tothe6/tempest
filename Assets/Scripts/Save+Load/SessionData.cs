using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SessionData
{
    public float[] h1;
    public float[] s1;
    public float[] v1;

    public float[] h2;
    public float[] s2;
    public float[] v2;

    public bool[] achievements;

    public bool warningActivate;

    public int balloonsCrafted;
    public int soulsKilled;
    public int spiritsKilled;
    public bool[] biomesVisited;

    public float musicVolume;
    public float ambientVolume;
    public float soundVolume;
    public float masterVolume;
}
