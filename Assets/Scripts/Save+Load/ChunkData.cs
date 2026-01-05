using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
    public int[,] chunkArray;
    public int[,] chunkArray2;
    public List<Entity> entities;
    public int biome;
    public int biomeLeft;
    public int addHeight;

    public int structureType;
    public int column;
    public int startCoordY;
    public int treeHeight;

    public int biomeHeight;
    public int biomeHeight2;

    public List<ChunkUpdater> chunkUpdaters;
    public List<Chest> chests;
}
