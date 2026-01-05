using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.Experimental.Rendering.LWRP;
using System.Threading.Tasks;

public class ChunkGeneration : MonoBehaviour
{
    public ChunkData chunkData;

    [Header("Object References")]
    public GameData dataObject;
    public Generation generatorObject;
    public GameObject entityScript;
    public GameObject player;

    [Header("Chunk Data")]
    public Tilemap tilemap;
    public Tilemap tilemap2;
    public int[,] chunkArray;
    public int[,] chunkArray2;
    public int chunkWidth;
    public int chunkHeight;

    //helps with generation
    private int currentTile;
    private int currentTile2;
    private int foliageTile;

    [Header("Seed")]
    public int seed;

    [Header("Chunk Position")]
    public float chunkCoordX;
    public float chunkCoordY;

    [Header("Biome Info")]
    public int biome;
    public int biomeLeft;

    [Header("Chunk Height")]
    public int addHeight;
    public int biomeHeight;
    public int biomeHeight2;

    public int worldModifier;

    [Header("Structures")]
    public int structureStop;
    public int startCoordY;

    public int structureLeftIdVar;
    public int structureLeftColumnVar;
    public int structureLeftYVar;
    public int treeHeightVar;

    public bool overChunkBorder;

    [Header("Misc.")]
    public float distanceThreshold;
    private bool unloaded;

    public List<GameObject> entities;
    public List<GameObject> lights;
    public List<Chest> chests;
    public List<ChunkUpdater> chunkUpdaters;
    private List<ChunkUpdater> remove;
    public GameObject leafParticle;
    public List<GameObject> leafParticles;
    public GameObject[] butterflies;

    public bool spawnNote;
    public bool spawnNPC;

    public float oldTime;

    [Header("Caves")]
    public float caveSize;
    public float caveThreshold;
    public float falloff;

    private void Awake()
    {
        unloaded = false;
        leafParticles = new List<GameObject>();

        chunkCoordX = (transform.position.x + 0.5f) / chunkWidth;
        chunkCoordY = 0f;

        if (chunkCoordX == 4)
        {
            spawnNPC = true;
        }

        chunkArray = new int[chunkWidth, chunkHeight + 64];
        chunkArray2 = new int[chunkWidth, chunkHeight + 64];

        chunkUpdaters = new List<ChunkUpdater>();

        chests = new List<Chest>();

        this.gameObject.name = "Chunk" + chunkCoordX;

        structureLeftIdVar = -1;
        structureLeftColumnVar = 0;
        structureLeftYVar = 0;
        treeHeightVar = 0;

        structureStop = 0;

        lights = new List<GameObject>();

        oldTime = 0;

        remove = new List<ChunkUpdater>();
    }

    Block BlockCheck(Vector3Int coords, bool background)
    {
        Block blockCheck = null;

        foreach (Block currentBlock in dataObject.blocks)
        {
            if (background)
            {
                if (currentBlock.tiles.Contains(tilemap2.GetTile(new Vector3Int(coords.x, coords.y, coords.z))))
                {
                    blockCheck = currentBlock;

                    break;
                }
            }
            else
            {
                if (currentBlock.tiles.Contains(tilemap.GetTile(new Vector3Int(coords.x, coords.y, coords.z))))
                {
                    blockCheck = currentBlock;

                    break;
                }
            }
        }

        return blockCheck;
    }

    public void WriteStructure(int id, int startColumn, int startCoordY, int treeHeight)
    {
        int column = 0;
        for (int i = 0; i < dataObject.structures[id].data.Length; i++)
        {
            if (i - (column * dataObject.structures[id].height) >= dataObject.structures[id].height)
            {
                column++;
            }

            if (dataObject.structures[id].data[i] != 0 && column >= startColumn)
            {
                if (i - (column * dataObject.structures[id].height) < 1)
                {
                    for (int n = 0; n < 12; n++)
                    {
                        if (tilemap2.GetTile(new Vector3Int((-chunkWidth / 2) + column - startColumn, startCoordY + (-chunkHeight / 2) + (i - (column * dataObject.structures[id].height)) + dataObject.structures[id].offsetY - dataObject.structures[id].height + (treeHeight - n), 0)) == null) 
                        {
                            tilemap2.SetTile(new Vector3Int((-chunkWidth / 2) + column - startColumn, startCoordY + (-chunkHeight / 2) + (i - (column * dataObject.structures[id].height)) + dataObject.structures[id].offsetY - dataObject.structures[id].height + (treeHeight - n), 0), dataObject.blocks[dataObject.structures[id].data[i] - 1].tiles[dataObject.blocks[dataObject.structures[id].data[i] - 1].tiles.Count - 1]);
                            chunkArray2[column - startColumn, startCoordY + (i - (column * dataObject.structures[id].height)) + dataObject.structures[id].offsetY - dataObject.structures[id].height + (treeHeight - n)] = dataObject.structures[id].data[i];
                        }
                    }
                }
                else
                {
                    tilemap2.SetTile(new Vector3Int((-chunkWidth / 2) + column - startColumn, startCoordY + (-chunkHeight / 2) + (i - (column * dataObject.structures[id].height)) + dataObject.structures[id].offsetY - dataObject.structures[id].height + treeHeight, 0), dataObject.blocks[dataObject.structures[id].data[i] - 1].tiles[dataObject.blocks[dataObject.structures[id].data[i] - 1].tiles.Count - 1]);
                    chunkArray2[column - startColumn, startCoordY + (i - (column * dataObject.structures[id].height)) + dataObject.structures[id].offsetY - dataObject.structures[id].height + treeHeight] = dataObject.structures[id].data[i];
                }
            }
        }

        structureLeftIdVar = -1;
        structureLeftColumnVar = 0;
        structureLeftYVar = 0;
        treeHeightVar = 0;

        structureStop = 0;
    }

    public void Load()
    {
        //get the data from the save file
        chunkUpdaters = chunkData.chunkUpdaters;
        chests = chunkData.chests;
        chunkArray = chunkData.chunkArray;
        chunkArray2 = chunkData.chunkArray2;
        biome = chunkData.biome;
        biomeLeft = chunkData.biomeLeft;
        addHeight = chunkData.addHeight;
        biomeHeight = chunkData.biomeHeight;
        biomeHeight2 = chunkData.biomeHeight2;
        int column = chunkData.column;
        int startCoordY = chunkData.startCoordY;
        int treeHeight = chunkData.treeHeight;

        structureLeftIdVar = chunkData.structureType;
        
        structureLeftColumnVar = column;
        structureLeftYVar = startCoordY;
        treeHeightVar = treeHeight;

        //spawn in entities
        foreach (Entity currentEntity in chunkData.entities)
        {
            GameObject newEntity = null;
            if (currentEntity != null)
            {
                if (currentEntity.type == 0)
                {
                    //note
                    newEntity = Instantiate(dataObject.entities[8], new Vector3(currentEntity.x, currentEntity.y, currentEntity.z), Quaternion.identity);
                    newEntity.GetComponent<NoteLogic>().generation = generatorObject;
                }
                else if (currentEntity.type == 1)
                {
                    //item
                    newEntity = Instantiate(dataObject.entities[7], new Vector3(currentEntity.x, currentEntity.y, currentEntity.z), Quaternion.identity);
                    newEntity.GetComponent<DroppedItem>().itemId = currentEntity.stack.itemType;
                    newEntity.GetComponent<DroppedItem>().hotbar = player.GetComponent<Hotbar>();
                    newEntity.GetComponent<DroppedItem>().inventory = player.GetComponent<OpenMenu>();

                    newEntity.GetComponent<SpriteRenderer>().sprite = dataObject.items[newEntity.GetComponent<DroppedItem>().itemId].sprite;
                }
                else if (currentEntity.type == 2)
                {
                    //balloon
                    newEntity = Instantiate(dataObject.entities[11], new Vector3(currentEntity.x, currentEntity.y, currentEntity.z), Quaternion.identity);
                }
                else if (currentEntity.type == 3)
                {
                    //soul
                    newEntity = Instantiate(dataObject.entities[4], new Vector3(currentEntity.x, currentEntity.y, currentEntity.z), Quaternion.identity);
                }
                else if (currentEntity.type == 4)
                {
                    //spirit
                    newEntity = Instantiate(dataObject.entities[5], new Vector3(currentEntity.x, currentEntity.y, currentEntity.z), Quaternion.identity);
                }
                else if (currentEntity.type == 5)
                {
                    //wyvern (boss)
                    newEntity = Instantiate(dataObject.entities[6], new Vector3(currentEntity.x, currentEntity.y, currentEntity.z), Quaternion.identity);
                }
                else if (currentEntity.type == 6)
                {
                    //night buttefly
                    newEntity = Instantiate(dataObject.entities[1], new Vector3(currentEntity.x, currentEntity.y, currentEntity.z), Quaternion.identity);
                }
                else if (currentEntity.type == 7)
                {
                    //glow buttefly
                    newEntity = Instantiate(dataObject.entities[2], new Vector3(currentEntity.x, currentEntity.y, currentEntity.z), Quaternion.identity);
                }
                else if (currentEntity.type == 8)
                {
                    //sky buttefly
                    newEntity = Instantiate(dataObject.entities[3], new Vector3(currentEntity.x, currentEntity.y, currentEntity.z), Quaternion.identity);
                }
                else if (currentEntity.type == 9)
                {
                    //bee
                    newEntity = Instantiate(dataObject.entities[0], new Vector3(currentEntity.x, currentEntity.y, currentEntity.z), Quaternion.identity);
                }
                else if (currentEntity.type == 10)
                {
                    //arrow (do nothing)
                }
                else if (currentEntity.type == 11)
                {
                    //bolt (do nothing)
                }
                else if (currentEntity.type == 12)
                {
                    //NPC
                    newEntity = Instantiate(dataObject.entities[12], new Vector3(currentEntity.x, currentEntity.y, currentEntity.z), Quaternion.identity);
                }
            }

            entityScript.GetComponent<Entities>().entities.Add(newEntity);
        }

        for (int x = 0; x < chunkWidth; x++)
        {
            for (int y = 0; y < chunkHeight + 64; y++)
            {
                if (chunkArray[x, y] != 0)
                {
                    tilemap.SetTile(new Vector3Int(x + (-32 / 2), y + (-128 / 2), 0), dataObject.blocks[chunkArray[x, y] - 1].tiles[dataObject.blocks[chunkArray[x, y] - 1].tiles.Count - 1]);
                    if (dataObject.blocks[chunkArray[x, y] - 1].lightPrefab != null)
                    {
                        GameObject newLight = Instantiate(dataObject.blocks[chunkArray[x, y] - 1].lightPrefab, transform.position, Quaternion.identity);
                        newLight.transform.SetParent(transform);
                        newLight.transform.localPosition = new Vector3Int(x + (-32 / 2), y + (-128 / 2), 0);
                        newLight.name = $"{new Vector3Int(x + (-32 / 2), y + (-128 / 2), 0)}";
                        lights.Add(newLight);
                    }
                }

                if (chunkArray2[x, y] != 0)
                {
                    tilemap2.SetTile(new Vector3Int(x + (-32 / 2), y + (-128 / 2), 0), dataObject.blocks[chunkArray2[x, y] - 1].tiles[dataObject.blocks[chunkArray2[x, y] - 1].tiles.Count - 1]);
                    if (dataObject.blocks[chunkArray2[x, y] - 1].lightPrefab != null)
                    {
                        GameObject newLight = Instantiate(dataObject.blocks[chunkArray2[x, y] - 1].lightPrefab, transform.position, Quaternion.identity);
                        newLight.transform.SetParent(transform);
                        newLight.transform.localPosition = new Vector3Int(x + (-32 / 2), y + (-128 / 2), 0);
                        newLight.name = $"{new Vector3Int(x + (-32 / 2), y + (-128 / 2), 0)}";
                        lights.Add(newLight);
                    }
                }
            }
        }

        foreach (ChunkUpdater currentUpdater in chunkUpdaters)
        {
            if (!currentUpdater.background)
            {
                tilemap.SetTile(new Vector3Int(currentUpdater.coordX + (-chunkWidth / 2), currentUpdater.coordY + (-chunkHeight / 2), 0), dataObject.blocks[currentUpdater.blockType].tiles[currentUpdater.currentStage]);
            }
            else
            {
                tilemap2.SetTile(new Vector3Int(currentUpdater.coordX + (-chunkWidth / 2), currentUpdater.coordY + (-chunkHeight / 2), 0), dataObject.blocks[currentUpdater.blockType].tiles[currentUpdater.currentStage]);
            }
        }

        for (int _x = 0; _x < chunkWidth; _x++)
        {
            for (int _y = 0; _y < chunkHeight; _y++)
            {
                if (_y > 1 && chunkArray2[_x, _y] != 0)
                {
                    if (dataObject.blocks[chunkArray2[_x, _y] - 1].tags.Contains("Leaf") && dataObject.blocks[chunkArray2[_x, _y - 1] - 1].obstructable)
                    {
                        GameObject newParticle = Instantiate(leafParticle, new Vector3Int(_x + (-chunkWidth / 2) + (int)chunkCoordX * 32, _y + (-chunkHeight / 2), 0), leafParticle.transform.rotation);
                        newParticle.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, tilemap2.GetSprite(new Vector3Int(_x + (-chunkWidth / 2), _y + (-chunkHeight / 2), 0)));
                        newParticle.name = new Vector3Int(_x + (-chunkWidth / 2) + (int)chunkCoordX * 32, _y + (-chunkHeight / 2), 0).ToString();
                        leafParticles.Add(newParticle);
                    }
                    else if (dataObject.blocks[chunkArray2[_x, _y] - 1].tags.Contains("Leaf") && chunkArray2[_x, _y - 1] == 0)
                    {
                        GameObject newParticle = Instantiate(leafParticle, new Vector3Int(_x + (-chunkWidth / 2) + (int)chunkCoordX * 32, _y + (-chunkHeight / 2), 0), leafParticle.transform.rotation);
                        newParticle.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, tilemap2.GetSprite(new Vector3Int(_x + (-chunkWidth / 2), _y + (-chunkHeight / 2), 0)));
                        newParticle.name = new Vector3Int(_x + (-chunkWidth / 2) + (int)chunkCoordX * 32, _y + (-chunkHeight / 2), 0).ToString();
                        leafParticles.Add(newParticle);
                    }
                }
            }
        }
    }

    void Update()
    {
        chunkData.chunkUpdaters = chunkUpdaters;
        chunkData.chests = chests;
        chunkData.chunkArray = chunkArray;
        chunkData.chunkArray2 = chunkArray2;
        chunkData.biome = biome;
        chunkData.biomeLeft = biomeLeft;
        chunkData.addHeight = addHeight;

        chunkData.entities.Clear();
        foreach (GameObject currentEntity in entities)
        {
            if (currentEntity != null)
            {
                Entity entityToAdd = new Entity();
                entityToAdd.x = currentEntity.transform.position.x;
                entityToAdd.y = currentEntity.transform.position.y;
                entityToAdd.z = currentEntity.transform.position.z;
                if (currentEntity.GetComponent<DroppedItem>() != null)
                {
                    entityToAdd.type = 1;

                    Stack newStack = new Stack();
                    newStack.i = 0;
                    newStack.count = 1;
                    newStack.itemType = currentEntity.GetComponent<DroppedItem>().itemId;

                    entityToAdd.stack = newStack;
                }

                if (currentEntity.GetComponent<NoteLogic>() != null)
                {
                    entityToAdd.type = 0;
                }

                if (currentEntity.GetComponent<Airship>() != null)
                {
                    entityToAdd.type = 2;
                }

                if (currentEntity.GetComponent<Soul>() != null)
                {
                    entityToAdd.type = 3;
                }

                if (currentEntity.GetComponent<Spirit>() != null)
                {
                    entityToAdd.type = 4;
                }

                if (currentEntity.GetComponent<Wyvern>() != null)
                {
                    entityToAdd.type = 5;
                }

                if (currentEntity.name == "NightButterfly(Clone)")
                {
                    entityToAdd.type = 6;
                }

                if (currentEntity.name == "GlowButterfly(Clone)")
                {
                    entityToAdd.type = 7;
                }

                if (currentEntity.name == "SkyButterfly(Clone)")
                {
                    entityToAdd.type = 8;
                }

                if (currentEntity.GetComponent<Bee>() != null)
                {
                    entityToAdd.type = 9;
                }

                if (currentEntity.GetComponent<Arrow>() != null)
                {
                    entityToAdd.type = 10;
                }

                if (currentEntity.GetComponent<NPC>() != null)
                {
                    entityToAdd.type = 12;
                }

                chunkData.entities.Add(entityToAdd);
            }
        }

        if (player.transform.position.x + distanceThreshold < transform.position.x || player.transform.position.x - distanceThreshold > transform.position.x)
        {
            if (!unloaded)
            {
                UnLoad(GameObject.Find("UIManager").GetComponent<UIManager>(), this);
                generatorObject.GetComponent<Generation>().chunksInWorld.Remove(Mathf.RoundToInt(chunkCoordX));
            }
        }

        foreach (GameObject currentLight in lights)
        {
            if (player.GetComponent<PlayerMove>().isDay == false)
            {
                currentLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = 1.5f;
            }
            else
            {
                currentLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = 0.9f;
            }
        }

        foreach (ChunkUpdater currentUpdater in remove)
        {
            chunkUpdaters.Remove(currentUpdater);
        }
        remove.Clear();

        if (oldTime != player.GetComponent<PlayerMove>().time)
        {
            foreach (ChunkUpdater currentUpdater in chunkUpdaters)
            {
                currentUpdater.time++;

                if (currentUpdater.time >= dataObject.blocks[currentUpdater.blockType].updater.intervalSpeed && dataObject.blocks[currentUpdater.blockType].updater.intervalSpeed != 0)
                {
                    currentUpdater.time = 0;
                    
                    if (currentUpdater.currentStage < dataObject.blocks[currentUpdater.blockType].tiles.Count - 1)
                    {
                        currentUpdater.currentStage++;

                        if (!currentUpdater.background && tilemap.GetTile(new Vector3Int(currentUpdater.coordX + (-chunkWidth / 2), currentUpdater.coordY + (-chunkHeight / 2), 0)) == dataObject.blocks[currentUpdater.blockType].tiles[currentUpdater.currentStage - 1])
                        {
                            tilemap.SetTile(new Vector3Int(currentUpdater.coordX + (-chunkWidth / 2), currentUpdater.coordY + (-chunkHeight / 2), 0), dataObject.blocks[currentUpdater.blockType].tiles[currentUpdater.currentStage]);
                        }
                        else if (currentUpdater.background && tilemap2.GetTile(new Vector3Int(currentUpdater.coordX + (-chunkWidth / 2), currentUpdater.coordY + (-chunkHeight / 2), 0)) == dataObject.blocks[currentUpdater.blockType].tiles[currentUpdater.currentStage - 1])
                        {
                            tilemap2.SetTile(new Vector3Int(currentUpdater.coordX + (-chunkWidth / 2), currentUpdater.coordY + (-chunkHeight / 2), 0), dataObject.blocks[currentUpdater.blockType].tiles[currentUpdater.currentStage]);
                        }
                        else
                        {
                            remove.Add(currentUpdater);
                        }
                    }
                }
            }

            oldTime = player.GetComponent<PlayerMove>().time;
        }
    }

    public async void UnLoad(UIManager ui, ChunkGeneration chunkToUnload)
    {
        unloaded = true;
        foreach (GameObject currentParticle in leafParticles)
        {
            Destroy(currentParticle);
        }
        leafParticles.Clear();

        generatorObject.cac.transform.parent = null;
        generatorObject.cursor.transform.parent = null;

        chunkData.entities.Clear();
        foreach (GameObject currentEntity in entities)
        {
            if (currentEntity != null)
            {
                Entity entityToAdd = new Entity();
                entityToAdd.x = currentEntity.transform.position.x;
                entityToAdd.y = currentEntity.transform.position.y;
                entityToAdd.z = currentEntity.transform.position.z;
                if (currentEntity.GetComponent<DroppedItem>() != null)
                {
                    entityToAdd.type = 1;

                    Stack newStack = new Stack();
                    newStack.i = 0;
                    newStack.count = 1;
                    newStack.itemType = currentEntity.GetComponent<DroppedItem>().itemId;

                    entityToAdd.stack = newStack;
                }

                if (currentEntity.GetComponent<NoteLogic>() != null)
                {
                    entityToAdd.type = 0;
                }

                if (currentEntity.GetComponent<Airship>() != null && currentEntity.transform.GetChild(0).childCount < 1)
                {
                    entityToAdd.type = 2;
                }
                else if (currentEntity.GetComponent<Airship>() != null && currentEntity.transform.GetChild(0).childCount >= 1)
                {
                    entityToAdd.type = -1;
                    currentEntity.transform.GetChild(0).GetChild(0).SetParent(null);
                }

                if (currentEntity.GetComponent<Soul>() != null)
                {
                    entityToAdd.type = 3;
                }

                if (currentEntity.GetComponent<Spirit>() != null)
                {
                    entityToAdd.type = 4;
                }

                if (currentEntity.GetComponent<Wyvern>() != null)
                {
                    entityToAdd.type = 5;
                }

                if (currentEntity.name == "NightButterfly(Clone)")
                {
                    entityToAdd.type = 6;
                }

                if (currentEntity.name == "GlowButterfly(Clone)")
                {
                    entityToAdd.type = 7;
                }

                if (currentEntity.name == "SkyButterfly(Clone)")
                {
                    entityToAdd.type = 8;
                }

                if (currentEntity.GetComponent<Bee>() != null)
                {
                    entityToAdd.type = 9;
                }

                if (currentEntity.GetComponent<Arrow>() != null)
                {
                    entityToAdd.type = 10;
                }

                if (currentEntity.GetComponent<NPC>() != null)
                {
                    entityToAdd.type = 12;
                }

                chunkData.entities.Add(entityToAdd);
            }
        }

        chunkData.structureType = structureLeftIdVar;

        await Task.Run(() => SaveSystem.SaveChunk(Mathf.RoundToInt(chunkCoordX), chunkToUnload, ui));
        foreach (GameObject currentEntity in entities)
        {
            entityScript.GetComponent<Entities>().remove.Add(currentEntity);
            Destroy(currentEntity);
        }
        if (this != null)
        {
            Destroy(this.gameObject);
        }
    }

    public async void Generate()
    {
        Chunk newChunk = new Chunk();
        int left;
        int right;

        if (GameObject.Find("Chunk" + (chunkCoordX - 1)) != null)
        {
            left = GameObject.Find("Chunk" + (chunkCoordX - 1)).GetComponent<ChunkGeneration>().biome;
        }
        else
        {
            left = -1;
        }

        if (GameObject.Find("Chunk" + (chunkCoordX + 1)) != null)
        {
            right = GameObject.Find("Chunk" + (chunkCoordX + 1)).GetComponent<ChunkGeneration>().biome;
        }
        else
        {
            right = -1;
        }

        newChunk = await Task.Run(() => GenerateChunk(left, right));

        List<string> noteNames = new List<string>();

        foreach (Note currentNote in dataObject.notes)
        {
            if (!generatorObject.foundNotes.Contains(currentNote.name))
            {
                noteNames.Add(currentNote.name);
            }
        }
        if (noteNames.Count > 0)
        {
            Debug.Log("A note has been spawned!");

            GameObject note = Instantiate(dataObject.entities[8], new Vector3Int(Mathf.RoundToInt(transform.position.x) + newChunk.noteX + (-chunkWidth / 2), Mathf.RoundToInt(transform.position.y) + newChunk.noteY + (-chunkHeight / 2) + 1, Mathf.RoundToInt(transform.position.z) + 0), Quaternion.identity);
            note.GetComponent<NoteLogic>().generation = generatorObject;

            //right now this is purely random, find a way to make it depend on the seed instead
            note.GetComponent<NoteLogic>().noteName = noteNames[UnityEngine.Random.Range(0, noteNames.Count - 1)];

            entities.Add(note);
            generatorObject.GetComponent<Generation>().entityScript.GetComponent<Entities>().entities.Add(note);

            spawnNote = false;
        }

        if (newChunk.spawnNPC)
        {
            GameObject NPC = Instantiate(dataObject.entities[12], new Vector3(newChunk.xNPC + (-chunkWidth / 2) + chunkCoordX * 32, newChunk.yNPC + (-chunkHeight / 2) + 1, 0), Quaternion.identity);
            entities.Add(NPC);
            generatorObject.gameObject.GetComponent<Generation>().entityScript.GetComponent<Entities>().entities.Add(NPC);
        }

        for(int x = 0; x < chunkWidth; x++)
        {
            for(int y = 0; y < chunkHeight; y++)
            {
                if (newChunk.front[x, y] != 0)
                {
                    tilemap.SetTile(new Vector3Int(x + (-chunkWidth / 2), y + (-chunkHeight / 2), 0), dataObject.blocks[newChunk.front[x, y] - 1].tiles[newChunk.frontId[x, y]]);
                }

                if (newChunk.back[x, y] != 0)
                {
                    tilemap2.SetTile(new Vector3Int(x + (-chunkWidth / 2), y + (-chunkHeight / 2), 0), dataObject.blocks[newChunk.back[x, y] - 1].tiles[newChunk.backId[x, y]]);
                }

                if (newChunk.front[x, y] > 0)
                {
                    if (dataObject.blocks[newChunk.front[x, y] - 1].lightPrefab != null)
                    {
                        GameObject newLight = Instantiate(dataObject.blocks[newChunk.front[x, y] - 1].lightPrefab, transform.position, Quaternion.identity);
                        newLight.transform.SetParent(transform);
                        newLight.transform.localPosition = new Vector3Int(x + (-32 / 2), y + (-128 / 2), 0);
                        newLight.name = $"{new Vector3Int(x + (-32 / 2), y + (-128 / 2), 0)}";
                        lights.Add(newLight);
                    }
                }

                if (newChunk.back[x, y] > 0)
                {
                    if (dataObject.blocks[newChunk.back[x, y] - 1].lightPrefab != null)
                    {
                        GameObject newLight = Instantiate(dataObject.blocks[newChunk.back[x, y] - 1].lightPrefab, transform.position, Quaternion.identity);
                        newLight.transform.SetParent(transform);
                        newLight.transform.localPosition = new Vector3Int(x + (-32 / 2), y + (-128 / 2), 0);
                        newLight.name = $"{new Vector3Int(x + (-32 / 2), y + (-128 / 2), 0)}";
                        lights.Add(newLight);
                    }
                }
            }
        }

        foreach (Vector2Int currentPosition in newChunk.particles)
        {
            GameObject newParticle = Instantiate(leafParticle, new Vector3Int(currentPosition.x + (-chunkWidth / 2) + (int)chunkCoordX * 32, currentPosition.y + (-chunkHeight / 2), 0), leafParticle.transform.rotation);
            newParticle.GetComponent<ParticleSystem>().textureSheetAnimation.SetSprite(0, tilemap2.GetSprite(new Vector3Int(currentPosition.x + (-chunkWidth / 2), currentPosition.y + (-chunkHeight / 2), 0)));
            newParticle.name = new Vector3Int(currentPosition.x + (-chunkWidth / 2) + (int)chunkCoordX * 32, currentPosition.y + (-chunkHeight / 2), 0).ToString();
            leafParticles.Add(newParticle);
        }
    }

    Chunk GenerateChunk(int leftChunk, int rightChunk)
    {
        Chunk returnChunk = new Chunk();
        returnChunk.front = new int[chunkWidth, chunkHeight];
        returnChunk.frontId = new int[chunkWidth, chunkHeight];
        returnChunk.back = new int[chunkWidth, chunkHeight];
        returnChunk.backId = new int[chunkWidth, chunkHeight];
        returnChunk.particles = new List<Vector2Int>();

        for (int x = 0; x < chunkWidth; x++)
        {
            //advanced perlin noise
            System.Random pRandom = new System.Random(Mathf.RoundToInt(seed) + Mathf.RoundToInt(chunkCoordX * 32 + x));

            int smoothPerlin = Mathf.RoundToInt(Mathf.PerlinNoise((chunkCoordX * 32 + x) / dataObject.biomes[biome].smoothMap, seed) * chunkHeight / 2);
            int roughPerlin = Mathf.RoundToInt(Mathf.PerlinNoise((chunkCoordX * 32 + x) / dataObject.biomes[biome].roughMap, seed) * chunkHeight / 2);

            float blendPerlin = Mathf.PerlinNoise((chunkCoordX * 32 + x) / dataObject.biomes[biome].blendMap, seed);

            float dif0 = blendPerlin;
            float dif1 = 1 - blendPerlin;

            int perlinHeight = Mathf.RoundToInt(smoothPerlin * dif0 + roughPerlin * dif1);
            perlinHeight += chunkHeight / 2;

            //make sure the chunk matches with the other chunks
            if (generatorObject.right)
            {
                if (chunkCoordX > 2 || chunkCoordX < -2)
                {
                    int smoothPerlin_ = Mathf.RoundToInt(Mathf.PerlinNoise((chunkCoordX * 32 + 0) / dataObject.biomes[biome].smoothMap, seed) * chunkHeight / 2);
                    int roughPerlin_ = Mathf.RoundToInt(Mathf.PerlinNoise((chunkCoordX * 32 + 0) / dataObject.biomes[biome].roughMap, seed) * chunkHeight / 2);

                    float blendPerlin_ = Mathf.PerlinNoise((chunkCoordX * 32 + 0) / dataObject.biomes[biome].blendMap, seed);

                    float dif0_ = blendPerlin_;
                    float dif1_ = 1 - blendPerlin_;

                    int perlinHeight_ = Mathf.RoundToInt(smoothPerlin_ * dif0_ + roughPerlin_ * dif1_);

                    addHeight = perlinHeight_ + chunkHeight / 2 - generatorObject.biomeHeight;

                    addHeight += worldModifier;
                }
            }
            else if (!generatorObject.right)
            {
                if (chunkCoordX > 2 || chunkCoordX < -2)
                {
                    int _smoothPerlin = Mathf.RoundToInt(Mathf.PerlinNoise((chunkCoordX * 32 + chunkWidth - 1) / dataObject.biomes[biome].smoothMap, seed) * chunkHeight / 2);
                    int _roughPerlin = Mathf.RoundToInt(Mathf.PerlinNoise((chunkCoordX * 32 + chunkWidth - 1) / dataObject.biomes[biome].roughMap, seed) * chunkHeight / 2);

                    float _blendPerlin = Mathf.PerlinNoise((chunkCoordX * 32 + chunkWidth - 1) / dataObject.biomes[biome].blendMap, seed);

                    float _dif0 = _blendPerlin;
                    float _dif1 = 1 - _blendPerlin;

                    int _perlinHeight = Mathf.RoundToInt(_smoothPerlin * _dif0 + _roughPerlin * _dif1);

                    addHeight = _perlinHeight + chunkHeight / 2 - generatorObject.biomeHeight2;

                    addHeight += worldModifier;
                }
            }

            perlinHeight -= addHeight;
            perlinHeight += worldModifier;

            //terrain
            for (int y = 0; y <= perlinHeight + 1; y++)
            {
                if (y == perlinHeight && spawnNote)
                {
                    returnChunk.noteX = x;
                    returnChunk.noteY = y;
                }

                Biome referenceBiome = dataObject.biomes[biome];

                //underground
                if (referenceBiome.surfaceThickness > 1 && y > perlinHeight - (referenceBiome.surfaceThickness / 2 + 1 + pRandom.Next(1, referenceBiome.surfaceThickness)) && y < perlinHeight)
                {
                    //surface
                    currentTile = referenceBiome.blocks[2 - 1].id;
                    currentTile2 = referenceBiome.blocks[2 - 1].id;
                }
                else if (y > perlinHeight - (referenceBiome.surfaceThickness + 1) && y < perlinHeight)
                {
                    //surface
                    currentTile = referenceBiome.blocks[2 - 1].id;
                    currentTile2 = referenceBiome.blocks[2 - 1].id;
                }
                else if (y < perlinHeight)
                {
                    //generate "dirt layer"
                    currentTile = referenceBiome.blocks[1 - 1].id;
                    currentTile2 = referenceBiome.blocks[1 - 1].id;
                }
                else
                {
                    //air
                    currentTile = 0;
                    currentTile2 = 0;
                }

                if (x == chunkWidth - 1 && y == perlinHeight)
                {
                    chunkData.biomeHeight = y;
                    biomeHeight = y;
                }

                if (x == 0 && y == perlinHeight)
                {
                    chunkData.biomeHeight2 = y;
                    biomeHeight2 = y;
                }

                //underground (ores, etc.)

                if (y < perlinHeight)
                {
                    if (pRandom.Next(1, 14) > 12 && y < perlinHeight - 8 && y > perlinHeight - (referenceBiome.stoneThreshold + worldModifier / 2) + 4)
                    {
                        //second ore
                        currentTile = referenceBiome.blocks[6 - 1].id;
                        currentTile2 = referenceBiome.blocks[6 - 1].id;
                    }
                }

                if (y > -1 && y < perlinHeight - (referenceBiome.stoneThreshold + worldModifier / 2) + pRandom.Next(-5, 5))
                {
                    int oreChance = pRandom.Next(1, 100);

                    if (y < 1 + pRandom.Next(1, 10))
                    {
                        //bedrock
                        currentTile = referenceBiome.blocks[5 - 1].id;
                        currentTile2 = referenceBiome.blocks[5 - 1].id;
                    }
                    else if (oreChance < 5)
                    {
                        if (pRandom.Next(1, 20) > 9)
                        {
                            //first ore
                            currentTile = referenceBiome.blocks[4 - 1].id;
                            currentTile2 = referenceBiome.blocks[4 - 1].id;
                        }
                        else
                        {
                            //third ore
                            currentTile = referenceBiome.blocks[7 - 1].id;
                            currentTile2 = referenceBiome.blocks[7 - 1].id;
                        }
                    }
                    else
                    {
                        //generate "stone layer"
                        currentTile = referenceBiome.blocks[3 - 1].id;
                        currentTile2 = referenceBiome.blocks[3 - 1].id;

                        if (1 == -1)
                        {
                            if (leftChunk != -1 && leftChunk != biome && x < 1 + pRandom.Next(1, 10))
                            {
                                //biome blending to the left
                                currentTile = GameObject.Find("Chunk" + (chunkCoordX - 1)).GetComponent<ChunkGeneration>().chunkArray[31, y];
                                currentTile2 = GameObject.Find("Chunk" + (chunkCoordX - 1)).GetComponent<ChunkGeneration>().chunkArray2[31, y];
                            }
                            else if (rightChunk != -1 && rightChunk != biome && x > 30 - pRandom.Next(1, 10))
                            {
                                //biome blending to the right
                                currentTile = GameObject.Find("Chunk" + (chunkCoordX + 1)).GetComponent<ChunkGeneration>().chunkArray[0, y];
                                currentTile2 = GameObject.Find("Chunk" + (chunkCoordX + 1)).GetComponent<ChunkGeneration>().chunkArray2[0, y];
                            }
                        }
                    }
                }

                //generates flowers and plants
                if (y == perlinHeight - 1 && referenceBiome.foliage.Length > 0)
                {
                    if (spawnNPC)
                    {
                        returnChunk.xNPC = x;
                        returnChunk.yNPC = y + 1;
                        returnChunk.spawnNPC = true;
                        spawnNPC = false;
                    }

                    int flowerType = pRandom.Next(1, 100);
                    int foliageId = 0;
                    float floor = 0;

                    for (int i = 0; i < referenceBiome.foliage.Length; i++)
                    {
                        if (flowerType > floor && flowerType < floor + referenceBiome.foliage[i].weight * 100)
                        {
                            foliageTile = referenceBiome.foliage[i].id;
                            foliageId = i;
                            break;
                        }
                        else
                        {
                            floor += referenceBiome.foliage[i].weight * 100;
                        }
                    }

                    if (foliageTile != 0)
                    {
                        //normally there is butterfly code here but I have deleted it to keep things simple

                        if (returnChunk.front[x, y] == 0)
                        {
                            if (!referenceBiome.foliage[foliageId].single)
                            {
                                int tileId = pRandom.Next(1, 100);
                                for (int i = 0; i < dataObject.blocks[foliageTile - 1].tiles.Count; i++)
                                {
                                    if (tileId < 100 / dataObject.blocks[foliageTile - 1].tiles.Count * (i + 1))
                                    {
                                        returnChunk.front[x, y + 1] = foliageTile;
                                        returnChunk.frontId[x, y + 1] = i;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                returnChunk.front[x, y + 1] = foliageTile;
                                returnChunk.frontId[x, y + 1] = dataObject.blocks[foliageTile - 1].updater.spawnStage;
                            }

                            chunkArray[x, y + 1] = foliageTile;

                            if (dataObject.blocks[foliageTile - 1].tiles.Count > 1)
                            {
                                ChunkUpdater newUpdater = new ChunkUpdater();

                                newUpdater.blockType = foliageTile - 1;
                                newUpdater.currentStage = returnChunk.frontId[x, y + 1];

                                newUpdater.coordX = x;
                                newUpdater.coordY = y + 1;

                                newUpdater.background = false;

                                newUpdater.time = 0;

                                chunkUpdaters.Add(newUpdater);
                            }
                        }
                    }
                }

                //generates the actual visual tiles
                if (currentTile != 0)
                {
                    if (dataObject.blocks[currentTile - 1].updater.spawnStage != -1)
                    {
                        returnChunk.front[x, y] = currentTile;
                        returnChunk.frontId[x, y] = dataObject.blocks[currentTile - 1].updater.spawnStage;
                    }
                    else
                    {
                        returnChunk.front[x, y] = currentTile;
                        returnChunk.frontId[x, y] = dataObject.blocks[currentTile - 1].tiles.Count - 1;
                    }

                    chunkArray[x, y] = currentTile;

                    if (dataObject.blocks[currentTile - 1].tiles.Count > 1)
                    {
                        ChunkUpdater newUpdater = new ChunkUpdater();

                        newUpdater.blockType = currentTile - 1;
                        newUpdater.currentStage = 0;

                        newUpdater.coordX = x;
                        newUpdater.coordY = y;

                        newUpdater.background = false;

                        newUpdater.time = 0;

                        chunkUpdaters.Add(newUpdater);
                    }
                }

                if (currentTile2 != 0)
                {
                    if (dataObject.blocks[currentTile2 - 1].updater.spawnStage != -1)
                    {
                        returnChunk.back[x, y] = currentTile2;
                        returnChunk.backId[x, y] = dataObject.blocks[currentTile2 - 1].updater.spawnStage;
                    }
                    else
                    {
                        returnChunk.back[x, y] = currentTile2;
                        returnChunk.backId[x, y] = dataObject.blocks[currentTile2 - 1].tiles.Count - 1;
                    }

                    chunkArray2[x, y] = currentTile2;

                    if (dataObject.blocks[currentTile2 - 1].tiles.Count > 1)
                    {
                        ChunkUpdater newUpdater = new ChunkUpdater();

                        newUpdater.blockType = currentTile2 - 1;
                        newUpdater.currentStage = 0;

                        newUpdater.coordX = x;
                        newUpdater.coordY = y;

                        newUpdater.background = true;

                        newUpdater.time = 0;

                        chunkUpdaters.Add(newUpdater);
                    }
                }
            }
        }

        //checking if the biome we're in has any structures to generate
        if (dataObject.biomes[biome].structures.Length > 0)
        {
            //looping through all the blocks to generate structures
            for (int x = 0; x < chunkWidth; x++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    //get some random numbers ready for later, keep in mind that these are based on the world seed
                    System.Random pRandom = new System.Random(Mathf.RoundToInt(seed) + Mathf.RoundToInt(chunkCoordX * 32 + x));

                    //check if the current block is an air block with a block below it, in other words check if the structure will be placed on the surface
                    if (returnChunk.back[x, y] == 0 && returnChunk.back[x, y - 1] == dataObject.biomes[biome].blocks[1].id)
                    {
                        //eventually I need to add a weights system so that certain structures will be rarer than others
                        StructureReference structureType = dataObject.biomes[biome].structures[pRandom.Next(0, dataObject.biomes[biome].structures.Length)];

                        //scanning the area to make sure there is room for the structure to generate

                        //keeping track of what column of the structure we are in
                        int column = 0;

                        //a variable that tells the script whether or not to go ahead and spawn in the structure
                        bool generate = true;

                        //adding height variation to the structures, only used for trees pretty much
                        int treeHeight = pRandom.Next(1, 6);

                        //if the structure isn't a tree, don't do the height variation
                        if (dataObject.structures[structureType.id].noTreeheight)
                        {
                            treeHeight = 0;
                        }

                        structureStop = 0;
                        overChunkBorder = false;
                        for (int i = 0; i < dataObject.structures[structureType.id].data.Length; i++)
                        {
                            //keeping track of where we are on the y-axis
                            startCoordY = y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + treeHeight;

                            //advace to the next column in the structure
                            if (i - (column * dataObject.structures[structureType.id].height) >= dataObject.structures[structureType.id].height)
                            {
                                column++;
                            }

                            //does the location of our structure go past the left border of the chunk? if so, don't spawn it there
                            if (x + column + dataObject.structures[structureType.id].offsetX > -1)
                            {
                                //does the location of our structure go past the right border of the chunk? if so, spawn half of it and tell the next chunk to finish spawning it
                                if (x + column + dataObject.structures[structureType.id].offsetX < chunkWidth)
                                {
                                    //checking to see if there are blocks in the way of our chosen location

                                    //making sure that we're not looking at an air block
                                    if (returnChunk.back[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + treeHeight] > 0)
                                    {
                                        Block blockCheck = dataObject.blocks[returnChunk.back[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + treeHeight] - 1];

                                        //if we're not forcing the structure, and there is a block in the way, then don't spawn the structure
                                        if (!dataObject.structures[structureType.id].forceGenerate && blockCheck.obstructable == false)
                                        {
                                            generate = false;
                                        }
                                    }
                                }
                                else if (!overChunkBorder)
                                {
                                    overChunkBorder = true;

                                    structureLeftIdVar = structureType.id;
                                    structureLeftColumnVar = column;
                                    structureLeftYVar = startCoordY;
                                    treeHeightVar = treeHeight;
                                    structureStop = column;

                                    chunkData.treeHeight = treeHeight;
                                    chunkData.structureType = structureType.id;
                                    chunkData.column = column;
                                    chunkData.startCoordY = startCoordY;
                                }
                            }
                            else
                            {
                                generate = false;
                            }
                        }

                        // if the background of the structure was fine, how about the foreground?
                        if (generate == true)
                        {
                            column = 0;

                            for (int i = 0; i < dataObject.structures[structureType.id].data2.Length; i++)
                            {
                                startCoordY = y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + treeHeight;

                                if (i - (column * dataObject.structures[structureType.id].height) >= dataObject.structures[structureType.id].height)
                                {
                                    column++;
                                }

                                Block blockCheck = null;
                                if (x + column + dataObject.structures[structureType.id].offsetX > -1)
                                {
                                    if (x + column + dataObject.structures[structureType.id].offsetX < chunkWidth)
                                    {
                                        if (returnChunk.front[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + treeHeight] > 0)
                                        {
                                            blockCheck = dataObject.blocks[returnChunk.front[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + treeHeight] - 1];

                                            if (!dataObject.structures[structureType.id].forceGenerate && blockCheck.obstructable == false)
                                            {
                                                generate = false;
                                            }
                                        }
                                    }
                                    else if (!overChunkBorder)
                                    {
                                        overChunkBorder = true;

                                        structureLeftIdVar = structureType.id;
                                        structureLeftColumnVar = column;
                                        structureLeftYVar = startCoordY;
                                        treeHeightVar = treeHeight;
                                        structureStop = column;

                                        chunkData.treeHeight = treeHeight;
                                        chunkData.structureType = structureType.id;
                                        chunkData.column = column;
                                        chunkData.startCoordY = startCoordY;
                                    }
                                }
                                else
                                {
                                    generate = false;
                                }
                            }
                        }

                        column = 0;

                        //make you see the actual structure
                        if (generate == true)
                        {
                            //spawns in the appropriate chest data for any structures that have chests in them
                            foreach (Chest currentChest in dataObject.structures[structureType.id].chests)
                            {
                                Chest newChest = new Chest();

                                newChest.items = currentChest.items;

                                newChest.coordX = x + dataObject.structures[structureType.id].offsetX + currentChest.coordX;
                                newChest.coordY = y + dataObject.structures[structureType.id].offsetY + currentChest.coordY + treeHeight;

                                newChest.background = currentChest.background;

                                chests.Add(newChest);
                            }

                            for (int i = 0; i < dataObject.structures[structureType.id].data.Length; i++)
                            {
                                if (i - (column * dataObject.structures[structureType.id].height) >= dataObject.structures[structureType.id].height)
                                {
                                    column++;
                                }

                                if (dataObject.structures[structureType.id].data[i] != 0)
                                {
                                    //making sure the structure conforms to the terrain
                                    if (i - (column * dataObject.structures[structureType.id].height) < 1)
                                    {
                                        for (int n = 0; n < 12; n++)
                                        {
                                            Block blockCheck = null;
                                            if (x + column + dataObject.structures[structureType.id].offsetX > -1 && y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + (treeHeight - n) > 0)
                                            {
                                                if (returnChunk.back[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + (treeHeight - n)] > 0)
                                                {
                                                    blockCheck = dataObject.blocks[returnChunk.back[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + (treeHeight - n)] - 1];
                                                }

                                                if (returnChunk.back[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + (treeHeight - n)] == 0 || blockCheck.obstructable == true)
                                                {
                                                    if (column < structureStop || structureStop == 0)
                                                    {
                                                        returnChunk.back[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + (treeHeight - n)] = dataObject.structures[structureType.id].data[i];
                                                        returnChunk.backId[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + (treeHeight - n)] = dataObject.blocks[dataObject.structures[structureType.id].data[i] - 1].tiles.Count - 1;
                                                        chunkArray2[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + (treeHeight - n)] = dataObject.structures[structureType.id].data[i];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (column < structureStop || structureStop == 0)
                                        {
                                            returnChunk.back[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + treeHeight] = dataObject.structures[structureType.id].data[i];
                                            returnChunk.backId[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + treeHeight] = dataObject.blocks[dataObject.structures[structureType.id].data[i] - 1].tiles.Count - 1;
                                            chunkArray2[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + treeHeight] = dataObject.structures[structureType.id].data[i];
                                        }
                                    }
                                }
                            }

                            column = 0;

                            for (int i = 0; i < dataObject.structures[structureType.id].data2.Length; i++)
                            {
                                if (i - (column * dataObject.structures[structureType.id].height) >= dataObject.structures[structureType.id].height)
                                {
                                    column++;
                                }

                                if (dataObject.structures[structureType.id].data2[i] != 0)
                                {
                                    //making sure the structure conforms to the terrain
                                    if (i - (column * dataObject.structures[structureType.id].height) < 1)
                                    {
                                        for (int n = 0; n < 12; n++)
                                        {
                                            Block blockCheck = null;
                                            if (x + column + dataObject.structures[structureType.id].offsetX > -1)
                                            {
                                                if (returnChunk.front[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + (treeHeight - n)] > 0)
                                                {
                                                    blockCheck = dataObject.blocks[returnChunk.front[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + (treeHeight - n)] - 1];
                                                }
                                            }

                                            if (returnChunk.front[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + (treeHeight - n)] == 0 || blockCheck.obstructable == true)
                                            {
                                                if (column < structureStop || structureStop == 0)
                                                {
                                                    returnChunk.front[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + (treeHeight - n)] = dataObject.structures[structureType.id].data2[i];
                                                    returnChunk.frontId[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + (treeHeight - n)] = dataObject.blocks[dataObject.structures[structureType.id].data2[i] - 1].tiles.Count - 1;
                                                    chunkArray[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + (treeHeight - n)] = dataObject.structures[structureType.id].data2[i];
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (column < structureStop || structureStop == 0)
                                        {
                                            returnChunk.front[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + treeHeight] = dataObject.structures[structureType.id].data2[i];
                                            returnChunk.frontId[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + treeHeight] = dataObject.blocks[dataObject.structures[structureType.id].data2[i] - 1].tiles.Count - 1;
                                            chunkArray[x + column + dataObject.structures[structureType.id].offsetX, y + (i - (column * dataObject.structures[structureType.id].height)) + dataObject.structures[structureType.id].offsetY + treeHeight] = dataObject.structures[structureType.id].data2[i];
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (y > 1 && chunkArray2[x, y] != 0)
                    {
                        if (dataObject.blocks[chunkArray2[x, y] - 1].tags.Contains("Leaf") && dataObject.blocks[chunkArray2[x, y - 1] - 1].obstructable)
                        {
                            returnChunk.particles.Add(new Vector2Int(x, y));
                        }
                        else if (dataObject.blocks[chunkArray2[x, y] - 1].tags.Contains("Leaf") && chunkArray2[x, y - 1] == 0)
                        {
                            returnChunk.particles.Add(new Vector2Int(x, y));
                        }
                    }

                    //generating caves
                    if (Mathf.PerlinNoise((chunkCoordX * 32 + x) / dataObject.biomes[biome].caveMap * caveSize, (seed + y) / dataObject.biomes[biome].caveMap * caveSize) > caveThreshold + y / falloff && y > 7 && y < 80)
                    {
                        returnChunk.front[x, y] = 0;
                        chunkArray[x, y] = 0;
                    }

                    //cave ceiling foliage
                    if (dataObject.biomes[biome].caveCeilingFoliage.Length > 0)
                    {
                        int flowerType = pRandom.Next(1, 100);
                        float floor = 0;

                        if (Mathf.PerlinNoise((chunkCoordX * 32 + x) / dataObject.biomes[biome].caveMap * caveSize, (seed + (y - 1)) / dataObject.biomes[biome].caveMap * caveSize) > caveThreshold + (y - 1) / falloff && (y - 1) > 7)
                        {
                            if (Mathf.PerlinNoise((chunkCoordX * 32 + x) / dataObject.biomes[biome].caveMap * caveSize, (seed + (y + 1)) / dataObject.biomes[biome].caveMap * caveSize) < caveThreshold + (y + 1) / falloff && (y + 1) > 7)
                            {
                                for (int i = 0; i < dataObject.biomes[biome].foliage.Length; i++)
                                {
                                    if (flowerType > floor && flowerType < floor + dataObject.biomes[biome].foliage[i].weight * 100)
                                    {
                                        foliageTile = dataObject.biomes[biome].foliage[i].id;
                                        break;
                                    }
                                    else
                                    {
                                        floor += dataObject.biomes[biome].foliage[i].weight * 100;
                                    }
                                }

                                if (foliageTile != 0)
                                {
                                    if (returnChunk.front[x, y] == 0 && returnChunk.front[x, y + 1] != 0)
                                    {
                                        int blockIndex = dataObject.blocks.IndexOf(dataObject.blocks[returnChunk.front[x, y + 1] - 1]);

                                        if (dataObject.blocks[foliageTile - 1].placeTags.Count > 0)
                                        {
                                            bool foliagePlaced = false;
                                            foreach (string currentTag in dataObject.blocks[foliageTile - 1].placeTags)
                                            {
                                                if (dataObject.blocks[blockIndex].tags.Contains(currentTag))
                                                {
                                                    foliagePlaced = true;

                                                    tilemap.SetTile(new Vector3Int(x + (-chunkWidth / 2), y + (-chunkHeight / 2), 0), dataObject.blocks[foliageTile - 1].tiles[dataObject.blocks[foliageTile - 1].tiles.Count - 1]);
                                                    chunkArray[x, y] = foliageTile;

                                                    break;
                                                }
                                            }

                                            if (!foliagePlaced)
                                            {
                                                foreach (string currentTag in dataObject.blocks[blockIndex].tags)
                                                {
                                                    foreach (FoliageReference currentFoliage in dataObject.biomes[biome].caveCeilingFoliage)
                                                    {
                                                        if (dataObject.blocks[currentFoliage.id - 1].placeTags.Contains(currentTag))
                                                        {
                                                            tilemap.SetTile(new Vector3Int(x + (-chunkWidth / 2), y + (-chunkHeight / 2), 0), dataObject.blocks[currentFoliage.id - 1].tiles[dataObject.blocks[currentFoliage.id - 1].tiles.Count - 1]);
                                                            chunkArray[x, y] = currentFoliage.id;

                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            tilemap.SetTile(new Vector3Int(x + (-chunkWidth / 2), y + (-chunkHeight / 2), 0), dataObject.blocks[foliageTile - 1].tiles[dataObject.blocks[foliageTile - 1].tiles.Count - 1]);
                                            chunkArray[x, y] = foliageTile;
                                        }
                                    }
                                }
                            }

                        }
                    }

                    //cave floor foliage
                    if (dataObject.biomes[biome].caveFloorFoliage.Length > 0)
                    {
                        int flowerType = pRandom.Next(1, 100);
                        float floor = 0;

                        if (Mathf.PerlinNoise((chunkCoordX * 32 + x) / dataObject.biomes[biome].caveMap * caveSize, (seed + (y - 1)) / dataObject.biomes[biome].caveMap * caveSize) < caveThreshold + (y - 1) / falloff && (y - 1) > 7)
                        {
                            if (Mathf.PerlinNoise((chunkCoordX * 32 + x) / dataObject.biomes[biome].caveMap * caveSize, (seed + (y + 1)) / dataObject.biomes[biome].caveMap * caveSize) > caveThreshold + (y + 1) / falloff && (y + 1) > 7)
                            {
                                for (int i = 0; i < dataObject.biomes[biome].foliage.Length; i++)
                                {
                                    if (flowerType > floor && flowerType < floor + dataObject.biomes[biome].foliage[i].weight * 100)
                                    {
                                        foliageTile = dataObject.biomes[biome].foliage[i].id;
                                        break;
                                    }
                                    else
                                    {
                                        floor += dataObject.biomes[biome].foliage[i].weight * 100;
                                    }
                                }

                                if (foliageTile != 0)
                                {
                                    if (returnChunk.front[x, y] == 0 && returnChunk.front[x, y + 1] != 0)
                                    {
                                        int blockIndex = dataObject.blocks.IndexOf(dataObject.blocks[returnChunk.front[x, y - 1] - 1]);

                                        if (dataObject.blocks[foliageTile - 1].placeTags.Count > 0)
                                        {

                                            bool foliagePlaced = false;
                                            foreach (string currentTag in dataObject.blocks[foliageTile - 1].placeTags)
                                            {
                                                if (dataObject.blocks[blockIndex].tags.Contains(currentTag))
                                                {
                                                    foliagePlaced = true;

                                                    tilemap.SetTile(new Vector3Int(x + (-chunkWidth / 2), y + (-chunkHeight / 2), 0), dataObject.blocks[foliageTile - 1].tiles[dataObject.blocks[foliageTile - 1].tiles.Count - 1]);
                                                    chunkArray[x, y] = foliageTile;

                                                    break;
                                                }
                                            }

                                            if (!foliagePlaced)
                                            {
                                                foreach (string currentTag in dataObject.blocks[blockIndex].tags)
                                                {
                                                    foreach (FoliageReference currentFoliage in dataObject.biomes[biome].caveFloorFoliage)
                                                    {
                                                        if (dataObject.blocks[currentFoliage.id - 1].placeTags.Contains(currentTag))
                                                        {
                                                            tilemap.SetTile(new Vector3Int(x + (-chunkWidth / 2), y + (-chunkHeight / 2), 0), dataObject.blocks[currentFoliage.id - 1].tiles[dataObject.blocks[currentFoliage.id - 1].tiles.Count - 1]);
                                                            chunkArray[x, y] = currentFoliage.id;

                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            tilemap.SetTile(new Vector3Int(x + (-chunkWidth / 2), y + (-chunkHeight / 2), 0), dataObject.blocks[foliageTile - 1].tiles[dataObject.blocks[foliageTile - 1].tiles.Count - 1]);
                                            chunkArray[x, y] = foliageTile;
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

        return returnChunk;
    }
}

public class Chunk
{
    public int[,] front;
    public int[,] frontId;
    public int[,] back;
    public int[,] backId;

    public int noteX;
    public int noteY;

    public bool spawnNPC;
    public int xNPC;
    public int yNPC;

    public List<Vector2Int> particles;
}
