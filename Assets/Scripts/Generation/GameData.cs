using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class GameData : MonoBehaviour
{
    public WorldData worldData;

    //all biomes in the game
    [SerializeField]
    public Biome[] biomes;

    //all structures in the game
    [SerializeField]
    public Structure[] structures;

    //all tiles in the game
    [SerializeField]
    public List<Block> blocks;

    //all items in the game
    [SerializeField]
    public List<Item> items;

    //all recipes in the game
    [SerializeField]
    public List<Recipe> recipes;

    //all entities in the game
    [SerializeField]
    public List<GameObject> entities;

    //all notes in the game
    [SerializeField]
    public List<Note> notes;

    public List<Achievement> achievements;

    //structure recording
    public Tilemap structureIndex;
    public Tilemap structureIndex2;
    public bool structureOverwrite;
    private int writeTile;
    private int writeTile2;
    private TileBase readTile;
    private TileBase readTile2;
    public int structureWidth;
    public int structureHeight;
    public int structureFile;

    //index finding
    public bool find;
    public bool itemSearch;
    public string input;
    public string output;

    void Awake()
    {
        structureIndex.gameObject.SetActive(false);
    }

    public void LoadWorld(string worldName)
    {
        if (SaveSystem.LoadWorld(worldName) != null)
        {
            worldData = SaveSystem.LoadWorld(worldName);
        }
        else
        {
            worldData = new WorldData();
        }
    }

    //Records a new structure
    void Update()
    {
        transform.GetChild(0).gameObject.transform.position = new Vector3Int(Mathf.RoundToInt(transform.position.x - 0.2f), Mathf.RoundToInt(transform.position.y - 0.2f), 0);

        if (structureOverwrite)
        {
            int writeIndex = 0;
            structures[structureFile].width = structureWidth;
            structures[structureFile].height = structureHeight;
            structures[structureFile].data = new int[structureWidth * structureHeight];
            structures[structureFile].data2 = new int[structureWidth * structureHeight];

            for (int x = 0; x < structureWidth; x++)
            {
                for (int y = 0; y < structureHeight; y++)
                {
                    readTile = structureIndex.GetTile(new Vector3Int(Mathf.RoundToInt(transform.position.x + x - 0.2f), Mathf.RoundToInt(transform.position.y + y - 0.2f), 0));
                    readTile2 = structureIndex2.GetTile(new Vector3Int(Mathf.RoundToInt(transform.position.x + x - 0.2f), Mathf.RoundToInt(transform.position.y + y - 0.2f), 0));

                    Debug.Log(readTile);

                    writeTile = 0;
                    writeTile2 = 0;

                    foreach (Block currentBlock in blocks)
                    {
                        if (currentBlock.tiles.Contains(readTile))
                        {
                            writeTile = blocks.IndexOf(currentBlock) + 1;
                            Debug.Log(1);
                        }

                        if (currentBlock.tiles.Contains(readTile2))
                        {
                            writeTile2 = blocks.IndexOf(currentBlock) + 1;
                            Debug.Log(1);
                        }
                    }
                    structures[structureFile].data[writeIndex] = writeTile;
                    structures[structureFile].data2[writeIndex] = writeTile2;

                    writeIndex++;
                }
            }

            structureOverwrite = false;
        }

        if (find)
        {
            if (!string.IsNullOrEmpty(input))
            {
                if (!itemSearch)
                {
                    int outNumber = 0;
                    if (int.TryParse(input, out outNumber) == true)
                    {
                        if (outNumber - 1 >= 0 && outNumber <= blocks.Count)
                        {
                            output = blocks[outNumber - 1].name;
                        }
                        else
                        {
                            output = "ERROR";
                        }
                    }
                    else
                    {
                        bool found = false;
                        foreach (Block currentBlock in blocks)
                        {
                            if (currentBlock.name == input)
                            {
                                output = (blocks.IndexOf(currentBlock) + 1).ToString();
                                found = true;

                                break;
                            }
                        }

                        if (!found)
                        {
                            output = "ERROR";
                        }
                    }
                }
                else
                {
                    int outNumber = 0;
                    if (int.TryParse(input, out outNumber) == true)
                    {
                        if (outNumber - 1 >= 0 && outNumber <= items.Count)
                        {
                            output = items[outNumber - 1].name;
                        }
                        else
                        {
                            output = "ERROR";
                        }
                    }
                    else
                    {
                        bool found = false;
                        foreach (Item currentItem in items)
                        {
                            if (currentItem.name == input)
                            {
                                output = (items.IndexOf(currentItem) + 1).ToString();
                                found = true;

                                break;
                            }
                        }

                        if (!found)
                        {
                            output = "ERROR";
                        }
                    }
                }
            }

            find = false;
        }
    }
}
