using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEditor;

public class UIManager : MonoBehaviour
{
    public SessionData session;

    [Header("Object References")]
    public GameData dataObject;
    public Generation generatorObject;

    public Transform player;

    public Music music;
    public Audio audioObject;

    public Credits credits;
    public Tutorial tutorial;

    public HUD hud;
    public OpenMenu inventoryObject;
    public Hotbar hotbar;

    [Header("UI Elements")]
    public GameObject pause;
    public GameObject[] objectGroups;

    [Header("World Information")]
    public string worldName;
    public Text worldNameField;

    public string seedString;
    public Text seedField;
    public Text seed;

    [Header("Misc.")]
    public GameObject heightWarning;
    public bool warningCoolDown;

    [Header("Saving + Loading")]
    public string[] saveSlots;
    public Text[] displaySlots;

    public int loadId;

    public string appPath;

    [Header("Game Information")]
    private bool begin;
    public bool inGame;

    public int currentMenu;

    [Header("Inventory")]
    public Transform inventory;
    public Transform crafting;
    public Transform smelting;

    public RectTransform openMenuContainer;
    public RectTransform[] openMenuCellPos;

    public Sprite craftingMenuSprite;
    public Sprite chestMenuSprite;
    public Sprite inventoryMenuSprite;

    public Chest chest;
    public bool chestActive;
    public GameObject chestChunk;

    [Header("Transitions")]
    public GameObject transitionPanel;
    public bool transition;
    public float transitionWait;
    private int transitionSwitch;

    public GameObject[] dayTransitionObjects;
    public bool dayTransition;
    public GameObject dayTransitionMain;
    public Color dayTransitionShow;
    public Color dayTransitionHide;

    [Header("Achievements")]
    public Transform achievement;
    public GameObject[] achievements;

    public Sprite unknown;
    public GameObject popup;
    private Vector3 popupTarget;

    public GameObject achievementInfo;
    public Vector3 achievementOffset;

    [Header("Journal")]
    public GameObject journal;

    private bool journalFlip;
    public string[] journalPages;
    private int currentPage;

    [Header("Combat")]
    public GameObject ammoType;
    public GameObject bossbar;

    [Header("Settings")]
    public Slider[] volumeSliders;

    public float musicVolume;
    public float ambientVolume;
    public float soundVolume;
    public float masterVolume;

    void Awake()
    {
        HideMenu(12);
        appPath = Application.persistentDataPath;

        inGame = false;

        saveSlots = new string[12];

        generatorObject.GetComponent<Generation>().player.SetActive(false);
        generatorObject.GetComponent<Generation>().player.transform.position = new Vector3(0, 64, 0);

        if (SaveSystem.LoadSession() != null)
        {
            session = SaveSystem.LoadSession();
        }
        else
        {
            session = new SessionData();

            session.h1 = new float[4];
            session.s1 = new float[4];
            session.v1 = new float[4];

            session.h2 = new float[4];
            session.s2 = new float[4];
            session.v2 = new float[4];

            session.warningActivate = true;

            session.achievements = new bool[16];
            session.biomesVisited = new bool[3];

            session.musicVolume = 1;
            session.ambientVolume = 1;
            session.soundVolume = 1;
            session.masterVolume = 1;
        }

        Begin();
    }

    public void FocusGroup(string input)
    {
        for(int i = 0; i < objectGroups.Length; i++)
        {
            if (objectGroups[i].name == input)
            {
                objectGroups[i].SetActive(true);
            }
            else
            {
                objectGroups[i].SetActive(false);
            }
        }
    }

    public void HideGroup(string input)
    {
        for (int i = 0; i < objectGroups.Length; i++)
        {
            if (objectGroups[i].name == input)
            {
                objectGroups[i].SetActive(false);
            }
        }
    }

    public void ShowGroup(string input)
    {
        for (int i = 0; i < objectGroups.Length; i++)
        {
            if (objectGroups[i].name == input)
            {
                objectGroups[i].SetActive(true);
            }
        }
    }

    public GameObject FindGroup(string input)
    {
        GameObject foundGroup = null;
        for (int i = 0; i < objectGroups.Length; i++)
        {
            if (objectGroups[i].name == input)
            {
                foundGroup = objectGroups[i];
            }
        }

        return foundGroup;
    }

    public void Begin()
    {
        volumeSliders[0].value = session.musicVolume;
        volumeSliders[1].value = session.ambientVolume;
        volumeSliders[2].value = session.soundVolume;
        volumeSliders[3].value = session.masterVolume;

        currentPage = 0;

        if (session.achievements != null)
        {
            foreach (GameObject currentObject in achievements)
            {
                if (session.achievements[System.Array.IndexOf(achievements, currentObject)] == true)
                {
                    currentObject.GetComponent<Image>().sprite = dataObject.GetComponent<GameData>().achievements[System.Array.IndexOf(achievements, currentObject)].icon;
                }
                else
                {
                    currentObject.GetComponent<Image>().sprite = unknown;
                }
            }
        }

        transitionPanel.GetComponent<Image>().color = dayTransitionHide;

        generatorObject.GetComponent<Generation>().worldModifier = -1;
        generatorObject.GetComponent<Generation>().lifeCount = -1;

        generatorObject.GetComponent<Generation>().ability = -1;

        if (SaveSystem.LoadWorldSlots() != null)
        {
            saveSlots = SaveSystem.LoadWorldSlots();
        }

        foreach (Text currentText in displaySlots)
        {
            currentText.text = saveSlots[System.Array.IndexOf(displaySlots, currentText)];
        }

        heightWarning.SetActive(false);

        chestActive = false;
        chestChunk = null;

        foreach (GameObject currentObject in dayTransitionObjects)
        {
            if (currentObject.GetComponent<Text>() != null)
            {
                currentObject.GetComponent<Text>().color = dayTransitionHide;
            }
            else if (currentObject.GetComponent<Image>() != null)
            {
                currentObject.GetComponent<Image>().color = dayTransitionHide;
            }
        }

        music.StopMusic();
        music.isMenu = true;
    }

    void FixedUpdate()
    {
        if (transition)
        {
            transitionPanel.GetComponent<Image>().color = Color.Lerp(transitionPanel.GetComponent<Image>().color, new Color(0, 0, 0, 1), 0.02f);
        }
        else
        {
            transitionPanel.GetComponent<Image>().color = Color.Lerp(transitionPanel.GetComponent<Image>().color, new Color(0, 0, 0, 0), 0.002f);
        }

        if (dayTransition)
        {
            foreach (GameObject currentObject in dayTransitionObjects)
            {
                if (currentObject.GetComponent<Text>() != null)
                {
                    currentObject.GetComponent<Text>().color = Color.Lerp(currentObject.GetComponent<Text>().color, dayTransitionShow, 0.001f);
                }
                else if (currentObject.GetComponent<Image>() != null)
                {
                    currentObject.GetComponent<Image>().color = Color.Lerp(currentObject.GetComponent<Image>().color, dayTransitionShow, 0.001f);
                }
            }
        }
        else
        {
            foreach (GameObject currentObject in dayTransitionObjects)
            {
                if (currentObject.GetComponent<Text>() != null)
                {
                    currentObject.GetComponent<Text>().color = Color.Lerp(currentObject.GetComponent<Text>().color, dayTransitionHide, 0.001f);
                }
                else if (currentObject.GetComponent<Image>() != null)
                {
                    currentObject.GetComponent<Image>().color = Color.Lerp(currentObject.GetComponent<Image>().color, dayTransitionHide, 0.001f);
                }
            }
        }
    }

    void Update()
    {
        //volume control
        masterVolume = volumeSliders[3].value;

        musicVolume = volumeSliders[0].value * masterVolume;
        ambientVolume = volumeSliders[1].value * masterVolume;
        soundVolume = volumeSliders[2].value * masterVolume;

        if (!inGame)
        {
            bossbar.SetActive(false);

            popup.SetActive(false);
            ammoType.SetActive(false);

            HideGroup("Inventory");
            HideGroup("Crafting");

            openMenuContainer = inventory.GetChild(1).GetComponent<RectTransform>();

            openMenuCellPos = new RectTransform[inventory.GetChild(1).childCount];

            for (int i = 0; i < openMenuCellPos.Length; i++)
            {
                openMenuCellPos[i] = inventory.GetChild(1).GetChild(i).GetComponent<RectTransform>();
            }
        }
        else
        {
            ammoType.SetActive(true);

            if (inventory.gameObject.activeSelf)
            {
                inventory.gameObject.SetActive(true);
                openMenuContainer = inventory.GetChild(1).GetComponent<RectTransform>();

                openMenuCellPos = new RectTransform[inventory.GetChild(1).childCount];

                for (int i = 0; i < openMenuCellPos.Length; i++)
                {
                    openMenuCellPos[i] = inventory.GetChild(1).GetChild(i).GetComponent<RectTransform>();
                }
            }
            else
            {
                inventory.gameObject.SetActive(false);
            }

            if (crafting.gameObject.activeSelf)
            {
                crafting.gameObject.SetActive(true);
                openMenuContainer = crafting.GetChild(1).GetComponent<RectTransform>();

                openMenuCellPos = new RectTransform[crafting.GetChild(1).childCount];

                for (int i = 0; i < openMenuCellPos.Length; i++)
                {
                    openMenuCellPos[i] = crafting.GetChild(1).GetChild(i).GetComponent<RectTransform>();
                }
            }
            else
            {
                crafting.gameObject.SetActive(false);
                if (openMenuContainer != null)
                {
                    if (openMenuContainer == crafting.GetChild(1).GetComponent<RectTransform>())
                    {
                        openMenuContainer = null;
                        openMenuCellPos = null;
                    }
                }
            }

            if (smelting.gameObject.activeSelf)
            {
                smelting.gameObject.SetActive(true);
                openMenuContainer = smelting.GetChild(1).GetComponent<RectTransform>();

                openMenuCellPos = new RectTransform[smelting.GetChild(1).childCount];

                for (int i = 0; i < openMenuCellPos.Length; i++)
                {
                    openMenuCellPos[i] = smelting.GetChild(1).GetChild(i).GetComponent<RectTransform>();
                }
            }
            else
            {
                smelting.gameObject.SetActive(false);
                if (openMenuContainer != null)
                {
                    if (openMenuContainer == smelting.GetChild(1).GetComponent<RectTransform>())
                    {
                        openMenuContainer = null;
                        openMenuCellPos = null;
                    }
                }
            }
        }

        if (session.biomesVisited[0] && session.biomesVisited[1] && session.biomesVisited[2] && session.achievements[6] == false)
        {
            StartCoroutine(Achievement(6));
            Debug.Log("Achievement Get!");
        }

        if (achievement.gameObject.activeSelf)
        {
            foreach (GameObject currentObject in achievements)
            {
                if (Vector3.Distance(Camera.main.ScreenToWorldPoint(currentObject.transform.position), Camera.main.ScreenToWorldPoint(Input.mousePosition)) < 1)
                {
                    achievementInfo.SetActive(true);

                    if (dataObject.GetComponent<GameData>().achievements.Count > System.Array.IndexOf(achievements, currentObject))
                    {
                        achievementInfo.transform.GetChild(1).GetComponent<Text>().text = dataObject.GetComponent<GameData>().achievements[System.Array.IndexOf(achievements, currentObject)].name;
                        achievementInfo.transform.GetChild(2).GetComponent<Text>().text = dataObject.GetComponent<GameData>().achievements[System.Array.IndexOf(achievements, currentObject)].description;
                    }
                    else
                    {
                        achievementInfo.transform.GetChild(1).GetComponent<Text>().text = "???";
                        achievementInfo.transform.GetChild(2).GetComponent<Text>().text = "???";
                    }

                    achievementInfo.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition + achievementOffset;

                    break;
                }
                else
                {
                    achievementInfo.SetActive(false);
                }
            }
        }
        else
        {
            achievementInfo.SetActive(false);
        }

        if (popupTarget != null)
        {
            popup.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(popup.GetComponent<RectTransform>().anchoredPosition, popupTarget, 0.005f);
        }

        if (dayTransitionObjects[0].GetComponent<Image>().color.a < 0.05f)
        {
            dayTransitionObjects[0].SetActive(false);
        }
        else
        {
            dayTransitionObjects[0].SetActive(true);
        }

        if (transitionPanel.GetComponent<Image>().color.a < 0.05f)
        {
            transitionPanel.SetActive(false);
        }
        else
        {
            transitionPanel.SetActive(true);
        }

        if (journal.gameObject.activeSelf)
        {
            journal.GetChild(0).gameObject.GetComponent<Animator>().SetBool("Flip", journalFlip);
            if (Input.GetKeyDown("a") && !journalFlip && currentPage > 0)
            {
                journal.GetChild(0).gameObject.GetComponent<Animator>().SetBool("Backwards", false);

                StartCoroutine(JournalFlip(true));
            }

            if (Input.GetKeyDown("d") && !journalFlip && journalPages.Length > currentPage + 2)
            {
                journal.GetChild(0).gameObject.GetComponent<Animator>().SetBool("Backwards", true);

                StartCoroutine(JournalFlip(false));
            }
        }

        //toggle the inventory
        if (Input.GetKeyDown("e") && begin && !crafting.gameObject.activeSelf && !journal.gameObject.activeSelf && !chestActive && !smelting.gameObject.activeSelf)
        {
            inventory.craftingType = -1;

            if (inventory.gameObject.activeSelf)
            {
                inventory.Clean();
            }

            inventory.gameObject.SetActive(!inventory.gameObject.activeSelf);

            openMenuContainer = inventory.GetChild(1).GetComponent<RectTransform>();

            openMenuCellPos = new RectTransform[inventory.GetChild(1).childCount];

            for (int i = 0; i < openMenuCellPos.Length; i++)
            {
                openMenuCellPos[i] = inventory.GetChild(1).GetChild(i).GetComponent<RectTransform>();
            }

            if (inventory.gameObject.activeSelf)
            {
                inventory.UnClean();
            }
        }
        else if (Input.GetKeyDown("e") && begin && !crafting.gameObject.activeSelf && !journal.gameObject.activeSelf && !smelting.gameObject.activeSelf)
        {
            ToggleChest(new Vector2(chest.coordX, chest.coordY), chest.background, false);
        }

        //toggle the crafting menu
        if (Input.GetKeyDown("c") && begin && !inventory.gameObject.activeSelf && !journal.gameObject.activeSelf && !smelting.gameObject.activeSelf && !pause.gameObject.activeSelf)
        {
            inventory.craftingType = 0;

            if (menuArray[11].activeSelf)
            {
                inventory.DumpMenu();
            }

            menuArray[11].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = craftingMenuSprite;
            menuArray[11].SetActive(!menuArray[11].activeSelf);
        }
        else if (Input.GetKeyDown("c") && begin && !menuArray[8].activeSelf && !menuArray[12].activeSelf && !menuArray[1].activeSelf)
        {
            menuArray[16].SetActive(false);
            inventory.DumpMenu();
        }

        //toggle the journal
        if (Input.GetKeyDown("r") && begin && !menuArray[8].activeSelf && !menuArray[11].activeSelf && !menuArray[16].activeSelf)
        {
            //menuArray[12].SetActive(!menuArray[12].activeSelf);
        }

        if (inGame && !player.gameObject.GetComponent<PlayerMove>().dead)
        {
            menuArray[9].SetActive(true);
            menuArray[13].SetActive(true);
        }
        else
        {
            menuArray[9].SetActive(false);
            menuArray[13].SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && begin && inGame)
        {
            menuArray[1].SetActive(!menuArray[1].activeSelf);
        }

        if (hud.health <= 0 && inGame && !player.gameObject.GetComponent<PlayerMove>().dead)
        {
            StartCoroutine(player.gameObject.GetComponent<PlayerMove>().Death());
        }
    }

    public IEnumerator JournalFlip(bool backwards)
    {
        menuArray[12].transform.GetChild(1).gameObject.SetActive(true);

        journalFlip = true;

        menuArray[12].transform.GetChild(1).gameObject.SetActive(false);

        if (backwards)
        {
            currentPage -= 2;
        }
        else
        {
            currentPage += 2;
        }
        yield return new WaitForSeconds(0.5f);

        menuArray[12].transform.GetChild(1).GetChild(0).gameObject.GetComponent<Text>().text = journalPages[currentPage];
        if (journalPages.Length > currentPage + 1)
        {
            menuArray[12].transform.GetChild(1).GetChild(1).gameObject.GetComponent<Text>().text = journalPages[currentPage + 1];
        }
        else
        {
            menuArray[12].transform.GetChild(1).GetChild(1).gameObject.GetComponent<Text>().text = null;
        }
        menuArray[12].transform.GetChild(1).gameObject.SetActive(true);

        journalFlip = false;
    }

    IEnumerator Warning()
    {
        warningCoolDown = true;
        heightWarning.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        heightWarning.SetActive(false);
        warningCoolDown = false;
    }

    IEnumerator Transition()
    {
        transition = true;

        yield return new WaitForSeconds(transitionWait);
        if (transitionSwitch != -1)
        {
            SwitchMenu(transitionSwitch);
        }

        transition = false;
    }

    public void StartTransition(int menuId)
    {
        transitionSwitch = menuId;

        StartCoroutine(Transition());
    }

    public void Warning(int menuId)
    {
        if (session.warningActivate)
        {
            transitionSwitch = 17;

            session.warningActivate = false;
        }
        else
        {
            transitionSwitch = menuId;
        }

        StartCoroutine(Transition());
    }

    public IEnumerator Achievement(int id)
    {
        popupTarget = new Vector3(-1217, 253, 0);

        popup.GetComponent<RectTransform>().anchoredPosition = new Vector3(-1217, 253, 0);
        popup.SetActive(true);

        popup.transform.GetChild(2).gameObject.GetComponent<Text>().text = dataObject.GetComponent<GameData>().achievements[id].name;
        popup.transform.GetChild(3).gameObject.GetComponent<Text>().text = dataObject.GetComponent<GameData>().achievements[id].description;
        session.achievements[id] = true;

        popupTarget = new Vector3(-785, 253, 0);

        yield return new WaitForSeconds(6);

        popupTarget = new Vector3(-1217, 253, 0);

        yield return new WaitForSeconds(6);

        popup.SetActive(false);

        int achievementsCount = 0;
        foreach (bool currentAchievement in session.achievements)
        {
            if (currentAchievement == true)
            {
                achievementsCount++;
            }
        }

        if (achievementsCount == session.achievements.Length - 1 && session.achievements[9] == false)
        {
            StartCoroutine(Achievement(9));
            Debug.Log("Achievement Get!");
        }
    }

    public void SaveColors(int saveSlot)
    {
        if (saveSlot == 0)
        {
            session.h1[0] = menuArray[14].transform.GetChild(11).GetChild(0).gameObject.GetComponent<Slider>().value;
            session.s1[0] = menuArray[14].transform.GetChild(11).GetChild(1).gameObject.GetComponent<Slider>().value;
            session.v1[0] = menuArray[14].transform.GetChild(11).GetChild(2).gameObject.GetComponent<Slider>().value;
            session.h1[1] = menuArray[14].transform.GetChild(11).GetChild(3).gameObject.GetComponent<Slider>().value;
            session.s1[1] = menuArray[14].transform.GetChild(11).GetChild(4).gameObject.GetComponent<Slider>().value;
            session.v1[1] = menuArray[14].transform.GetChild(11).GetChild(5).gameObject.GetComponent<Slider>().value;
            session.h1[2] = menuArray[14].transform.GetChild(11).GetChild(6).gameObject.GetComponent<Slider>().value;
            session.s1[2] = menuArray[14].transform.GetChild(11).GetChild(7).gameObject.GetComponent<Slider>().value;
            session.v1[2] = menuArray[14].transform.GetChild(11).GetChild(8).gameObject.GetComponent<Slider>().value;
            session.h1[3] = menuArray[14].transform.GetChild(11).GetChild(9).gameObject.GetComponent<Slider>().value;
            session.s1[3] = menuArray[14].transform.GetChild(11).GetChild(10).gameObject.GetComponent<Slider>().value;
            session.v1[3] = menuArray[14].transform.GetChild(11).GetChild(11).gameObject.GetComponent<Slider>().value;
        }
        else
        {
            session.h2[0] = menuArray[14].transform.GetChild(11).GetChild(0).gameObject.GetComponent<Slider>().value;
            session.s2[0] = menuArray[14].transform.GetChild(11).GetChild(1).gameObject.GetComponent<Slider>().value;
            session.v2[0] = menuArray[14].transform.GetChild(11).GetChild(2).gameObject.GetComponent<Slider>().value;
            session.h2[1] = menuArray[14].transform.GetChild(11).GetChild(3).gameObject.GetComponent<Slider>().value;
            session.s2[1] = menuArray[14].transform.GetChild(11).GetChild(4).gameObject.GetComponent<Slider>().value;
            session.v2[1] = menuArray[14].transform.GetChild(11).GetChild(5).gameObject.GetComponent<Slider>().value;
            session.h2[2] = menuArray[14].transform.GetChild(11).GetChild(6).gameObject.GetComponent<Slider>().value;
            session.s2[2] = menuArray[14].transform.GetChild(11).GetChild(7).gameObject.GetComponent<Slider>().value;
            session.v2[2] = menuArray[14].transform.GetChild(11).GetChild(8).gameObject.GetComponent<Slider>().value;
            session.h2[3] = menuArray[14].transform.GetChild(11).GetChild(9).gameObject.GetComponent<Slider>().value;
            session.s2[3] = menuArray[14].transform.GetChild(11).GetChild(10).gameObject.GetComponent<Slider>().value;
            session.v2[3] = menuArray[14].transform.GetChild(11).GetChild(11).gameObject.GetComponent<Slider>().value;
        }
    }

    public void LoadColors(int saveSlot)
    {
        if (saveSlot == 0)
        {
            menuArray[14].transform.GetChild(11).GetChild(0).gameObject.GetComponent<Slider>().value = session.h1[0];
            menuArray[14].transform.GetChild(11).GetChild(1).gameObject.GetComponent<Slider>().value = session.s1[0];
            menuArray[14].transform.GetChild(11).GetChild(2).gameObject.GetComponent<Slider>().value = session.v1[0];
            menuArray[14].transform.GetChild(11).GetChild(3).gameObject.GetComponent<Slider>().value = session.h1[1];
            menuArray[14].transform.GetChild(11).GetChild(4).gameObject.GetComponent<Slider>().value = session.s1[1];
            menuArray[14].transform.GetChild(11).GetChild(5).gameObject.GetComponent<Slider>().value = session.v1[1];
            menuArray[14].transform.GetChild(11).GetChild(6).gameObject.GetComponent<Slider>().value = session.h1[2];
            menuArray[14].transform.GetChild(11).GetChild(7).gameObject.GetComponent<Slider>().value = session.s1[2];
            menuArray[14].transform.GetChild(11).GetChild(8).gameObject.GetComponent<Slider>().value = session.v1[2];
            menuArray[14].transform.GetChild(11).GetChild(9).gameObject.GetComponent<Slider>().value = session.h1[3];
            menuArray[14].transform.GetChild(11).GetChild(10).gameObject.GetComponent<Slider>().value = session.s1[3];
            menuArray[14].transform.GetChild(11).GetChild(11).gameObject.GetComponent<Slider>().value = session.v1[3];
        }
        else
        {
            menuArray[14].transform.GetChild(11).GetChild(0).gameObject.GetComponent<Slider>().value = session.h2[0];
            menuArray[14].transform.GetChild(11).GetChild(1).gameObject.GetComponent<Slider>().value = session.s2[0];
            menuArray[14].transform.GetChild(11).GetChild(2).gameObject.GetComponent<Slider>().value = session.v2[0];
            menuArray[14].transform.GetChild(11).GetChild(3).gameObject.GetComponent<Slider>().value = session.h2[1];
            menuArray[14].transform.GetChild(11).GetChild(4).gameObject.GetComponent<Slider>().value = session.s2[1];
            menuArray[14].transform.GetChild(11).GetChild(5).gameObject.GetComponent<Slider>().value = session.v2[1];
            menuArray[14].transform.GetChild(11).GetChild(6).gameObject.GetComponent<Slider>().value = session.h2[2];
            menuArray[14].transform.GetChild(11).GetChild(7).gameObject.GetComponent<Slider>().value = session.s2[2];
            menuArray[14].transform.GetChild(11).GetChild(8).gameObject.GetComponent<Slider>().value = session.v2[2];
            menuArray[14].transform.GetChild(11).GetChild(9).gameObject.GetComponent<Slider>().value = session.h2[3];
            menuArray[14].transform.GetChild(11).GetChild(10).gameObject.GetComponent<Slider>().value = session.s2[3];
            menuArray[14].transform.GetChild(11).GetChild(11).gameObject.GetComponent<Slider>().value = session.v2[3];
        }
    }

    public IEnumerator NewDay()
    {
        dayTransitionMain.SetActive(true);
        dayTransition = true;

        yield return new WaitForSeconds(2.5f);

        dayTransition = false;

        yield return new WaitForSeconds(2.5f);

        dayTransitionMain.SetActive(false);
    }

    public void ToggleChest(Vector2 coords, bool background, bool open)
    {
        if (open)
        {
            chestActive = true;

            chestChunk = generatorObject.GetComponent<Generation>().cursorChunk.gameObject;
            menuArray[8].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = chestMenuSprite;
        }
        else
        {
            chestActive = false;
            menuArray[8].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = inventoryMenuSprite;
        }

        chest = null;

        foreach (Chest currentChest in chestChunk.GetComponent<ChunkGeneration>().chests)
        {
            if (currentChest.coordX == coords.x && currentChest.coordY == coords.y && currentChest.background == background)
            {
                chest = currentChest;

                break;
            }
        }

        if (!open)
        {
            inventory.CleanChest(chest);
        }

        menuArray[8].SetActive(!menuArray[8].activeSelf);

        openMenuContainer = menuArray[8].transform.GetChild(1).GetComponent<RectTransform>();

        openMenuCellPos = new RectTransform[menuArray[8].transform.GetChild(1).childCount];

        for (int i = 0; i < openMenuCellPos.Length; i++)
        {
            openMenuCellPos[i] = menuArray[8].transform.GetChild(1).GetChild(i).GetComponent<RectTransform>();
        }

        if (open)
        {
            inventory.UnCleanChest(chest);
        }
    }

    public void SwitchMenu(int menuId)
    {
        foreach (GameObject currentMenu in menuArray)
        {
            currentMenu.SetActive(false);
        }

        menuArray[menuId].SetActive(true);
        currentMenu = menuId;
    }

    public void HideMenu(int menuId)
    {
        menuArray[menuId].SetActive(false);
        currentMenu = -1;
    }

    public void ChangeWorldHeight(int height)
    {
        generatorObject.GetComponent<Generation>().worldModifier = height;
    }

    public void ChangeLifeCount(int count)
    {
        if (count > 0)
        {
            generatorObject.GetComponent<Generation>().lifeCount = count;
        }
        else
        {
            generatorObject.GetComponent<Generation>().lifeCount = Mathf.Infinity;
        }
    }

    public void StartGameNew()
    {
        generatorObject.GetComponent<Generation>().lifeCount = Mathf.Infinity;
        generatorObject.GetComponent<Generation>().worldModifier = -25;

        int outNumber = 0;
        if (int.TryParse(seedField.text, out outNumber) == true && generatorObject.GetComponent<Generation>().worldModifier != -1 && !string.IsNullOrEmpty(worldNameField.text) && generatorObject.GetComponent<Generation>().lifeCount != -1)
        {
            player.gameObject.GetComponent<PlayerMove>().property = 0;
            player.gameObject.GetComponent<PlayerMove>().h = new float[4];
            player.gameObject.GetComponent<PlayerMove>().s = new float[4];
            player.gameObject.GetComponent<PlayerMove>().v = new float[4];

            for (int i = 0; i < 12; i++)
            {
                menuArray[14].transform.GetChild(11).GetChild(i).GetComponent<Slider>().value = 0;
            }

            foreach (Image currentImage in player.gameObject.GetComponent<PlayerMove>().playerPreviews)
            {
                currentImage.color = Color.HSVToRGB(0, 0, 0);
            }

            foreach (Animator currentAnimator in player.gameObject.GetComponent<PlayerMove>().playerAnimators)
            {
                currentAnimator.gameObject.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(0, 0, 0);
            }

            worldName = worldNameField.text;
            seedString = seedField.text;

            dataObject.GetComponent<GameData>().worldData.worldName = worldName;

            StartTransition(14);

            generatorObject.GetComponent<Generation>().ability = -1;
        }
    }

    public void BeginGameNew()
    {
        string fileName = @"C:\Users\maxim\AppData\LocalLow\Soulflame Games\Tempest\saves\Beta 1.0\";

        foreach (string currentFileName in saveSlots)
        {
            if (currentFileName == worldNameField.text)
            {
                saveSlots[Array.IndexOf(saveSlots, currentFileName)] = null;
            }
        }

        SaveSystem.SaveWorldSlots(saveSlots);

        if (Directory.Exists(fileName + worldNameField.text))
        {
            var _files = Directory.GetFiles(fileName + worldNameField.text);

            for (int i = 0; i < _files.Length; i++)
            {
                File.Delete(_files[i]);
            }

            try
            {
                Directory.Delete(fileName + worldNameField.text);
            }
            catch (Exception e)
            {
                Debug.Log("The deletion failed: {0}" + e.Message);
            }
        }
        else
        {
            Debug.Log("Specified file doesn't exist");
        }

        HideMenu(14);

        int found = -1;
        foreach (string currentSave in saveSlots)
        {
            if (string.IsNullOrEmpty(currentSave))
            {
                found = System.Array.IndexOf(saveSlots, currentSave);
                break;
            }
        }
        if (found != -1)
        {
            loadId = found;
        }

        begin = true;
        inGame = true;
        generatorObject.GetComponent<Generation>().startGen = false;
        generatorObject.GetComponent<Generation>().StartGameNew();

        this.gameObject.GetComponent<HUD>().BeginNew();

        inventory.BeginNew();

        music.isMenu = false;
        music.StopMusic();
        music.isDay = true;
        audioObject.Initialize();
    }

    public void StartGameLoad(int saveId)
    {
        menuArray[10].SetActive(false);

        generatorObject.GetComponent<Generation>().cac.transform.parent = null;
        generatorObject.GetComponent<Generation>().cursor.transform.parent = null;

        if (!String.IsNullOrEmpty(saveSlots[saveId]))
        {
            HideMenu(6);

            loadId = saveId;

            dataObject.GetComponent<GameData>().LoadWorld(saveSlots[saveId]);
            worldName = dataObject.GetComponent<GameData>().worldData.worldName;
            inventory.inventoryItems = dataObject.GetComponent<GameData>().worldData.inventory;
            hotbar.items = dataObject.GetComponent<GameData>().worldData.hotbar;
            inventory.BeginLoad();

            this.gameObject.GetComponent<HUD>().BeginLoad();

            bool dead = false;
            if (hud.health <= 0)
            {
                hud.health = 30;
                hud.hunger = 20;
                dead = true;

                dataObject.GetComponent<GameData>().worldData.lifeCount--;
                dataObject.GetComponent<GameData>().worldData.deathCount++;
            }

            begin = true;
            inGame = true;
            generatorObject.GetComponent<Generation>().startGen = false;
            if (dead)
            {
                generatorObject.GetComponent<Generation>().respawn = true;

                if (!session.achievements[3])
                {
                    if (player.gameObject.GetComponent<PlayerMove>().dayCount == 1 && player.gameObject.GetComponent<PlayerMove>().time < 480 && player.gameObject.GetComponent<PlayerMove>().isDay)
                    {
                        StartCoroutine(Achievement(3));
                        Debug.Log("Achievement Get!");
                    }
                }
            }

            player.gameObject.GetComponent<PlayerMove>().h = dataObject.GetComponent<GameData>().worldData.h;
            player.gameObject.GetComponent<PlayerMove>().s = dataObject.GetComponent<GameData>().worldData.s;
            player.gameObject.GetComponent<PlayerMove>().v = dataObject.GetComponent<GameData>().worldData.v;

            int i = 0;
            foreach (Animator currentAnimator in player.gameObject.GetComponent<PlayerMove>().playerAnimators)
            {
                currentAnimator.gameObject.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(player.gameObject.GetComponent<PlayerMove>().h[i], player.gameObject.GetComponent<PlayerMove>().s[i], player.gameObject.GetComponent<PlayerMove>().v[i]);

                i++;
            }

            generatorObject.GetComponent<Generation>().StartGameLoad();

            music.isMenu = false;
            music.StopMusic();
            music.isDay = dataObject.GetComponent<GameData>().worldData.isDay;
            audioObject.Initialize();
        }
    }

    public void Quit()
    {
        session.musicVolume = musicVolume;
        session.ambientVolume = ambientVolume;
        session.soundVolume = soundVolume;
        session.masterVolume = masterVolume;

        SaveSystem.SaveSession(session);
    }

    public void ExitGame(int respawn)
    {
        audioObject.StopAmbient();
        audioObject.StopSound();

        inGame = false;
        player.gameObject.GetComponent<PlayerMove>().dead = false;

        dayTransition = false;

        foreach (GameObject currentObject in dayTransitionObjects)
        {
            if (currentObject.GetComponent<Text>() != null)
            {
                currentObject.GetComponent<Text>().color = dayTransitionHide;
            }
            else if (currentObject.GetComponent<Image>() != null)
            {
                currentObject.GetComponent<Image>().color = dayTransitionHide;
            }
        }

        generatorObject.GetComponent<Generation>().cac.transform.parent = null;
        generatorObject.GetComponent<Generation>().cursor.transform.parent = null;

        if (menuArray[11].activeSelf)
        {
            inventory.DumpMenu();
        }

        if (menuArray[8].activeSelf)
        {
            if (chestActive)
            {
                chest.items.Clear();
                foreach (Stack currentStack in inventory.items)
                {
                    Stack newStack = currentStack;
                    chest.items.Add(newStack);
                }
            }
            else
            {
                inventory.inventoryItems.Clear();
                foreach (Stack currentStack in inventory.items)
                {
                    Stack newStack = currentStack;
                    inventory.inventoryItems.Add(newStack);
                }
            }
        }

        dataObject.GetComponent<GameData>().worldData.inventory = inventory.inventoryItems;

        foreach (int currentChunk in generatorObject.GetComponent<Generation>().chunksInWorld)
        {
            GameObject chunk = GameObject.Find("Chunk" + currentChunk);
            chunk.GetComponent<ChunkGeneration>().UnLoad(this, chunk.GetComponent<ChunkGeneration>());
        }
        generatorObject.GetComponent<Generation>().chunksInWorld.Clear();

        inventory.Kill();

        SaveSystem.SaveWorld(dataObject.GetComponent<GameData>().worldData);
        SaveSystem.SaveWorldSlots(saveSlots);

        generatorObject.GetComponent<Generation>().startGen = false;

        generatorObject.GetComponent<Generation>().player.SetActive(false);
        generatorObject.GetComponent<Generation>().player.transform.position = new Vector3(0, 64, 0);

        Begin();

        if (respawn == 1)
        {
            StartGameLoad(loadId);
        }
    }
}
