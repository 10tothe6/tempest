using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenMenu : MonoBehaviour
{
    public List<Stack> items;
    public List<Stack> inventoryItems;
    public List<GameObject> visuals;
    public bool[] cells;

    public Vector3 infoOffset;
    public RectTransform itemInfo;

    public GameObject stackPrefab;
    public GameData data;
    public Generation generator;

    public int placeIndex;

    public GameObject cursor;
    public RectTransform held;

    public Hotbar hotbar;
    public UIManager ui;

    public bool transition;

    public GameObject itemEntity;

    public int craftingType;

    public bool smelt;

    public void BeginNew()
    {
        items = new List<Stack>();
        inventoryItems = new List<Stack>();
        visuals = new List<GameObject>();
        cells = new bool[24];
        held = null;

        hotbar.BeginNew();

        Refresh(); 
    }

    public void BeginLoad()
    {
        visuals = new List<GameObject>();
        cells = new bool[24];
        held = null;

        hotbar.BeginLoad();

        Refresh();
    }

    void Update()
    {
        if (ui.session.achievements.Length > 0)
        {
            if (ui.session.achievements[11] == false)
            {
                foreach (Stack currentItem in items)
                {
                    if (currentItem.itemType == 27)
                    {
                        StartCoroutine(ui.Achievement(11));
                        Debug.Log("Achievement Get!");

                        break;
                    }

                }
            }

            if (ui.session.achievements[10] == false)
            {
                foreach (Stack currentItem in items)
                {
                    if (currentItem.itemType == 4)
                    {
                        StartCoroutine(ui.Achievement(10));
                        Debug.Log("Achievement Get!");

                        break;
                    }

                }
            }

            if (ui.session.achievements[4] == false)
            {
                foreach (Stack currentItem in items)
                {
                    if (currentItem.itemType == 47)
                    {
                        StartCoroutine(ui.Achievement(4));
                        Debug.Log("Achievement Get!");

                        break;
                    }

                }
            }
        }

        if (held == null && hotbar.held == null && !ui.menuArray[1].activeSelf)
        {
            foreach (GameObject currentStack in visuals)
            {
                if (Vector3.Distance(Input.mousePosition, currentStack.transform.position) < 60)
                {
                    int hoverIndex = visuals.IndexOf(currentStack.gameObject);

                    itemInfo.gameObject.SetActive(true);

                    itemInfo.GetChild(1).gameObject.GetComponent<Text>().text = data.items[items[hoverIndex].itemType].name;
                    itemInfo.GetChild(2).gameObject.GetComponent<Text>().text = data.items[items[hoverIndex].itemType].description;


                    if (!data.items[items[hoverIndex].itemType].giveEffect)
                    {
                        itemInfo.GetChild(0).gameObject.transform.localScale = new Vector3(itemInfo.GetChild(0).gameObject.transform.localScale.x, 0.5f, 1);
                        itemInfo.anchoredPosition = Input.mousePosition + new Vector3(infoOffset.x, infoOffset.y * 1.2f, infoOffset.z);

                        itemInfo.GetChild(3).gameObject.GetComponent<Image>().sprite = null;
                        itemInfo.GetChild(3).GetChild(0).gameObject.GetComponent<Text>().text = null;
                        itemInfo.GetChild(4).gameObject.GetComponent<Image>().sprite = null;
                        itemInfo.GetChild(4).GetChild(0).gameObject.GetComponent<Text>().text = null;
                        itemInfo.GetChild(5).gameObject.GetComponent<Image>().sprite = null;
                        itemInfo.GetChild(5).GetChild(0).gameObject.GetComponent<Text>().text = null;

                        itemInfo.GetChild(3).gameObject.SetActive(false);
                        itemInfo.GetChild(4).gameObject.SetActive(false);
                        itemInfo.GetChild(5).gameObject.SetActive(false);
                    }
                    else
                    {
                        itemInfo.GetChild(0).gameObject.transform.localScale = new Vector3(itemInfo.GetChild(0).gameObject.transform.localScale.x, 1, 1);
                        itemInfo.anchoredPosition = Input.mousePosition + new Vector3(infoOffset.x, infoOffset.y, infoOffset.z);

                        if (data.items[items[hoverIndex].itemType].effects.Length > 0)
                        {
                            itemInfo.GetChild(3).gameObject.GetComponent<Image>().sprite = ui.gameObject.GetComponent<StatusManager>().effectSprites[data.items[items[hoverIndex].itemType].effects[0]];
                            itemInfo.GetChild(3).GetChild(0).gameObject.GetComponent<Text>().text = data.items[items[hoverIndex].itemType].effectTimes[0].ToString() + " sec";

                            itemInfo.GetChild(3).gameObject.SetActive(true);
                        }

                        if (data.items[items[hoverIndex].itemType].effects.Length > 1)
                        {
                            itemInfo.GetChild(4).gameObject.GetComponent<Image>().sprite = ui.gameObject.GetComponent<StatusManager>().effectSprites[data.items[items[hoverIndex].itemType].effects[1]];
                            itemInfo.GetChild(4).GetChild(0).gameObject.GetComponent<Text>().text = data.items[items[hoverIndex].itemType].effectTimes[1].ToString() + " sec";

                            itemInfo.GetChild(4).gameObject.SetActive(true);
                        }

                        if (data.items[items[hoverIndex].itemType].effects.Length > 2)
                        {
                            itemInfo.GetChild(5).gameObject.GetComponent<Image>().sprite = ui.gameObject.GetComponent<StatusManager>().effectSprites[data.items[items[hoverIndex].itemType].effects[2]];
                            itemInfo.GetChild(5).GetChild(0).gameObject.GetComponent<Text>().text = data.items[items[hoverIndex].itemType].effectTimes[2].ToString() + " sec";

                            itemInfo.GetChild(5).gameObject.SetActive(true);
                        }
                    }

                    break;
                }
                else
                {
                    itemInfo.gameObject.SetActive(false);
                    itemInfo.GetChild(0).gameObject.transform.localScale = new Vector3(itemInfo.GetChild(0).gameObject.transform.localScale.x, 1, 1);

                    itemInfo.GetChild(3).gameObject.GetComponent<Image>().sprite = null;
                    itemInfo.GetChild(3).GetChild(0).gameObject.GetComponent<Text>().text = null;
                    itemInfo.GetChild(4).gameObject.GetComponent<Image>().sprite = null;
                    itemInfo.GetChild(4).GetChild(0).gameObject.GetComponent<Text>().text = null;
                    itemInfo.GetChild(5).gameObject.GetComponent<Image>().sprite = null;
                    itemInfo.GetChild(5).GetChild(0).gameObject.GetComponent<Text>().text = null;

                    itemInfo.GetChild(3).gameObject.SetActive(false);
                    itemInfo.GetChild(4).gameObject.SetActive(false);
                    itemInfo.GetChild(5).gameObject.SetActive(false);
                }
            }
        }

        if (ui.menuArray[11].activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            Craft();

            smelt = false;
        }

        if (ui.menuArray[16].activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            smelt = true;

            Craft();
        }

        if (!transition)
        {
            if (Input.GetMouseButtonDown(0) && held == null && hotbar.held == null && !ui.menuArray[1].activeSelf)
            {
                if (ui.menuArray[8].activeSelf || ui.menuArray[11].activeSelf || ui.menuArray[16].activeSelf)
                {
                    foreach (GameObject currentStack in visuals)
                    {
                        if (Vector3.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.ScreenToWorldPoint(currentStack.transform.position)) < 1)
                        {
                            held = currentStack.GetComponent<RectTransform>();
                            placeIndex = visuals.IndexOf(held.gameObject);
                            cells[items[placeIndex].i] = false;

                            held.SetParent(hotbar.container);
                        }
                    }
                }
            }
            else if (held != null && hotbar.held == null && !ui.menuArray[1].activeSelf)
            {
                if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    bool inCell = false;

                    if (ui.menuArray[8].activeSelf || ui.menuArray[11].activeSelf || ui.menuArray[16].activeSelf)
                    {
                        held.SetParent(hotbar.container);
                        foreach (RectTransform currentCell in hotbar.cellPos)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20 && hotbar.cells[System.Array.IndexOf(hotbar.cellPos, currentCell)] == false)
                                {
                                    Stack heldItem = items[placeIndex];
                                    items.Remove(heldItem);
                                    hotbar.items.Add(heldItem);

                                    held.anchoredPosition = currentCell.anchoredPosition;
                                    hotbar.items[hotbar.items.Count - 1].i = System.Array.IndexOf(hotbar.cellPos, currentCell);

                                    inCell = true;
                                }
                                else if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20)
                                {
                                    Stack heldItem = items[placeIndex];

                                    bool cellFound = false;
                                    foreach (bool _currentCell in hotbar.cells)
                                    {
                                        if (!_currentCell)
                                        {
                                            heldItem.i = System.Array.IndexOf(hotbar.cells, _currentCell);
                                            cellFound = true;

                                            break;
                                        }
                                    }

                                    if (cellFound)
                                    {
                                        items.Remove(heldItem);

                                        hotbar.AddItem(heldItem);

                                        held.anchoredPosition = currentCell.anchoredPosition;
                                        held.SetParent(hotbar.container);

                                        inCell = true;

                                        break;
                                    }
                                }
                            }
                            else if (Input.GetMouseButtonDown(1))
                            {
                                if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20 && hotbar.cells[System.Array.IndexOf(hotbar.cellPos, currentCell)] == false)
                                {
                                    Stack newStack = new Stack();
                                    newStack.itemType = items[placeIndex].itemType;
                                    newStack.count = 1;
                                    newStack.i = System.Array.IndexOf(hotbar.cellPos, currentCell);

                                    hotbar.items.Add(newStack);
                                    items[placeIndex].count--;

                                    Refresh();
                                    hotbar.Refresh();
                                }
                                else if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20)
                                {
                                    Refresh();
                                    hotbar.Refresh();
                                }
                            }
                        }
                    }

                    if (!inCell)
                    {
                        held.SetParent(ui.openMenuContainer);
                        foreach (RectTransform currentCell in ui.openMenuCellPos)
                        {
                            if (Input.GetMouseButtonDown(0))
                            {
                                if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20 && cells[System.Array.IndexOf(ui.openMenuCellPos, currentCell)] == false)
                                {
                                    held.anchoredPosition = currentCell.anchoredPosition;
                                    held.SetParent(ui.openMenuContainer.parent);
                                    items[placeIndex].i = System.Array.IndexOf(ui.openMenuCellPos, currentCell);

                                    inCell = true;
                                }
                                else if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20)
                                {
                                    foreach (Stack currentItem in items)
                                    {
                                        if (currentItem.itemType == items[placeIndex].itemType && currentItem.i == System.Array.IndexOf(ui.openMenuCellPos, currentCell) && currentItem.count < data.items[currentItem.itemType].maxStackSize - items[placeIndex].count)
                                        {
                                            currentItem.count += items[placeIndex].count;
                                            items.Remove(items[placeIndex]);

                                            inCell = true;

                                            break;
                                        }
                                    }

                                    Refresh();
                                    hotbar.Refresh();
                                }
                            }
                            else if (Input.GetMouseButtonDown(1))
                            {
                                if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20 && cells[System.Array.IndexOf(ui.openMenuCellPos, currentCell)] == false)
                                {
                                    Stack newStack = new Stack();
                                    newStack.itemType = items[placeIndex].itemType;
                                    newStack.count = 1;
                                    newStack.i = System.Array.IndexOf(ui.openMenuCellPos, currentCell);

                                    items.Add(newStack);
                                    items[placeIndex].count--;

                                    Refresh();
                                    hotbar.Refresh();
                                }
                                else if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20)
                                {
                                    Refresh();
                                    hotbar.Refresh();
                                }
                            }
                        }
                    }

                    if (inCell)
                    {
                        held = null;

                        Refresh();
                        hotbar.Refresh();
                    }
                    else
                    {
                        held.SetParent(hotbar.container);
                    }
                }
            }
        }
        
        if (held != null)
        {
            held.transform.position = Input.mousePosition;
        }

        transition = false;
    }

    public void AddItem(Stack item)
    {
        Stack check = CheckForItem(item.itemType);
        if (check != null && check.count < data.items[check.itemType].maxStackSize && item.count > 1)
        {
            int countLeft = item.count;
            for (int i = 0; i < countLeft; i++)
            {
                if (check.count < data.items[check.itemType].maxStackSize)
                {
                    check.count += 1;
                    item.count -= 1;
                }
                else
                {
                    if (item.count != 0)
                    {
                        AddItem(item);
                    }
                }
            }
        }
        else if (check != null && check.count < data.items[check.itemType].maxStackSize && item.count > 0)
        {
            check.count += 1;
            item.count -= 1;
        }
        else
        {
            items.Add(item);
        }

        Refresh();
    }

    public Stack RemoveItem(Stack item)
    {
        Stack check = CheckForItem(item.itemType); 

        if (check != null && item.count > 1)
        {
            Debug.Log(1);

            int countLeft = item.count;
            for (int i = 0; i <= countLeft; i++)
            {
                if (check.count > 0)
                {
                    check.count -= 1;

                    if (check.count <= 0)
                    {
                        items.Remove(check);
                        cells[check.i] = false;
                    }
                }
                else
                {
                    if (item.count != 0)
                    {
                        RemoveItem(item);
                    }
                }
            }

            Refresh();
            return item;
        }
        else if (check != null && item.count > 0)
        {
            check.count -= 1;

            if (check.count <= 0)
            {
                items.Remove(check);
                cells[check.i] = false;
            }

            Refresh();
            return item;
        }
        else
        {
            Refresh();
            return null;
        }
        
    }

    public void Refresh()
    {
        int i = 0;

        foreach (GameObject currentThing in visuals)
        {
            Destroy(currentThing);
        }
        visuals.Clear();

        for (int _i = 0; _i < items.Count; _i++)
        {
            if (items[_i].count < 1)
            {
                items.RemoveAt(_i);
            }
        }

        foreach (Stack currentStack in items)
        {
            RectTransform newItem = Instantiate(stackPrefab, ui.openMenuContainer).GetComponent<RectTransform>();
            newItem.position = ui.openMenuContainer.transform.position;
            newItem.anchoredPosition = ui.openMenuCellPos[currentStack.i].anchoredPosition;
            newItem.GetComponent<Image>().sprite = data.items[items[i].itemType].sprite;
            newItem.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = items[i].count.ToString();
            newItem.SetParent(ui.openMenuContainer.parent);
            visuals.Add(newItem.gameObject);
            cells[currentStack.i] = true;
            i++;
        }

        data.worldData.inventory = inventoryItems;
    }

    public void Kill()
    {
        foreach (GameObject currentThing in visuals)
        {
            Destroy(currentThing);
        }
        visuals.Clear();

        hotbar.Kill();
    }

    public Stack CheckForItem(int itemType)
    {
        Stack found = null;
        foreach (Stack currentStack in items)
        {
            if (currentStack.itemType == itemType)
            {
                found = currentStack;
            }
        }

        return found;
    }

    public void DumpMenu()
    {
        List<Stack> remove = new List<Stack>();

        foreach (Stack currentStack in items)
        {
            for (int i = 1; i <= currentStack.count; i++)
            {
                GameObject dropItem = Instantiate(itemEntity, new Vector3(GameObject.Find("Player").transform.position.x, GameObject.Find("Player").transform.position.y, 0), Quaternion.identity);
                dropItem.GetComponent<SpriteRenderer>().sprite = data.items[currentStack.itemType].sprite;

                dropItem.GetComponent<DroppedItem>().inventory = this;
                dropItem.GetComponent<DroppedItem>().hotbar = hotbar;
                dropItem.GetComponent<DroppedItem>().itemId = currentStack.itemType;

                generator.currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(dropItem);
                generator.entityScript.GetComponent<Entities>().entities.Add(dropItem);
            }

            Stack newStack = new Stack();
            newStack.itemType = currentStack.itemType;
            newStack.count = currentStack.count;
            remove.Add(newStack);
        }

        if (remove.Count > 0)
        {
            foreach (Stack _currentStack in remove)
            {
                RemoveItem(_currentStack);
            }
        }
    }

    public void Clean()
    {
        inventoryItems.Clear();
        foreach (Stack currentStack in items)
        {
            Stack newStack = currentStack;
            inventoryItems.Add(newStack);
        }

        foreach (GameObject currentThing in visuals)
        {
            Destroy(currentThing);
        }
        visuals.Clear();

        items.Clear();
        cells = new bool[24];
    }

    public void CleanChest(Chest _chest)
    {
        Chest chest = _chest;

        chest.items.Clear();
        foreach (Stack currentStack in items)
        {
            Stack newStack = currentStack;
            chest.items.Add(newStack);
        }

        foreach (GameObject currentThing in visuals)
        {
            Destroy(currentThing);
        }
        visuals.Clear();

        items.Clear();
        cells = new bool[24];
    }

    public void UnClean()
    {
        items.Clear();
        foreach (Stack currentStack in inventoryItems)
        {
            Stack newStack = currentStack;
            items.Add(newStack);
        }

        Refresh();
    }

    public void UnCleanChest(Chest _chest)
    {
        Chest chest = _chest;

        items.Clear();
        foreach (Stack currentStack in chest.items)
        {
            Stack newStack = currentStack;
            items.Add(newStack);
        }

        Refresh();
    }

    public void Craft()
    {
        if (smelt)
        {
            int[] craftingInput = new int[3];

            int _i = 0;
            for (int i = 0; i < 3; i++)
            {
                craftingInput[i] = -1;
                foreach (Stack currentItem in items)
                {
                    if (currentItem.i == _i)
                    {
                        craftingInput[i] = currentItem.itemType;
                    }
                }

                _i++;
            }

            foreach (Recipe currentRecipe in data.recipes)
            {
                int correctItemCount = 0;
                for (int i = 0; i < 3; i++)
                {
                    int outNumber = 0;
                    if (int.TryParse(currentRecipe.inputs[i], out outNumber) == true)
                    {
                        if (outNumber == craftingInput[i])
                        {
                            correctItemCount++;
                        }
                    }
                    else if (craftingInput[i] != -1)
                    {
                        if (data.items[craftingInput[i]].tags.Contains(currentRecipe.inputs[i]))
                        {
                            correctItemCount++;
                        }
                    }
                }

                if (correctItemCount == 3 && currentRecipe.type == craftingType)
                {
                    correctItemCount = 0;
                    Debug.Log("crafted an item");

                    Stack itemToAdd = new Stack();
                    itemToAdd.itemType = currentRecipe.output;
                    itemToAdd.count = currentRecipe.count;
                    itemToAdd.i = -1;
                    foreach (bool currentCell in hotbar.cells)
                    {
                        if (!currentCell)
                        {
                            itemToAdd.i = System.Array.IndexOf(hotbar.cells, currentCell);
                            Debug.Log(System.Array.IndexOf(hotbar.cells, currentCell));
                            hotbar.AddItem(itemToAdd);

                            break;
                        }
                    }

                    if (itemToAdd.i == -1)
                    {
                        GameObject dropItem = Instantiate(itemEntity, new Vector3(GameObject.Find("Player").transform.position.x, GameObject.Find("Player").transform.position.y, 0), Quaternion.identity);
                        dropItem.GetComponent<SpriteRenderer>().sprite = data.items[itemToAdd.itemType].sprite;

                        dropItem.GetComponent<DroppedItem>().inventory = this;
                        dropItem.GetComponent<DroppedItem>().hotbar = hotbar;
                        dropItem.GetComponent<DroppedItem>().itemId = itemToAdd.itemType;
                        dropItem.GetComponent<DroppedItem>().count = itemToAdd.count;

                        generator.currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(dropItem);
                        generator.entityScript.GetComponent<Entities>().entities.Add(dropItem);

                        dropItem.GetComponent<DroppedItem>().Initialize();
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        Stack itemToRemove = new Stack();
                        itemToRemove.itemType = craftingInput[i];
                        itemToRemove.count = 1;
                        itemToRemove.i = i;

                        RemoveItem(itemToRemove);

                        _i++;
                    }

                    break;
                }
            }
        }
        else
        {
            int[] craftingInput = new int[5];

            int _i = 0;
            for (int i = 0; i < 5; i++)
            {
                craftingInput[i] = -1;
                foreach (Stack currentItem in items)
                {
                    if (currentItem.i == _i)
                    {
                        craftingInput[i] = currentItem.itemType;
                    }
                }

                _i++;
            }

            foreach (Recipe currentRecipe in data.recipes)
            {
                int correctItemCount = 0;
                for (int i = 0; i < 5; i++)
                {
                    int outNumber = 0;
                    if (int.TryParse(currentRecipe.inputs[i], out outNumber) == true)
                    {
                        if (outNumber == craftingInput[i])
                        {
                            correctItemCount++;
                        }
                    }
                    else if (craftingInput[i] != -1)
                    {
                        if (data.items[craftingInput[i]].tags.Contains(currentRecipe.inputs[i]))
                        {
                            correctItemCount++;
                        }
                    }
                }

                if (correctItemCount == 5 && currentRecipe.type == craftingType)
                {
                    correctItemCount = 0;
                    Debug.Log("crafted an item");

                    Stack itemToAdd = new Stack();
                    itemToAdd.itemType = currentRecipe.output;
                    itemToAdd.count = currentRecipe.count;
                    itemToAdd.i = -1;
                    foreach (bool currentCell in hotbar.cells)
                    {
                        if (!currentCell)
                        {
                            itemToAdd.i = System.Array.IndexOf(hotbar.cells, currentCell);
                            Debug.Log(System.Array.IndexOf(hotbar.cells, currentCell));
                            hotbar.AddItem(itemToAdd);

                            break;
                        }
                    }

                    if (itemToAdd.i == -1)
                    {
                        GameObject dropItem = Instantiate(itemEntity, new Vector3(GameObject.Find("Player").transform.position.x, GameObject.Find("Player").transform.position.y, 0), Quaternion.identity);
                        dropItem.GetComponent<SpriteRenderer>().sprite = data.items[itemToAdd.itemType].sprite;

                        dropItem.GetComponent<DroppedItem>().inventory = this;
                        dropItem.GetComponent<DroppedItem>().hotbar = hotbar;
                        dropItem.GetComponent<DroppedItem>().itemId = itemToAdd.itemType;
                        dropItem.GetComponent<DroppedItem>().count = itemToAdd.count;

                        generator.currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(dropItem);
                        generator.entityScript.GetComponent<Entities>().entities.Add(dropItem);

                        dropItem.GetComponent<DroppedItem>().Initialize();
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        Stack itemToRemove = new Stack();
                        itemToRemove.itemType = craftingInput[i];
                        itemToRemove.count = 1;
                        itemToRemove.i = i;

                        RemoveItem(itemToRemove);

                        _i++;
                    }

                    break;
                }
            }
        }
    }
}
