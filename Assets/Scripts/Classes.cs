using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Reference
{
    public string name;
    public int id;
}

[System.Serializable]
public class FoliageReference
{
    public string name;
    public int id;
    public float weight;

    //if this is true only use the first tile in the foliage's tile array, in other words the foliage should not have variation
    public bool single;
}

[System.Serializable]
public class StructureReference
{
    public string name;
    public int id;

    //how rare is this structure? The higher the number, the rarer the structure
    public int spawnChance;
}

[System.Serializable]
public class Effect
{
    public int id;
    public float timer;
}

[System.Serializable]
public class Message
{
    public int day;
    public string text;
}

[System.Serializable]
public class Entity
{
    public float x;
    public float y;
    public float z;

    public int type;
    public Stack stack;
}

[System.Serializable]
public class Chest
{
    public List<Stack> items;

    public float coordX;
    public float coordY;

    public bool background;
}

[System.Serializable]
public class Achievement
{
    public string name;
    public string description;

    public Sprite icon;
}

[System.Serializable]
public class Stack
{
    public int count;
    public int itemType;

    public int i;
}

[System.Serializable]
public class Recipe
{
    public string name;
    public int type;

    public int output;
    public int count;
    public string[] inputs;
}

[System.Serializable]
public class ChunkTile
{
    public int front;
    public int frontId;

    public int back;
    public int backId;
}

[System.Serializable]
public class Item
{
    public string name;
    public string description;
    public int maxStackSize;

    public Sprite sprite;

    public int place;
    public int placeStage;

    public int giveHealth;

    public bool giveEffect;
    public int[] effects;
    public float[] effectTimes;

    public bool isPickaxe;
    public float pickaxeSpeed;
    public string affectedBlock;

    public bool isSword;
    public bool isBow;
    public bool isAmmo;

    public float damage;

    public bool spawnBalloon;

    public Sprite placePreview;

    public List<string> tags;
}

[System.Serializable]
public class Updater
{
    public int intervalSpeed;

    public int spawnStage;
}

[System.Serializable]
public class Structure
{
    public string name;
    public int[] data;
    public int[] data2;

    public Chest[] chests;

    public bool noTreeheight;
    public bool forceGenerate;

    public int width;
    public int height;

    public int offsetX;
    public int offsetY;
}

[System.Serializable]
public class Drop
{
    public int dropId;
    public int dropCount;
}

[System.Serializable]
public class ChunkUpdater
{
    public int blockType;
    public int currentStage;

    public int coordX;
    public int coordY;

    public bool background;

    public int time;
}

[System.Serializable]
public class Block
{
    public string name;
    public bool obstructable;
    public List<TileBase> tiles;

    public Drop[] drops;

    public GameObject lightPrefab;

    public float breakTime;

    public bool isClimb;

    public bool dependent;
    public int offsetX;
    public int offsetY;
    public List<string> placeTags;

    public List<string> tags;

    public Updater updater;
}

[System.Serializable]
public class Biome
{
    //don't really have to explain this
    public string name;

    //blocks needed to generate that biome
    public Reference[] blocks; 

    //structures that can generate in the biome
    public StructureReference[] structures;

    //flowers and plants and such that are in the biome
    public FoliageReference[] foliage;
    public FoliageReference[] caveFloorFoliage;
    public FoliageReference[] caveCeilingFoliage;

    //it's the color of the sky. Duh
    public Color skyColor;

    //how smooth the perlin noise is, there is a bug that creates seams when this is changed
    public float smoothMap;
    public float roughMap;
    public float blendMap;
    public float caveMap;

    //how thick the band of surface blocks is
    public int surfaceThickness;

    //which biomes can generate after the current one
    public Reference[] compatBiomes;

    //how deep the stone starts
    public int stoneThreshold;
}
