using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Generation : MonoBehaviour
{
    //some important object references
    public GameObject chunkPrefab;
    public GameObject cameraObject;
    public GameObject dataObject;
    public GameObject ui;
    public GameObject player;
    public GameObject entityScript;

    //inventory and hotbar references
    public OpenMenu inventory;
    public Hotbar hotbar;

    //the info for detecting chunks
    private bool isChunk;
    public LayerMask whatIsChunk;
    public float chunkCheckRadius;

    //all of the 'checkers' needed to see what chunk ur in, where the chunks should be generated, etc.
    public Transform leftChecker;
    public Transform rightChecker;
    public Transform biomeChecker;
    public Transform leftBiomeLeftChecker;
    public Transform rightBiomeLeftChecker;

    //info about the seed
    public string tempSeed;
    public int seed;
    public bool randomSeed;

    //related to detecting the current biome
    public int currentBiome;
    public Collider2D currentChunk;
    private Collider2D sampleBiome;

    //some settings for how the sky-changing works
    private Color currentSkyColor;
    public float skyFadeSpeed;

    //biome generation info
    public int biomeToGenerate;
    public int biomeLeft;
    public int biomeHeight;
    public int biomeHeight2;

    //information on which 'checkers' to use, changes depending where the player is (left or right from spawn)
    private Transform generateDir;
    private Transform sampleDir;

    //variable that initiates the filling of the 3 middle chunks, or 'spawn chunks'
    public bool startGen;

    //forces a certain biome to generate
    public bool forceBiome;
    public int forceBiomeId;

    //structure generation info
    public int structureLeftId;
    public int structureLeftColumn;
    public int structureLeftY;
    public int treeHeight;

    //stuff related to the cursor, and placing/breaking blocks
    public GameObject cursor;
    public Transform cac;
    public GameObject dummy;
    public GameObject dummyPrefab;
    public Collider2D cursorChunk;
    public Transform cursorChecker;

    //world and game info
    public bool begin;
    public bool right = true;
    public List<int> chunksInWorld;
    public bool respawn;

    //other misc references
    public Tile bedrockTile;
    public Tile beehiveTile;
    public Breaker breaker;

    private bool coolDown;
    public Vector3 oldPos;

    public float breakerWait;
    public GameObject breakerParticle;
    public Color breakerColor;
    public Color fadeColor;
    public bool breakerFade;
    public float breakerFadeSpeed;

    public GameObject placeParticle;
    public List<GameObject> placeParticles;
    public List<float> placeParticleTimers;

    public bool lucky;

    public List<string> foundNotes;

    public int worldModifier;

    public float lifeCount;
    public bool spectate;
    public GameObject filter;

    public int ability;

    public float breakDistance;
    public float placeDistance;

    public GameObject climbCheck;

    public TileBase chestTile;
    public TileBase[] workbenchTiles;

    public int deathCount;

    public Sprite cookingMenuSprite;

    //when loading a game
    public void StartGameLoad()
    {
        lucky = false;

        breakerParticle.SetActive(false);
        structureLeftId = -1;

        cac.transform.parent = null;
        cursor.transform.parent = null;

        //check if there is a seed to load (keep in mind the seed might for some reason not be there)
        if (dataObject.GetComponent<GameData>().worldData.worldSeed != 0)
        {
            seed = dataObject.GetComponent<GameData>().worldData.worldSeed;
        }
        else
        {
            //make a random seed if we want that
            if (randomSeed)
            {
                seed = Random.Range(1, 100);
            }
            dataObject.GetComponent<GameData>().worldData.worldSeed = seed;
        }

        if (!respawn)
        {
            player.transform.position = new Vector3(dataObject.GetComponent<GameData>().worldData.playerPosX, dataObject.GetComponent<GameData>().worldData.playerPosY, dataObject.GetComponent<GameData>().worldData.playerPosZ);
        }
        else
        {
            player.transform.position = new Vector3(0, 40 + worldModifier, 0);
        }

        foundNotes = dataObject.GetComponent<GameData>().worldData.foundNotes;

        worldModifier = dataObject.GetComponent<GameData>().worldData.worldModifier;

        ability = dataObject.GetComponent<GameData>().worldData.ability;

        lifeCount = dataObject.GetComponent<GameData>().worldData.lifeCount;

        deathCount = dataObject.GetComponent<GameData>().worldData.deathCount;

        if (lifeCount < 1)
        {
            spectate = true;
        }

        oldPos = player.transform.position;
        player.GetComponent<PlayerMove>().targets = new List<GameObject>();
        player.GetComponent<PlayerMove>().isDay = dataObject.GetComponent<GameData>().worldData.isDay;
        player.GetComponent<PlayerMove>().time = dataObject.GetComponent<GameData>().worldData.time;
        player.GetComponent<PlayerMove>().rollSpeed = player.GetComponent<PlayerMove>().speed * 2.25f;
        player.GetComponent<PlayerMove>().lockCursor = true;
        transform.position = player.transform.position;
        begin = true;
        Invoke("StartGen", 0.2f);

        breakerWait = 0.1f;

        player.GetComponent<PlayerMove>().dayCount = dataObject.GetComponent<GameData>().worldData.dayCount;
        player.GetComponent<PlayerMove>().swordTrigger.gameObject.GetComponent<SpriteRenderer>().sprite = null;
    } 

    //when starting a new game
    public void StartGameNew()
    {
        lucky = false;

        breakerParticle.SetActive(false);
        structureLeftId = -1;

        if (ui.GetComponent<UIManager>().seed != null)
        {
            seed = int.Parse(ui.GetComponent<UIManager>().seedString);
            dataObject.GetComponent<GameData>().worldData.worldSeed = seed;
        }
        else
        {
            //make a random seed if we want that
            if (randomSeed)
            {
                seed = Random.Range(1, 100);
            }
            dataObject.GetComponent<GameData>().worldData.worldSeed = seed;
        }

        foundNotes = new List<string>();

        spectate = false;

        deathCount = 0;

        //setting up the player position and the day/night cycle
        player.GetComponent<PlayerMove>().targets = new List<GameObject>();
        player.transform.position = new Vector3(0, 64 + worldModifier, 0);
        player.GetComponent<PlayerMove>().isDay = true;
        player.GetComponent<PlayerMove>().time = 0;
        player.GetComponent<PlayerMove>().rollSpeed = player.GetComponent<PlayerMove>().speed * 2.25f;
        player.GetComponent<PlayerMove>().lockCursor = true;
        oldPos = player.transform.position;
        transform.position = player.transform.position;
        begin = true;
        Invoke("StartGen", 0.2f);

        breakerWait = 0.1f;

        player.GetComponent<PlayerMove>().dayCount = 1;
        StartCoroutine(ui.GetComponent<UIManager>().NewDay());

        player.GetComponent<PlayerMove>().swordTrigger.gameObject.GetComponent<SpriteRenderer>().sprite = null;

        if (ui.GetComponent<UIManager>().session.achievements[12] == false && player.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color == new Color(0, 0, 0, 1) && player.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color == new Color(0, 0, 0, 1) && player.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().color == new Color(0, 0, 0, 1) && player.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().color == new Color(0, 0, 0, 1))
        {
            StartCoroutine(ui.GetComponent<UIManager>().Achievement(12));
            Debug.Log("Achievement Get!");
        }
    }

    //generates the 5 chunks that the player initially spawns on
    public void StartGen()
    {
        if (begin)
        {
            System.Random pRandom = new System.Random(Mathf.RoundToInt(seed) + Mathf.RoundToInt(rightBiomeLeftChecker.position.x));

            biomeLeft = pRandom.Next(5, 10);
            if (forceBiome)
            {
                biomeToGenerate = forceBiomeId;
            }
            else
            {
                biomeToGenerate = pRandom.Next(1, dataObject.GetComponent<GameData>().biomes.Length + 1) - 1;
            }

            GenerateChunk(leftChecker.position);
            GenerateChunk(leftBiomeLeftChecker.position);
            GenerateChunk(biomeChecker.position);
            GenerateChunk(rightBiomeLeftChecker.position);
            GenerateChunk(rightChecker.position);

            if (biomeChecker.position.x >= 0)
            {
                generateDir = rightChecker;
                sampleDir = rightBiomeLeftChecker;
            }
            else if (biomeChecker.position.x < 0)
            {
                generateDir = leftChecker;
                sampleDir = leftBiomeLeftChecker;
            }

            startGen = true;
            player.gameObject.SetActive(true);

            player.GetComponent<PlayerMove>().interacting = false;
            player.GetComponent<PlayerMove>().dead = false;

            player.GetComponent<PlayerMove>().StartCoroutine("TimeControl");
            player.GetComponent<PlayerMove>().InitializeMoon();
        }
    }

    void Update()
    {
        if (currentChunk != null)
        {
            if (currentChunk.gameObject.GetComponent<ChunkGeneration>().biome == 0)
            {
                ui.GetComponent<UIManager>().session.biomesVisited[0] = true;
            }
            else if (currentChunk.gameObject.GetComponent<ChunkGeneration>().biome == 1)
            {
                ui.GetComponent<UIManager>().session.biomesVisited[1] = true;
            }
            else if (currentChunk.gameObject.GetComponent<ChunkGeneration>().biome == 2)
            {
                ui.GetComponent<UIManager>().session.biomesVisited[2] = true;
            }
        }

        if (spectate)
        {
            filter.SetActive(true);
        }
        else
        {
            filter.SetActive(false);
        }

        dataObject.GetComponent<GameData>().worldData.foundNotes = foundNotes;
        dataObject.GetComponent<GameData>().worldData.worldModifier = worldModifier;

        dataObject.GetComponent<GameData>().worldData.ability = ability;

        dataObject.GetComponent<GameData>().worldData.lifeCount = lifeCount;

        dataObject.GetComponent<GameData>().worldData.deathCount = deathCount;

        dataObject.GetComponent<GameData>().worldData.dayCount = player.GetComponent<PlayerMove>().dayCount;

        int remove = -1;

        foreach (float currentTimer in placeParticleTimers)
        {
            if (Time.time - currentTimer < 1 && Time.time - currentTimer > -1)
            {
                remove = placeParticleTimers.IndexOf(currentTimer);
            }
        }

        if (remove != -1)
        {
            placeParticleTimers.Remove(placeParticleTimers[remove]);
            Destroy(placeParticles[remove]);
            placeParticles.Remove(placeParticles[remove]);
        }

        breakerParticle.transform.position = cursor.transform.position;

        if (breakerFade)
        {
            ParticleSystem ps = breakerParticle.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = Color.Lerp(breakerColor, fadeColor, breakerFadeSpeed);
        }

        //saving biome modulation data
        dataObject.GetComponent<GameData>().worldData.biomeHeight = biomeHeight;
        dataObject.GetComponent<GameData>().worldData.biomeHeight2 = biomeHeight2;

        //checking which chunk the player is in right now
        currentChunk = Physics2D.OverlapCircle(biomeChecker.position, chunkCheckRadius, whatIsChunk);
        cursorChunk = Physics2D.OverlapCircle(cursorChecker.position, chunkCheckRadius, whatIsChunk);

        //if you left click (and hold), break a block
        if (Input.GetMouseButton(0) && begin && ui.GetComponent<UIManager>().inGame)
        {
            if (!coolDown)
            {
                StartCoroutine(BreakBlock());
            }
        }

        //if you right click place a block
        if (Input.GetMouseButtonDown(1) && begin && ui.GetComponent<UIManager>().inGame && !spectate)
        {
            PlaceBlock();
        }

        //checking if the player is travelling to the right
        if (oldPos.x < player.transform.position.x)
        {
            right = true;
        }

        //or to the left
        if (oldPos.x > player.transform.position.x)
        {
            right = false;
        }
        oldPos = player.transform.position;

        //deciding which 'checker' object to use to generate chunks, depends on wether ur traveling left or right
        if (right && startGen)
        {
            generateDir = rightChecker;
            sampleDir = rightBiomeLeftChecker;
        }
        else if (!right && startGen)
        {
            generateDir = leftChecker;
            sampleDir = leftBiomeLeftChecker;
        }

        if (startGen)
        {
            sampleBiome = Physics2D.OverlapCircle(sampleDir.position, chunkCheckRadius, whatIsChunk);

            Collider2D BH1 = Physics2D.OverlapCircle(rightBiomeLeftChecker.position, chunkCheckRadius, whatIsChunk);
            if (BH1 != null)
            {
                biomeHeight = BH1.gameObject.GetComponent<ChunkGeneration>().biomeHeight;
            }

            Collider2D BH2 = Physics2D.OverlapCircle(leftBiomeLeftChecker.position, chunkCheckRadius, whatIsChunk);
            if (BH2 != null)
            {
                biomeHeight2 = BH2.gameObject.GetComponent<ChunkGeneration>().biomeHeight2;
            }

            isChunk = Physics2D.OverlapCircle(generateDir.position, chunkCheckRadius, whatIsChunk);
        }

        //if there isn't a chunk already, make one
        if (!isChunk && startGen && ui.GetComponent<UIManager>().inGame)
        {
            GenerateChunk(generateDir.position);   
        } 

        //gets the biome that the player is in
        if (currentChunk != null && begin)
        {
            if (currentChunk.GetComponent<ChunkGeneration>() != null)
            {
                currentBiome = currentChunk.GetComponent<ChunkGeneration>().biome;
            }    
        }
        //sets the color of the sky to match the biome that the player is in
        currentSkyColor = dataObject.GetComponent<GameData>().biomes[currentBiome].skyColor;
        cameraObject.GetComponent<Camera>().backgroundColor = Color.Lerp(cameraObject.GetComponent<Camera>().backgroundColor, currentSkyColor, Time.deltaTime * skyFadeSpeed);

        if (ui.GetComponent<UIManager>().inGame && startGen && cursorChunk != null)
        {
            player.GetComponent<PlayerMove>().climbing = false;
            foreach (Block currentBlock in dataObject.GetComponent<GameData>().blocks)
            {
                Grid grid = cursorChunk.gameObject.transform.GetChild(0).gameObject.GetComponent<Grid>();

                if (currentBlock.tiles.Contains(cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(grid.WorldToCell(transform.position))))
                {
                    if (currentBlock.isClimb)
                    {
                        player.GetComponent<PlayerMove>().climbing = true;
                        player.GetComponent<PlayerMove>().takeoff = player.transform.position.y;

                        break;
                    }
                }
            }
        }
    }

    //allows you to break blocks and plays the breaking animation
    IEnumerator BreakBlock()
    {
        if (cursorChunk != null)
        {
            if (!ui.GetComponent<UIManager>().menuArray[8].activeSelf && !ui.GetComponent<UIManager>().menuArray[11].activeSelf)
            {
                coolDown = true;
                bool yes = true;
                bool yes2 = false;

                //the animation player
                dummy = Instantiate(dummyPrefab, cursor.transform.position, Quaternion.identity);
                dummy.transform.parent = cursorChunk.gameObject.transform;

                if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(dummy.transform.localPosition.x - 0.2f), Mathf.RoundToInt(dummy.transform.localPosition.y - 0.2f), 0)) != bedrockTile && cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(dummy.transform.localPosition.x - 0.2f), Mathf.RoundToInt(dummy.transform.localPosition.y - 0.2f), 0)) != null)
                {
                    if (!spectate)
                    {
                        breakerWait = 0.075f;
                        foreach (Block currentBlock in dataObject.GetComponent<GameData>().blocks)
                        {
                            if (currentBlock.tiles.Contains(cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(dummy.transform.localPosition.x - 0.2f), Mathf.RoundToInt(dummy.transform.localPosition.y - 0.2f), 0))))
                            {
                                if (currentBlock.breakTime != 0)
                                {
                                    if (hotbar.selectedItem != null)
                                    {
                                        if (hotbar.selectedItem.isPickaxe && currentBlock.tags.Contains(hotbar.selectedItem.affectedBlock))
                                        {
                                            breakerWait = currentBlock.breakTime / hotbar.selectedItem.pickaxeSpeed;
                                        }
                                        else
                                        {
                                            breakerWait = currentBlock.breakTime;
                                        }
                                    }
                                    else
                                    {
                                        breakerWait = currentBlock.breakTime;
                                    }

                                    break;
                                }
                                else
                                {
                                    if (hotbar.selectedItem != null)
                                    {
                                        if (hotbar.selectedItem.isPickaxe && currentBlock.tags.Contains(hotbar.selectedItem.affectedBlock))
                                        {
                                            breakerWait = 0.075f / hotbar.selectedItem.pickaxeSpeed;
                                        }
                                        else
                                        {
                                            breakerWait = 0.075f;
                                        }
                                    }
                                    else
                                    {
                                        breakerWait = 0.075f;
                                    }

                                    break;
                                }
                            }
                        }

                        Vector3 breakPos = cursor.transform.position;
                        breakerParticle.SetActive(true);
                        breakerParticle.GetComponent<ParticleSystem>().Play();
                        breakerFade = false;
                        ParticleSystem ps = breakerParticle.GetComponent<ParticleSystem>();

                        var main = ps.main;
                        var texture = ps.textureSheetAnimation;

                        main.startColor = breakerColor;

                        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector3Int coordinate = cursorChunk.gameObject.transform.GetChild(0).gameObject.GetComponent<Grid>().WorldToCell(mouseWorldPos);
                        texture.SetSprite(0, cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetSprite(coordinate));

                        for (int i = 0; i < 10; i++)
                        {
                            if (!Input.GetMouseButton(0) || cursor.transform.position != breakPos || Vector3.Distance(cursor.transform.position, player.transform.position) > breakDistance)
                            {
                                yes = false;
                                breaker.SetFrame(9);
                            }
                            else
                            {
                                yield return new WaitForSeconds(breakerWait);
                                breaker.SetFrame(i);
                            }
                        }
                    }
                }
                else if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.GetTile(new Vector3Int(Mathf.RoundToInt(dummy.transform.localPosition.x - 0.2f), Mathf.RoundToInt(dummy.transform.localPosition.y - 0.2f), 0)) != bedrockTile && cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.GetTile(new Vector3Int(Mathf.RoundToInt(dummy.transform.localPosition.x - 0.2f), Mathf.RoundToInt(dummy.transform.localPosition.y - 0.2f), 0)) != null)
                {
                    if (!spectate)
                    {
                        breakerWait = 0.075f;
                        foreach (Block currentBlock in dataObject.GetComponent<GameData>().blocks)
                        {
                            if (currentBlock.tiles.Contains(cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.GetTile(new Vector3Int(Mathf.RoundToInt(dummy.transform.localPosition.x - 0.2f), Mathf.RoundToInt(dummy.transform.localPosition.y - 0.2f), 0))))
                            {
                                if (currentBlock.breakTime != 0)
                                {
                                    if (hotbar.selectedItem != null)
                                    {
                                        if (hotbar.selectedItem.isPickaxe && currentBlock.tags.Contains(hotbar.selectedItem.affectedBlock))
                                        {
                                            breakerWait = currentBlock.breakTime / hotbar.selectedItem.pickaxeSpeed;
                                        }
                                        else
                                        {
                                            breakerWait = currentBlock.breakTime;
                                        }
                                    }
                                    else
                                    {
                                        breakerWait = currentBlock.breakTime;
                                    }

                                    break;
                                }
                                else
                                {
                                    if (hotbar.selectedItem != null)
                                    {
                                        if (hotbar.selectedItem.isPickaxe && currentBlock.tags.Contains(hotbar.selectedItem.affectedBlock))
                                        {
                                            breakerWait = 0.075f / hotbar.selectedItem.pickaxeSpeed;
                                        }
                                        else
                                        {
                                            breakerWait = 0.075f;
                                        }
                                    }
                                    else
                                    {
                                        breakerWait = 0.075f;
                                    }

                                    break;
                                }
                            }
                        }

                        yes = false;
                        yes2 = true;

                        Vector3 breakPos = cursor.transform.position;
                        breakerParticle.SetActive(true);
                        breakerParticle.GetComponent<ParticleSystem>().Play();
                        breakerFade = false;
                        ParticleSystem ps = breakerParticle.GetComponent<ParticleSystem>();

                        var main = ps.main;
                        var texture = ps.textureSheetAnimation;

                        main.startColor = breakerColor;

                        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        Vector3Int coordinate = cursorChunk.gameObject.transform.GetChild(0).gameObject.GetComponent<Grid>().WorldToCell(mouseWorldPos);
                        texture.SetSprite(0, cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.GetSprite(coordinate));

                        for (int i = 0; i < 10; i++)
                        {
                            if (!Input.GetMouseButton(0) || cursor.transform.position != breakPos || Vector3.Distance(cursor.transform.position, player.transform.position) > breakDistance)
                            {
                                yes2 = false;
                                breaker.SetFrame(9);
                            }
                            else
                            {
                                yield return new WaitForSeconds(breakerWait);
                                breaker.SetFrame(i);
                            }
                        }
                    }
                }


                if (dummy != null)
                {
                    Destroy(dummy);
                }

                if (yes && !spectate)
                {
                    cursor.transform.parent = cursorChunk.gameObject.transform;
                    if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) != bedrockTile && cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) != null)
                    {
                        TileBase readTile = null;
                        int writeTile = 0;

                        for (int i = 0; i < 4; i++)
                        {
                            int xOffset = 0;
                            int yOffset = 0;

                            if (i == 0)
                            {
                                xOffset = -1;

                                yOffset = 0;
                            }
                            else if (i == 1)
                            {
                                xOffset = 1;

                                yOffset = 0;
                            }
                            else if (i == 2)
                            {
                                xOffset = 0;

                                yOffset = -1;
                            }
                            else if (i == 3)
                            {
                                xOffset = 0;

                                yOffset = 1;
                            }

                            foreach (Block currentBlock in dataObject.GetComponent<GameData>().blocks)
                            {
                                if (currentBlock.tiles.Contains(cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset, 0))))
                                {
                                    if (currentBlock.dependent && currentBlock.offsetX == -xOffset && currentBlock.offsetY == -yOffset)
                                    {
                                        readTile = cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset, 0));
                                        writeTile = 0;
                                        foreach (Block _currentBlock in dataObject.GetComponent<GameData>().blocks)
                                        {
                                            if (_currentBlock.tiles.Contains(readTile))
                                            {
                                                writeTile = dataObject.GetComponent<GameData>().blocks.IndexOf(_currentBlock);
                                            }
                                        }

                                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.SetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset, 0), null);
                                        for (int _i = 0; _i < cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights.Count; _i++)
                                        {
                                            if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights[_i].name == $"{new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset, 0)}")
                                            {
                                                Destroy(cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights[_i]);
                                                cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights.RemoveAt(_i);
                                            }
                                        }

                                        for (int _i = 0; _i < cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles.Count; _i++)
                                        {
                                            if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles[_i].name == $"{new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset, 0)}")
                                            {
                                                Destroy(cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles[_i]);
                                                cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles.RemoveAt(_i);
                                            }
                                        }

                                        cac.transform.parent = cursorChunk.gameObject.transform;
                                        cac.localPosition = new Vector3((-32 / 2), (-128 / 2), 0);
                                        cursor.transform.parent = cac;

                                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkArray[Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset] = 0;
                                        Debug.Log(cursorChunk.gameObject.name);

                                        if (dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropId != 0)
                                        {
                                            if (dataObject.GetComponent<GameData>().blocks[writeTile].tiles[0] == beehiveTile)
                                            {
                                                GameObject bee = Instantiate(dataObject.GetComponent<GameData>().entities[0], new Vector3(Mathf.RoundToInt(cursor.transform.position.x - 0.2f) + xOffset, Mathf.RoundToInt(cursor.transform.position.y - 0.2f) + yOffset, 0), Quaternion.identity);

                                                currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(bee);
                                                entityScript.GetComponent<Entities>().entities.Add(bee);

                                                if (ui.GetComponent<UIManager>().session.achievements[14] == false)
                                                {
                                                    StartCoroutine(ui.GetComponent<UIManager>().Achievement(14));
                                                    Debug.Log("Achievement Get!");
                                                }
                                            }

                                            Stack stack = new Stack();
                                            if (!lucky)
                                            {
                                                stack.count = dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropCount;
                                            }
                                            else
                                            {
                                                stack.count = dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropCount * 2;
                                            }

                                            stack.itemType = dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropId - 1;

                                            GameObject dropItem = Instantiate(dataObject.GetComponent<GameData>().entities[7], new Vector3(GameObject.Find("Cursor").transform.position.x, GameObject.Find("Cursor").transform.position.y, 0), Quaternion.identity);
                                            dropItem.GetComponent<SpriteRenderer>().sprite = dataObject.GetComponent<GameData>().items[stack.itemType].sprite;

                                            dropItem.GetComponent<DroppedItem>().inventory = inventory;
                                            dropItem.GetComponent<DroppedItem>().hotbar = hotbar;
                                            dropItem.GetComponent<DroppedItem>().itemId = stack.itemType;
                                            dropItem.GetComponent<DroppedItem>().count = stack.count;

                                            currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(dropItem);
                                            entityScript.GetComponent<Entities>().entities.Add(dropItem);

                                            dropItem.GetComponent<DroppedItem>().Initialize();
                                        }

                                        cac.transform.parent = cursorChunk.gameObject.transform;
                                        cac.localPosition = new Vector3((-32 / 2), (-128 / 2), 0);
                                        cursor.transform.parent = cac;

                                        ChunkUpdater remove = null;
                                        foreach (ChunkUpdater currentUpdater in cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkUpdaters)
                                        {
                                            if (currentUpdater.coordX == Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset && currentUpdater.coordY == Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset && currentUpdater.background == false)
                                            {
                                                remove = currentUpdater;

                                                break;
                                            }
                                        }

                                        Chest _remove = null;
                                        foreach (Chest currentChest in cursorChunk.gameObject.GetComponent<ChunkGeneration>().chests)
                                        {
                                            if (currentChest.coordX == Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset && currentChest.coordY == Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset && currentChest.background == false)
                                            {
                                                _remove = currentChest;

                                                foreach (Stack currentStack in currentChest.items)
                                                {
                                                    for (int _i = 0; _i < currentStack.count; _i++)
                                                    {
                                                        GameObject dropItem = Instantiate(inventory.itemEntity, new Vector3(GameObject.Find("Cursor").transform.position.x, GameObject.Find("Cursor").transform.position.y, 0), Quaternion.identity);
                                                        dropItem.GetComponent<SpriteRenderer>().sprite = dataObject.GetComponent<GameData>().items[currentStack.itemType].sprite;

                                                        dropItem.GetComponent<DroppedItem>().inventory = inventory;
                                                        dropItem.GetComponent<DroppedItem>().hotbar = hotbar;
                                                        dropItem.GetComponent<DroppedItem>().itemId = currentStack.itemType;
                                                        dropItem.GetComponent<DroppedItem>().count = currentStack.count;

                                                        currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(dropItem);
                                                        entityScript.GetComponent<Entities>().entities.Add(dropItem);

                                                        dropItem.GetComponent<DroppedItem>().Initialize();
                                                    }
                                                }
                                                currentChest.items.Clear();

                                                break;
                                            }
                                        }

                                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkUpdaters.Remove(remove);
                                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().chests.Remove(_remove);

                                        cac.transform.parent = null;
                                        cursor.transform.parent = null;
                                    }
                                }
                            }
                        }

                        cursor.transform.parent = cursorChunk.gameObject.transform;

                        readTile = cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0));
                        writeTile = 0;
                        foreach (Block currentBlock in dataObject.GetComponent<GameData>().blocks)
                        {
                            if (currentBlock.tiles.Contains(readTile))
                            {
                                writeTile = dataObject.GetComponent<GameData>().blocks.IndexOf(currentBlock);
                            }
                        }

                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.SetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0), null);
                        for (int i = 0; i < cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights.Count; i++)
                        {
                            if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights[i].name == $"{new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)}")
                            {
                                Destroy(cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights[i]);
                                cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights.RemoveAt(i);
                            }
                        }

                        for (int i = 0; i < cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles.Count; i++)
                        {
                            if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles[i].name == $"{new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)}")
                            {
                                Destroy(cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles[i]);
                                cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles.RemoveAt(i);
                            }
                        }

                        cac.transform.parent = cursorChunk.gameObject.transform;
                        cac.localPosition = new Vector3((-32 / 2), (-128 / 2), 0);
                        cursor.transform.parent = cac;

                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkArray[Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f)] = 0;
                        Debug.Log(cursorChunk.gameObject.name);

                        if (dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropId != 0)
                        {
                            if (dataObject.GetComponent<GameData>().blocks[writeTile].tiles[0] == beehiveTile)
                            {
                                GameObject bee = Instantiate(dataObject.GetComponent<GameData>().entities[0], new Vector3(Mathf.RoundToInt(cursor.transform.position.x - 0.2f), Mathf.RoundToInt(cursor.transform.position.y - 0.2f), 0), Quaternion.identity);

                                currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(bee);
                                entityScript.GetComponent<Entities>().entities.Add(bee);

                                if (ui.GetComponent<UIManager>().session.achievements[14] == false)
                                {
                                    StartCoroutine(ui.GetComponent<UIManager>().Achievement(14));
                                    Debug.Log("Achievement Get!");
                                }
                            }

                            Stack stack = new Stack();
                            if (!lucky)
                            {
                                stack.count = dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropCount;
                            }
                            else
                            {
                                stack.count = dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropCount * 2;
                            }

                            stack.itemType = dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropId - 1;

                            GameObject dropItem = Instantiate(dataObject.GetComponent<GameData>().entities[7], new Vector3(GameObject.Find("Cursor").transform.position.x, GameObject.Find("Cursor").transform.position.y, 0), Quaternion.identity);
                            dropItem.GetComponent<SpriteRenderer>().sprite = dataObject.GetComponent<GameData>().items[stack.itemType].sprite;

                            dropItem.GetComponent<DroppedItem>().inventory = inventory;
                            dropItem.GetComponent<DroppedItem>().hotbar = hotbar;
                            dropItem.GetComponent<DroppedItem>().itemId = stack.itemType;
                            dropItem.GetComponent<DroppedItem>().count = stack.count;

                            currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(dropItem);
                            entityScript.GetComponent<Entities>().entities.Add(dropItem);

                            dropItem.GetComponent<DroppedItem>().Initialize();
                        }

                        cac.transform.parent = cursorChunk.gameObject.transform;
                        cac.localPosition = new Vector3((-32 / 2), (-128 / 2), 0);
                        cursor.transform.parent = cac;

                        ChunkUpdater remove_ = null;
                        foreach (ChunkUpdater currentUpdater in cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkUpdaters)
                        {
                            if (currentUpdater.coordX == Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) && currentUpdater.coordY == Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) && currentUpdater.background == false)
                            {
                                remove_ = currentUpdater;

                                break;
                            }
                        }

                        Chest _remove_ = null;
                        foreach (Chest currentChest in cursorChunk.gameObject.GetComponent<ChunkGeneration>().chests)
                        {
                            if (currentChest.coordX == Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) && currentChest.coordY == Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) && currentChest.background == false)
                            {
                                _remove_ = currentChest;

                                foreach (Stack currentStack in currentChest.items)
                                {
                                    for (int i = 0; i < currentStack.count; i++)
                                    {
                                        GameObject dropItem = Instantiate(inventory.itemEntity, new Vector3(GameObject.Find("Cursor").transform.position.x, GameObject.Find("Cursor").transform.position.y, 0), Quaternion.identity);
                                        dropItem.GetComponent<SpriteRenderer>().sprite = dataObject.GetComponent<GameData>().items[currentStack.itemType].sprite;

                                        dropItem.GetComponent<DroppedItem>().inventory = inventory;
                                        dropItem.GetComponent<DroppedItem>().hotbar = hotbar;
                                        dropItem.GetComponent<DroppedItem>().itemId = currentStack.itemType;
                                        dropItem.GetComponent<DroppedItem>().count = currentStack.count;

                                        currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(dropItem);
                                        entityScript.GetComponent<Entities>().entities.Add(dropItem);

                                        dropItem.GetComponent<DroppedItem>().Initialize();
                                    }
                                }
                                currentChest.items.Clear();

                                break;
                            }
                        }

                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkUpdaters.Remove(remove_);
                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().chests.Remove(_remove_);

                        cac.transform.parent = null;
                        cursor.transform.parent = null;
                    }
                }

                if (yes2 && !spectate)
                {
                    cursor.transform.parent = cursorChunk.gameObject.transform;
                    if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) != bedrockTile && cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) != null)
                    {
                        TileBase readTile = null;
                        int writeTile = 0;

                        for (int i = 0; i < 4; i++)
                        {
                            int xOffset = 0;
                            int yOffset = 0;

                            if (i == 0)
                            {
                                xOffset = -1;

                                yOffset = 0;
                            }
                            else if (i == 1)
                            {
                                xOffset = 1;

                                yOffset = 0;
                            }
                            else if (i == 2)
                            {
                                xOffset = 0;

                                yOffset = -1;
                            }
                            else if (i == 3)
                            {
                                xOffset = 0;

                                yOffset = 1;
                            }

                            foreach (Block currentBlock in dataObject.GetComponent<GameData>().blocks)
                            {
                                if (currentBlock.tiles.Contains(cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset, 0))))
                                {
                                    if (currentBlock.dependent && currentBlock.offsetX == -xOffset && currentBlock.offsetY == -yOffset)
                                    {
                                        readTile = cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset, 0));
                                        writeTile = 0;
                                        foreach (Block _currentBlock in dataObject.GetComponent<GameData>().blocks)
                                        {
                                            if (_currentBlock.tiles.Contains(readTile))
                                            {
                                                writeTile = dataObject.GetComponent<GameData>().blocks.IndexOf(_currentBlock);
                                            }
                                        }

                                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.SetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset, 0), null);
                                        for (int _i = 0; _i < cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights.Count; _i++)
                                        {
                                            if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights[_i].name == $"{new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset, 0)}")
                                            {
                                                Destroy(cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights[_i]);
                                                cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights.RemoveAt(_i);
                                            }
                                        }

                                        for (int _i = 0; _i < cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles.Count; _i++)
                                        {
                                            if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles[_i].name == $"{new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset, 0)}")
                                            {
                                                Destroy(cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles[_i]);
                                                cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles.RemoveAt(_i);
                                            }
                                        }

                                        cac.transform.parent = cursorChunk.gameObject.transform;
                                        cac.localPosition = new Vector3((-32 / 2), (-128 / 2), 0);
                                        cursor.transform.parent = cac;

                                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkArray2[Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset] = 0;
                                        Debug.Log(cursorChunk.gameObject.name);

                                        if (dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropId != 0)
                                        {
                                            if (dataObject.GetComponent<GameData>().blocks[writeTile].tiles[0] == beehiveTile)
                                            {
                                                GameObject bee = Instantiate(dataObject.GetComponent<GameData>().entities[0], new Vector3(Mathf.RoundToInt(cursor.transform.position.x - 0.2f) + xOffset, Mathf.RoundToInt(cursor.transform.position.y - 0.2f) + yOffset, 0), Quaternion.identity);

                                                currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(bee);
                                                entityScript.GetComponent<Entities>().entities.Add(bee);

                                                if (ui.GetComponent<UIManager>().session.achievements[14] == false)
                                                {
                                                    StartCoroutine(ui.GetComponent<UIManager>().Achievement(14));
                                                    Debug.Log("Achievement Get!");
                                                }
                                            }

                                            Stack stack = new Stack();
                                            if (!lucky)
                                            {
                                                stack.count = dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropCount;
                                            }
                                            else
                                            {
                                                stack.count = dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropCount * 2;
                                            }

                                            stack.itemType = dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropId - 1;

                                            GameObject dropItem = Instantiate(dataObject.GetComponent<GameData>().entities[7], new Vector3(GameObject.Find("Player").transform.position.x, GameObject.Find("Player").transform.position.y, 0), Quaternion.identity);
                                            dropItem.GetComponent<SpriteRenderer>().sprite = dataObject.GetComponent<GameData>().items[stack.itemType].sprite;

                                            dropItem.GetComponent<DroppedItem>().inventory = inventory;
                                            dropItem.GetComponent<DroppedItem>().hotbar = hotbar;
                                            dropItem.GetComponent<DroppedItem>().itemId = stack.itemType;
                                            dropItem.GetComponent<DroppedItem>().count = stack.count;

                                            currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(dropItem);
                                            entityScript.GetComponent<Entities>().entities.Add(dropItem);

                                            dropItem.GetComponent<DroppedItem>().Initialize();
                                        }

                                        cac.transform.parent = cursorChunk.gameObject.transform;
                                        cac.localPosition = new Vector3((-32 / 2), (-128 / 2), 0);
                                        cursor.transform.parent = cac;

                                        ChunkUpdater remove = null;
                                        foreach (ChunkUpdater currentUpdater in cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkUpdaters)
                                        {
                                            if (currentUpdater.coordX == Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset && currentUpdater.coordY == Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset && currentUpdater.background == true)
                                            {
                                                remove = currentUpdater;

                                                break;
                                            }
                                        }

                                        Chest _remove = null;
                                        foreach (Chest currentChest in cursorChunk.gameObject.GetComponent<ChunkGeneration>().chests)
                                        {
                                            if (currentChest.coordX == Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + xOffset && currentChest.coordY == Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + yOffset && currentChest.background == true)
                                            {
                                                _remove = currentChest;

                                                foreach (Stack currentStack in currentChest.items)
                                                {
                                                    for (int _i = 0; _i < currentStack.count; _i++)
                                                    {
                                                        GameObject dropItem = Instantiate(inventory.itemEntity, new Vector3(GameObject.Find("Player").transform.position.x, GameObject.Find("Player").transform.position.y, 0), Quaternion.identity);
                                                        dropItem.GetComponent<SpriteRenderer>().sprite = dataObject.GetComponent<GameData>().items[currentStack.itemType].sprite;

                                                        dropItem.GetComponent<DroppedItem>().inventory = inventory;
                                                        dropItem.GetComponent<DroppedItem>().hotbar = hotbar;
                                                        dropItem.GetComponent<DroppedItem>().itemId = currentStack.itemType;
                                                        dropItem.GetComponent<DroppedItem>().count = currentStack.count;

                                                        currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(dropItem);
                                                        entityScript.GetComponent<Entities>().entities.Add(dropItem);

                                                        dropItem.GetComponent<DroppedItem>().Initialize();
                                                    }
                                                }
                                                currentChest.items.Clear();

                                                break;
                                            }
                                        }

                                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkUpdaters.Remove(remove);
                                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().chests.Remove(_remove);

                                        cac.transform.parent = null;
                                        cursor.transform.parent = null;
                                    }
                                }
                            }
                        }

                        cursor.transform.parent = cursorChunk.gameObject.transform;

                        readTile = cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0));
                        writeTile = 0;
                        foreach (Block currentBlock in dataObject.GetComponent<GameData>().blocks)
                        {
                            if (currentBlock.tiles.Contains(readTile))
                            {
                                writeTile = dataObject.GetComponent<GameData>().blocks.IndexOf(currentBlock);
                            }
                        }

                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.SetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0), null);
                        for (int i = 0; i < cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights.Count; i++)
                        {
                            if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights[i].name == $"{new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)}")
                            {
                                Destroy(cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights[i]);
                                cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights.RemoveAt(i);
                            }
                        }

                        for (int i = 0; i < cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles.Count; i++)
                        {
                            if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles[i].name == $"{new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)}")
                            {
                                Destroy(cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles[i]);
                                cursorChunk.gameObject.GetComponent<ChunkGeneration>().leafParticles.RemoveAt(i);
                            }
                        }

                        cac.transform.parent = cursorChunk.gameObject.transform;
                        cac.localPosition = new Vector3((-32 / 2), (-128 / 2), 0);
                        cursor.transform.parent = cac;

                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkArray2[Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f)] = 0;

                        Debug.Log("x" + Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f));
                        Debug.Log("y" + Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f));
                        Debug.Log(cursorChunk.gameObject.name);

                        if (dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropId != 0)
                        {
                            if (dataObject.GetComponent<GameData>().blocks[writeTile].tiles[0] == beehiveTile)
                            {
                                GameObject bee = Instantiate(dataObject.GetComponent<GameData>().entities[0], new Vector3(Mathf.RoundToInt(cursor.transform.position.x - 0.2f), Mathf.RoundToInt(cursor.transform.position.y - 0.2f), 0), Quaternion.identity);

                                currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(bee);
                                entityScript.GetComponent<Entities>().entities.Add(bee);

                                if (ui.GetComponent<UIManager>().session.achievements[14] == false)
                                {
                                    StartCoroutine(ui.GetComponent<UIManager>().Achievement(14));
                                    Debug.Log("Achievement Get!");
                                }
                            }

                            Stack stack = new Stack();
                            if (!lucky)
                            {
                                stack.count = dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropCount;
                            }
                            else
                            {
                                stack.count = dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropCount * 2;
                            }

                            stack.itemType = dataObject.GetComponent<GameData>().blocks[writeTile].drops[dataObject.GetComponent<GameData>().blocks[writeTile].tiles.IndexOf(readTile)].dropId - 1;

                            GameObject dropItem = Instantiate(dataObject.GetComponent<GameData>().entities[7], new Vector3(GameObject.Find("Cursor").transform.position.x, GameObject.Find("Cursor").transform.position.y, 0), Quaternion.identity);
                            dropItem.GetComponent<SpriteRenderer>().sprite = dataObject.GetComponent<GameData>().items[stack.itemType].sprite;

                            dropItem.GetComponent<DroppedItem>().inventory = inventory;
                            dropItem.GetComponent<DroppedItem>().hotbar = hotbar;
                            dropItem.GetComponent<DroppedItem>().itemId = stack.itemType;
                            dropItem.GetComponent<DroppedItem>().count = stack.count;

                            currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(dropItem);
                            entityScript.GetComponent<Entities>().entities.Add(dropItem);

                            dropItem.GetComponent<DroppedItem>().Initialize();
                        }

                        cac.transform.parent = cursorChunk.gameObject.transform;
                        cac.localPosition = new Vector3((-32 / 2), (-128 / 2), 0);
                        cursor.transform.parent = cac;

                        ChunkUpdater remove_ = null;
                        foreach (ChunkUpdater currentUpdater in cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkUpdaters)
                        {
                            if (currentUpdater.coordX == Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) && currentUpdater.coordY == Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) && currentUpdater.background == true)
                            {
                                remove_ = currentUpdater;

                                break;
                            }
                        }

                        Chest _remove_ = null;
                        foreach (Chest currentChest in cursorChunk.gameObject.GetComponent<ChunkGeneration>().chests)
                        {
                            if (currentChest.coordX == Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) && currentChest.coordY == Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) && currentChest.background == true)
                            {
                                _remove_ = currentChest;

                                foreach (Stack currentStack in currentChest.items)
                                {
                                    for (int i = 0; i < currentStack.count; i++)
                                    {
                                        GameObject dropItem = Instantiate(inventory.itemEntity, new Vector3(GameObject.Find("Cursor").transform.position.x, GameObject.Find("Cursor").transform.position.y, 0), Quaternion.identity);
                                        dropItem.GetComponent<SpriteRenderer>().sprite = dataObject.GetComponent<GameData>().items[currentStack.itemType].sprite;

                                        dropItem.GetComponent<DroppedItem>().inventory = inventory;
                                        dropItem.GetComponent<DroppedItem>().hotbar = hotbar;
                                        dropItem.GetComponent<DroppedItem>().itemId = currentStack.itemType;
                                        dropItem.GetComponent<DroppedItem>().count = currentStack.count;

                                        currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(dropItem);
                                        entityScript.GetComponent<Entities>().entities.Add(dropItem);

                                        dropItem.GetComponent<DroppedItem>().Initialize();
                                    }
                                }
                                currentChest.items.Clear();

                                break;
                            }
                        }

                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkUpdaters.Remove(remove_);
                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().chests.Remove(_remove_);

                        cac.transform.parent = null;
                        cursor.transform.parent = null;
                    }
                }

                coolDown = false;
                breakerFade = true;
            }
        }
    }

    //places a block for you, when you right-click
    void PlaceBlock()
    {
        cursor.transform.parent = cursorChunk.gameObject.transform;

        if (!ui.GetComponent<UIManager>().menuArray[1].activeSelf && !ui.GetComponent<UIManager>().menuArray[8].activeSelf && !ui.GetComponent<UIManager>().menuArray[11].activeSelf && hotbar.selectedItem != null &&!spectate)
        {
            bool workbenchFound = false;
            TileBase foundTile = null;

            foreach (TileBase currentTile in workbenchTiles)
            {
                if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) == currentTile || cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) == null && cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) == currentTile && !ui.GetComponent<UIManager>().menuArray[8].activeSelf && !ui.GetComponent<UIManager>().menuArray[12].activeSelf && !ui.GetComponent<UIManager>().menuArray[1].activeSelf)
                {
                    workbenchFound = true;
                    foundTile = currentTile;

                    break;
                }
            }

            if (workbenchFound)
            {
                ui.GetComponent<UIManager>().inventory.craftingType = System.Array.IndexOf(workbenchTiles, foundTile) + 1;

                if (ui.GetComponent<UIManager>().inventory.craftingType == 1)
                {
                    if (ui.GetComponent<UIManager>().menuArray[16].activeSelf)
                    {
                        ui.GetComponent<UIManager>().inventory.DumpMenu();
                    }

                    ui.GetComponent<UIManager>().menuArray[16].SetActive(!ui.GetComponent<UIManager>().menuArray[16].activeSelf);
                }
                else
                {
                    if (ui.GetComponent<UIManager>().menuArray[11].activeSelf)
                    {
                        ui.GetComponent<UIManager>().inventory.DumpMenu();
                    }

                    ui.GetComponent<UIManager>().menuArray[11].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = cookingMenuSprite;
                    ui.GetComponent<UIManager>().menuArray[11].SetActive(!ui.GetComponent<UIManager>().menuArray[11].activeSelf);
                }
            }
            else if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) == chestTile)
            {
                cac.transform.parent = cursorChunk.gameObject.transform;
                cac.localPosition = new Vector3((-32 / 2), (-128 / 2), 0);
                cursor.transform.parent = cac;

                ui.GetComponent<UIManager>().ToggleChest(new Vector2(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f)), false, true);
            }
            else if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) == null && cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) == chestTile)
            {
                cac.transform.parent = cursorChunk.gameObject.transform;
                cac.localPosition = new Vector3((-32 / 2), (-128 / 2), 0);
                cursor.transform.parent = cac;

                ui.GetComponent<UIManager>().ToggleChest(new Vector2(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f)), true, true);
            }
            else
            {
                if (!player.GetComponent<PlayerMove>().isCrouch)
                {
                    bool removeItem = false;

                    Stack stack = new Stack();
                    stack.count = 1;
                    stack.itemType = dataObject.GetComponent<GameData>().items.IndexOf(hotbar.selectedItem);

                    Debug.Log(cursorChunk.gameObject.name);

                    ui.GetComponent<HUD>().UpdateHunger(hotbar.selectedItem.giveHealth);

                    if (hotbar.selectedItem.giveEffect)
                    {
                        foreach (int currentEffect in hotbar.selectedItem.effects)
                        {
                            ui.GetComponent<StatusManager>().TriggerEffect(currentEffect, hotbar.selectedItem.effectTimes[System.Array.IndexOf(hotbar.selectedItem.effects, currentEffect)]);
                        }
                    }

                    if (hotbar.selectedItem.giveHealth != 0 || hotbar.selectedItem.giveEffect)
                    {
                        removeItem = true;
                    }

                    if (Vector3.Distance(cursor.transform.position, player.transform.position) < placeDistance)
                    {
                        cursor.transform.parent = cursorChunk.gameObject.transform;
                        if (hotbar.selectedItem.spawnBalloon && cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) == null && Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + -128 / 2 + 64 < 128)
                        {
                            GameObject newAirship = Instantiate(dataObject.GetComponent<GameData>().entities[11], cursor.transform.position, Quaternion.identity);
                            currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(newAirship);
                            entityScript.GetComponent<Entities>().entities.Add(newAirship);

                            removeItem = true;
                        }

                        cursor.transform.parent = cursorChunk.gameObject.transform;
                        bool proceed = false;
                        if (hotbar.selectedItem.place < 1)
                        {
                            proceed = true;
                        }
                        else
                        {
                            if (dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].dependent)
                            {
                                if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].offsetX, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].offsetY, 0)) == null)
                                {
                                    proceed = false;
                                }
                                else
                                {
                                    int blockIndex = 0;
                                    foreach (Block currentBlock in dataObject.GetComponent<GameData>().blocks)
                                    {
                                        if (currentBlock.tiles.Contains(cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].offsetX, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].offsetY, 0))))
                                        {
                                            blockIndex = dataObject.GetComponent<GameData>().blocks.IndexOf(currentBlock);

                                            break;
                                        }
                                    }

                                    foreach (string currentTag in dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].placeTags)
                                    {
                                        if (dataObject.GetComponent<GameData>().blocks[blockIndex].tags.Contains(currentTag))
                                        {
                                            proceed = true;

                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                proceed = true;
                            }
                        }

                        if (proceed)
                        {
                            cursor.transform.parent = cursorChunk.gameObject.transform;
                            if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) == null && Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + -128 / 2 + 64 < 128 && hotbar.selectedItem.place != 0)
                            {
                                if (hotbar.selectedItem.placeStage != -1)
                                {
                                    cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.SetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0), dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].tiles[hotbar.selectedItem.placeStage]);
                                }
                                else
                                {
                                    cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.SetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0), dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].tiles[dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].tiles.Count - 1]);
                                }

                                if (dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].lightPrefab != null)
                                {
                                    GameObject newLight = Instantiate(dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].lightPrefab, cursorChunk.gameObject.transform.position, Quaternion.identity);
                                    newLight.transform.SetParent(cursorChunk.gameObject.transform);
                                    newLight.transform.localPosition = new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0);
                                    newLight.name = $"{new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)}";
                                    cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights.Add(newLight);
                                }

                                cac.transform.parent = cursorChunk.gameObject.transform;
                                cac.localPosition = new Vector3((-32 / 2), (-128 / 2), 0);
                                cursor.transform.parent = cac;

                                cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkArray[Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f)] = hotbar.selectedItem.place;

                                if (dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].tiles.Count > 1)
                                {
                                    ChunkUpdater newUpdater = new ChunkUpdater();

                                    newUpdater.blockType = hotbar.selectedItem.place - 1;
                                    newUpdater.currentStage = 0;

                                    newUpdater.coordX = Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f);
                                    newUpdater.coordY = Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f);

                                    newUpdater.background = false;

                                    newUpdater.time = 0;

                                    cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkUpdaters.Add(newUpdater);
                                }

                                if (dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].name == "Chest")
                                {
                                    Chest newChest = new Chest();

                                    newChest.items = new List<Stack>();

                                    newChest.coordX = Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f);
                                    newChest.coordY = Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f);

                                    newChest.background = false;

                                    cursorChunk.gameObject.GetComponent<ChunkGeneration>().chests.Add(newChest);
                                }

                                removeItem = true;
                            }
                            else if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) == null && hotbar.selectedItem.place != 0)
                            {
                                if (!ui.GetComponent<UIManager>().warningCoolDown)
                                {
                                    ui.GetComponent<UIManager>().StartCoroutine("Warning");
                                }
                            }
                        }
                    }

                    if (removeItem)
                    {
                        hotbar.RemoveItem(stack, hotbar.selectedCell);
                        hotbar.selecter.SetActive(false);
                        hotbar.selectedItem = null;

                        hotbar.RunSelecter(hotbar.selectedCell - 1);
                    }
                }
                else
                {
                    bool removeItem = false;

                    Stack stack = new Stack();
                    stack.count = 1;
                    stack.itemType = dataObject.GetComponent<GameData>().items.IndexOf(hotbar.selectedItem);

                    Debug.Log(cursorChunk.gameObject.name);

                    ui.GetComponent<HUD>().UpdateHunger(hotbar.selectedItem.giveHealth);

                    if (hotbar.selectedItem.giveEffect)
                    {
                        foreach (int currentEffect in hotbar.selectedItem.effects)
                        {
                            ui.GetComponent<StatusManager>().TriggerEffect(currentEffect, hotbar.selectedItem.effectTimes[System.Array.IndexOf(hotbar.selectedItem.effects, currentEffect)]);
                        }
                    }

                    if (hotbar.selectedItem.giveHealth != 0 || hotbar.selectedItem.giveEffect)
                    {
                        removeItem = true;
                    }

                    if (Vector3.Distance(cursor.transform.position, player.transform.position) < placeDistance)
                    {
                        cursor.transform.parent = cursorChunk.gameObject.transform;
                        if (hotbar.selectedItem.spawnBalloon && cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) == null && Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + -128 / 2 + 64 < 128)
                        {
                            GameObject newAirship = Instantiate(dataObject.GetComponent<GameData>().entities[11], cursor.transform.position, Quaternion.identity);
                            currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(newAirship);
                            entityScript.GetComponent<Entities>().entities.Add(newAirship);

                            removeItem = true;
                        }

                        cursor.transform.parent = cursorChunk.gameObject.transform;
                        if (hotbar.selectedItem != null)
                        {
                            bool proceed = false;
                            if (hotbar.selectedItem.place < 1)
                            {
                                proceed = true;
                            }
                            else
                            {
                                if (dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].dependent)
                                {
                                    if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].offsetX, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].offsetY, 0)) == null)
                                    {
                                        proceed = false;
                                    }
                                    else
                                    {
                                        int blockIndex = 0;
                                        foreach (Block currentBlock in dataObject.GetComponent<GameData>().blocks)
                                        {
                                            if (currentBlock.tiles.Contains(cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f) + dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].offsetX, Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].offsetY, 0))))
                                            {
                                                blockIndex = dataObject.GetComponent<GameData>().blocks.IndexOf(currentBlock);

                                                break;
                                            }
                                        }

                                        foreach (string currentTag in dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].placeTags)
                                        {
                                            if (dataObject.GetComponent<GameData>().blocks[blockIndex].tags.Contains(currentTag))
                                            {
                                                proceed = true;

                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    proceed = true;
                                }
                            }

                            if (proceed)
                            {
                                cursor.transform.parent = cursorChunk.gameObject.transform;
                                if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) == null && Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f) + -128 / 2 + 64 < 128 && hotbar.selectedItem.place != 0)
                                {
                                    //Placing particles, I don't want these right not as they're a little too much feedback

                                    //GameObject newParticle = Instantiate(placeParticle, cursor.transform.position, Quaternion.identity);

                                    //ParticleSystem ps = newParticle.GetComponent<ParticleSystem>();
                                    //var texture = ps.textureSheetAnimation;

                                    //texture.SetSprite(0, hotbar.selectedItem.sprite);

                                    //placeParticles.Add(newParticle);
                                    //placeParticleTimers.Add(Time.time + 2);

                                    //newParticle.GetComponent<ParticleSystem>().Play();

                                    if (hotbar.selectedItem.placeStage != -1)
                                    {
                                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.SetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0), dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].tiles[hotbar.selectedItem.placeStage]);
                                    }
                                    else
                                    {
                                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.SetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0), dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].tiles[dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].tiles.Count - 1]);
                                    }

                                    if (dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].lightPrefab != null)
                                    {
                                        GameObject newLight = Instantiate(dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].lightPrefab, cursorChunk.gameObject.transform.position, Quaternion.identity);
                                        newLight.transform.SetParent(cursorChunk.gameObject.transform);
                                        newLight.transform.localPosition = new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0);
                                        newLight.name = $"{new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)}";
                                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().lights.Add(newLight);
                                    }

                                    cac.transform.parent = cursorChunk.gameObject.transform;
                                    cac.localPosition = new Vector3((-32 / 2), (-128 / 2), 0);
                                    cursor.transform.parent = cac;

                                    cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkArray2[Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f)] = hotbar.selectedItem.place;

                                    if (dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].tiles.Count > 1)
                                    {
                                        ChunkUpdater newUpdater = new ChunkUpdater();

                                        newUpdater.blockType = hotbar.selectedItem.place - 1;
                                        newUpdater.currentStage = 0;

                                        newUpdater.coordX = Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f);
                                        newUpdater.coordY = Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f);

                                        newUpdater.background = true;

                                        newUpdater.time = 0;

                                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().chunkUpdaters.Add(newUpdater);
                                    }

                                    if (dataObject.GetComponent<GameData>().blocks[hotbar.selectedItem.place - 1].name == "Chest")
                                    {
                                        Chest newChest = new Chest();

                                        newChest.items = new List<Stack>();

                                        newChest.coordX = Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f);
                                        newChest.coordY = Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f);

                                        newChest.background = true;

                                        cursorChunk.gameObject.GetComponent<ChunkGeneration>().chests.Add(newChest);
                                    }

                                    removeItem = true;
                                }
                                else if (cursorChunk.gameObject.GetComponent<ChunkGeneration>().tilemap2.GetTile(new Vector3Int(Mathf.RoundToInt(cursor.transform.localPosition.x - 0.2f), Mathf.RoundToInt(cursor.transform.localPosition.y - 0.2f), 0)) == null && hotbar.selectedItem.place != 0)
                                {
                                    if (!ui.GetComponent<UIManager>().warningCoolDown)
                                    {
                                        ui.GetComponent<UIManager>().StartCoroutine("Warning");
                                    }
                                }
                            }
                        }
                    }

                    if (removeItem)
                    {
                        hotbar.RemoveItem(stack, hotbar.selectedCell);
                        hotbar.selecter.SetActive(false);
                        hotbar.selectedItem = null;

                        hotbar.RunSelecter(hotbar.selectedCell - 1);
                    }
                }
            }
        }

        cac.transform.parent = null;
        cursor.transform.parent = null;
    }

    //generates the chunk at the position specified
    void GenerateChunk(Vector3 position)
    {
        chunksInWorld.Add(Mathf.RoundToInt(position.x / 32));

        if (right && GameObject.Find("Chunk" + Mathf.RoundToInt(position.x / 32 - 1)) != null)
        {
            structureLeftId = GameObject.Find("Chunk" + Mathf.RoundToInt(position.x / 32 - 1)).GetComponent<ChunkGeneration>().structureLeftIdVar;
            structureLeftColumn = GameObject.Find("Chunk" + Mathf.RoundToInt(position.x / 32 - 1)).GetComponent<ChunkGeneration>().structureLeftColumnVar;
            structureLeftY = GameObject.Find("Chunk" + Mathf.RoundToInt(position.x / 32 - 1)).GetComponent<ChunkGeneration>().structureLeftYVar;
            treeHeight = GameObject.Find("Chunk" + Mathf.RoundToInt(position.x / 32 - 1)).GetComponent<ChunkGeneration>().treeHeightVar;
        }
        else if (!right && GameObject.Find("Chunk" + Mathf.RoundToInt(position.x / 32 + 1)) != null)
        {
            structureLeftId = GameObject.Find("Chunk" + Mathf.RoundToInt(position.x / 32 + 1)).GetComponent<ChunkGeneration>().structureLeftIdVar;
            structureLeftColumn = GameObject.Find("Chunk" + Mathf.RoundToInt(position.x / 32 + 1)).GetComponent<ChunkGeneration>().structureLeftColumnVar;
            structureLeftY = GameObject.Find("Chunk" + Mathf.RoundToInt(position.x / 32 + 1)).GetComponent<ChunkGeneration>().structureLeftYVar;
            treeHeight = GameObject.Find("Chunk" + Mathf.RoundToInt(position.x / 32 + 1)).GetComponent<ChunkGeneration>().treeHeightVar;
        }

        if (SaveSystem.LoadChunk(Mathf.RoundToInt(position.x / 32)) != null)
        {
            GameObject chunk = Instantiate(chunkPrefab, new Vector3(position.x - 0.5f, position.y - 0.5f, 0), Quaternion.identity);

            chunk.GetComponent<ChunkGeneration>().chunkData = SaveSystem.LoadChunk(Mathf.RoundToInt(position.x / 32));
            chunk.GetComponent<ChunkGeneration>().dataObject = dataObject.GetComponent<GameData>();
            chunk.GetComponent<ChunkGeneration>().generatorObject = this;
            chunk.GetComponent<ChunkGeneration>().entityScript = entityScript;
            chunk.GetComponent<ChunkGeneration>().player = player;
            chunk.GetComponent<ChunkGeneration>().Load();
        }
        else
        {
            GameObject chunk = Instantiate(chunkPrefab, new Vector3(position.x - 0.5f, position.y - 0.5f, 0), Quaternion.identity);
            System.Random pRandom = new System.Random(Mathf.RoundToInt(seed) + Mathf.RoundToInt(chunk.GetComponent<ChunkGeneration>().chunkCoordX * 32));

            if ((position.x / 32) % 2 == 0)
            {
                chunk.GetComponent<ChunkGeneration>().spawnNote = true;
            }

            if (sampleBiome != null)
            {
                biomeLeft = sampleBiome.GetComponent<ChunkGeneration>().biomeLeft;
                biomeToGenerate = sampleBiome.GetComponent<ChunkGeneration>().biome;
            }

            if (biomeLeft < 1)
            {
                biomeLeft = pRandom.Next(5, 10);
                biomeToGenerate = dataObject.GetComponent<GameData>().biomes[biomeToGenerate].compatBiomes[pRandom.Next(1, dataObject.GetComponent<GameData>().biomes[biomeToGenerate].compatBiomes.Length + 1) - 1].id;
            }

            chunk.GetComponent<ChunkGeneration>().dataObject = dataObject.GetComponent<GameData>();
            chunk.GetComponent<ChunkGeneration>().generatorObject = this;
            chunk.GetComponent<ChunkGeneration>().entityScript = entityScript;
            chunk.GetComponent<ChunkGeneration>().player = player;
            chunk.GetComponent<ChunkGeneration>().biome = biomeToGenerate;
            chunk.GetComponent<ChunkGeneration>().biomeLeft = biomeLeft - 1;
            chunk.GetComponent<ChunkGeneration>().seed = seed;

            chunk.GetComponent<ChunkGeneration>().worldModifier = worldModifier;

            if (structureLeftId != -1)
            {
                if (right)
                {
                    chunk.GetComponent<ChunkGeneration>().WriteStructure(structureLeftId, structureLeftColumn, structureLeftY, treeHeight);
                    structureLeftId = -1;
                    structureLeftColumn = 0;

                }
                else
                {
                    GameObject.Find("Chunk" + (position.x / 32 + 2)).GetComponent<ChunkGeneration>().WriteStructure(structureLeftId, structureLeftColumn, structureLeftY, treeHeight);
                    structureLeftId = -1;
                    structureLeftColumn = 0;
                }
            }

            chunk.GetComponent<ChunkGeneration>().Generate();
        }
    }
}