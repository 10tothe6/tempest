using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public int worldSeed;
    public string worldName;
    public int biomeHeight;
    public int biomeHeight2;

    public List<Stack> inventory;
    public List<Stack> hotbar;

    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;

    public float health;
    public float hunger;

    public float time;
    public bool isDay;

    public List<string> foundNotes;

    public int worldModifier;

    public float lifeCount;

    public int deathCount;

    public int ability;

    public int dayCount;

    public float[] h;
    public float[] s;
    public float[] v;
}
