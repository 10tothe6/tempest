using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    public List<Stack> items;
    public List<GameObject> visuals;
    public bool[] cells;
    public RectTransform[] cellPos;

    public Vector3 infoOffset;
    public RectTransform itemInfo;

    public GameObject stackPrefab;
    public Transform container;
    public GameData data;

    public int cellSizeX;
    public int cellSizeY;

    public int placeIndex;
    public int placeX;
    public int placeY;

    public GameObject cursor;
    public RectTransform held;
    private Vector3 placePos;

    public OpenMenu inventory;
    public UIManager ui;

    public Item selectedItem;
    public int selectedCell;
    public GameObject selecter;
    public Generation generator;

    public GameObject blockPreview;

    public bool test;

    private int selecterId;

    private float itemNameHide;

    public void BeginNew()
    {
        items = new List<Stack>();
        visuals = new List<GameObject>();
        cells = new bool[8];
        held = null;

        if (test)
        {
            Stack stack1 = new Stack();
            stack1.count = 4;
            stack1.itemType = 4;
            stack1.i = 0;

            Stack stack2 = new Stack();
            stack2.count = 3;
            stack2.itemType = 39;
            stack2.i = 1;

            AddItem(stack1);
            AddItem(stack2);
        }

        Refresh();

        itemNameHide = Time.time;
    }

    public void BeginLoad()
    {
        visuals = new List<GameObject>();
        cells = new bool[8];
        held = null;

        Refresh();

        itemNameHide = Time.time;
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

        if (itemNameHide < Time.time)
        {
            container.parent.GetChild(3).gameObject.GetComponent<Text>().text = null;
        }

        if (selectedItem != null)
        {
            if (selectedItem.place != 0 && Vector3.Distance(cursor.transform.position, transform.position) < generator.placeDistance || selectedItem.spawnBalloon && Vector3.Distance(cursor.transform.position, transform.position) < generator.placeDistance)
            {
                if (selectedItem.placePreview == null)
                {
                    blockPreview.GetComponent<SpriteRenderer>().sprite = selectedItem.sprite;
                }
                else
                {
                    blockPreview.GetComponent<SpriteRenderer>().sprite = selectedItem.placePreview;
                }
            }
            else
            {
                blockPreview.GetComponent<SpriteRenderer>().sprite = null;
            }
        }
        else
        {
            blockPreview.GetComponent<SpriteRenderer>().sprite = null;
        }

        RunSelecter(selecterId);

        for (int i = 1; i < 9; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                selecterId = i - 1;
                RunSelecter(selecterId);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (selecterId < 7)
            {
                selecterId++;
            }
            else
            {
                selecterId = 0;
            }
            
            RunSelecter(selecterId);
        }
        else if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (selecterId > 0)
            {
                selecterId--;
            }
            else
            {
                selecterId = 7;
            }

            RunSelecter(selecterId);
        }

        if (Input.GetKeyDown("q") && selectedItem != null)
        {
            GameObject dropItem = Instantiate(data.entities[7], new Vector3(GameObject.Find("Player").transform.position.x, GameObject.Find("Player").transform.position.y, 0), Quaternion.identity);
            dropItem.GetComponent<SpriteRenderer>().sprite = selectedItem.sprite;

            dropItem.GetComponent<DroppedItem>().inventory = inventory;
            dropItem.GetComponent<DroppedItem>().hotbar = this;
            dropItem.GetComponent<DroppedItem>().itemId = data.items.IndexOf(selectedItem);
            dropItem.GetComponent<DroppedItem>().count = 1;

            generator.currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(dropItem);
            generator.entityScript.GetComponent<Entities>().entities.Add(dropItem);

            Stack newStack = new Stack();
            newStack.itemType = data.items.IndexOf(selectedItem);
            newStack.count = 1;
            RemoveItem(newStack, selectedCell);
        }

        if (held == null && inventory.held == null && !ui.menuArray[1].activeSelf && !ui.menuArray[10].activeSelf)
        {
            foreach (GameObject currentStack in visuals)
            {
                if (Vector3.Distance(Input.mousePosition, currentStack.transform.position) < 60)
                {
                    int hoverIndex = visuals.IndexOf(currentStack.gameObject);

                    itemInfo.gameObject.SetActive(true);
                    
                    itemInfo.GetChild(1).gameObject.GetComponent<Text>().text = data.items[items[hoverIndex].itemType].name;
                    itemInfo.GetChild(2).gameObject.GetComponent<Text>().text = data.items[items[hoverIndex].itemType].description;
                    

                    if (!data.items[items[hoverIndex].itemType].giveEffect && data.items[items[hoverIndex].itemType].giveHealth == 0)
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
                        else if (data.items[items[hoverIndex].itemType].giveHealth > 0)
                        {
                            itemInfo.GetChild(3).gameObject.GetComponent<Image>().sprite = ui.gameObject.GetComponent<StatusManager>().effectSprites[6];
                            itemInfo.GetChild(3).GetChild(0).gameObject.GetComponent<Text>().text = data.items[items[hoverIndex].itemType].giveHealth.ToString();

                            itemInfo.GetChild(3).gameObject.SetActive(true);
                        }

                        if (data.items[items[hoverIndex].itemType].effects.Length > 1)
                        {
                            itemInfo.GetChild(4).gameObject.GetComponent<Image>().sprite = ui.gameObject.GetComponent<StatusManager>().effectSprites[data.items[items[hoverIndex].itemType].effects[1]];
                            itemInfo.GetChild(4).GetChild(0).gameObject.GetComponent<Text>().text = data.items[items[hoverIndex].itemType].effectTimes[1].ToString() + " sec";

                            itemInfo.GetChild(4).gameObject.SetActive(true);
                        }
                        else if (data.items[items[hoverIndex].itemType].giveHealth > 0 && data.items[items[hoverIndex].itemType].effects.Length > 0)
                        {
                            itemInfo.GetChild(4).gameObject.GetComponent<Image>().sprite = ui.gameObject.GetComponent<StatusManager>().effectSprites[6];
                            itemInfo.GetChild(4).GetChild(0).gameObject.GetComponent<Text>().text = data.items[items[hoverIndex].itemType].giveHealth.ToString();

                            itemInfo.GetChild(4).gameObject.SetActive(true);
                        }

                        if (data.items[items[hoverIndex].itemType].effects.Length > 2)
                        {
                            itemInfo.GetChild(5).gameObject.GetComponent<Image>().sprite = ui.gameObject.GetComponent<StatusManager>().effectSprites[data.items[items[hoverIndex].itemType].effects[2]];
                            itemInfo.GetChild(5).GetChild(0).gameObject.GetComponent<Text>().text = data.items[items[hoverIndex].itemType].effectTimes[2].ToString() + " sec";

                            itemInfo.GetChild(5).gameObject.SetActive(true);
                        }
                        else if (data.items[items[hoverIndex].itemType].giveHealth > 0 && data.items[items[hoverIndex].itemType].effects.Length > 1)
                        {
                            itemInfo.GetChild(5).gameObject.GetComponent<Image>().sprite = ui.gameObject.GetComponent<StatusManager>().effectSprites[6];
                            itemInfo.GetChild(5).GetChild(0).gameObject.GetComponent<Text>().text = data.items[items[hoverIndex].itemType].giveHealth.ToString();

                            itemInfo.GetChild(5).gameObject.SetActive(true);
                        }
                    }

                    break;
                }
                else
                {
                    HideItemInfo();
                }
            }
        }
        else
        {
            HideItemInfo();
        }

        if (Input.GetMouseButtonDown(0) && held == null && inventory.held == null && !ui.menuArray[1].activeSelf)
        {
            foreach (GameObject currentStack in visuals)
            {
                if (Vector3.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.ScreenToWorldPoint(currentStack.transform.position)) < 1)
                {
                    held = currentStack.GetComponent<RectTransform>();
                    placeIndex = visuals.IndexOf(held.gameObject);
                    cells[items[placeIndex].i] = false;

                    held.SetParent(container);
                }
            }
        }
        else if (held != null && inventory.held == null && !ui.menuArray[1].activeSelf)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                bool inCell = false;

                held.SetParent(container);
                foreach (RectTransform currentCell in cellPos)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20 && cells[System.Array.IndexOf(cellPos, currentCell)] == false)
                        {
                            held.anchoredPosition = currentCell.anchoredPosition;
                            items[placeIndex].i = System.Array.IndexOf(cellPos, currentCell);

                            inCell = true;
                        }
                        else if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20)
                        {
                            foreach (Stack currentItem in items)
                            {
                                if (currentItem.itemType == items[placeIndex].itemType && currentItem.i == System.Array.IndexOf(cellPos, currentCell) && currentItem.count <= data.items[currentItem.itemType].maxStackSize - items[placeIndex].count)
                                {
                                    currentItem.count += items[placeIndex].count;
                                    items.Remove(items[placeIndex]);

                                    inCell = true;

                                    break;
                                }
                            }

                            Refresh();
                            inventory.Refresh();
                        }
                    }
                    else if (Input.GetMouseButtonDown(1))
                    {
                        if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20 && cells[System.Array.IndexOf(cellPos, currentCell)] == false)
                        {
                            Stack newStack = new Stack();
                            newStack.itemType = items[placeIndex].itemType;
                            newStack.count = 1;
                            newStack.i = System.Array.IndexOf(cellPos, currentCell);

                            items.Add(newStack);
                            items[placeIndex].count--;

                            Refresh();
                            inventory.Refresh();
                        }
                        else if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20)
                        {
                            Refresh();
                            inventory.Refresh();
                        }
                    }
                }

                if (!inCell && ui.openMenuContainer != null)
                {
                    held.SetParent(ui.openMenuContainer);
                    foreach (RectTransform currentCell in ui.openMenuCellPos)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20 && inventory.cells[System.Array.IndexOf(ui.openMenuCellPos, currentCell)] == false)
                            {
                                Stack heldItem = items[placeIndex];
                                items.Remove(heldItem);
                                inventory.items.Add(heldItem);

                                held.anchoredPosition = currentCell.anchoredPosition;
                                held.SetParent(ui.openMenuContainer.parent);
                                inventory.items[inventory.items.Count - 1].i = System.Array.IndexOf(ui.openMenuCellPos, currentCell);

                                inCell = true;
                                inventory.transition = true;
                            }
                            else if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20)
                            {
                                Stack heldItem = items[placeIndex];

                                bool cellFound = false;
                                foreach (bool _currentCell in inventory.cells)
                                {
                                    if (!_currentCell)
                                    {
                                        heldItem.i = System.Array.IndexOf(inventory.cells, _currentCell);
                                        cellFound = true;

                                        break;
                                    }
                                }

                                if (cellFound)
                                {
                                    items.Remove(heldItem);

                                    inventory.AddItem(heldItem);

                                    held.anchoredPosition = currentCell.anchoredPosition;
                                    held.SetParent(ui.openMenuContainer.parent);

                                    inCell = true;

                                    break;
                                }
                            }
                        }
                        else if (Input.GetMouseButtonDown(1))
                        {
                            if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20 && inventory.cells[System.Array.IndexOf(ui.openMenuCellPos, currentCell)] == false)
                            {
                                Stack newStack = new Stack();
                                newStack.itemType = items[placeIndex].itemType;
                                newStack.count = 1;
                                newStack.i = System.Array.IndexOf(ui.openMenuCellPos, currentCell);

                                inventory.items.Add(newStack);
                                items[placeIndex].count--;

                                Refresh();
                                inventory.Refresh();
                            }
                            else if (Vector3.Distance(held.anchoredPosition, currentCell.anchoredPosition) < 20)
                            {
                                Refresh();
                                inventory.Refresh();
                            }
                        }
                    }
                }

                if (inCell)
                {
                    held = null;

                    Refresh();
                    inventory.Refresh();
                }
                else
                {
                    held.SetParent(container);
                }
            }
        }

        if (held != null)
        {
            held.transform.position = Input.mousePosition;
        }
    }

    void HideItemInfo()
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

    public int GetAmmoType()
    {
        int itemFoundType = -1;
        int[] itemList = new int[8];

        for (int i = 0; i < itemList.Length; i++)
        {
            itemList[i] = -1;
        }

        foreach (Stack currentItem in items)
        {
            itemList[currentItem.i] = currentItem.itemType;
        }

        for (int i = 0; i < itemList.Length; i++)
        {
            if (itemList[i] != -1)
            {
                if (data.items[itemList[i]].isAmmo)
                {
                    itemFoundType = itemList[i];

                    break;
                }
            }
        }

        return itemFoundType;
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

    public Stack RemoveItem(Stack item, int cell)
    {
        if (cell == 0)
        {
            Stack check = CheckForItem(item.itemType);

            if (check != null && item.count > 1)
            {
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
                            RemoveItem(item, 0);
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
        else
        {
            bool found = false;
            int itemLoc = 0;
            foreach (Stack currentItem in items)
            {
                if (currentItem.i == cell - 1 && !found)
                {
                    found = true;
                    itemLoc = items.IndexOf(currentItem);
                }
            }

            if (item.count > 1)
            {
                int countLeft = item.count;
                for (int i = 0; i <= countLeft; i++)
                {
                    if (items[itemLoc].count > 0)
                    {
                        items[itemLoc].count -= 1;

                        if (items[itemLoc].count <= 0)
                        {
                            items.Remove(items[itemLoc]);
                            cells[cell - 1] = false;
                        }
                    }
                    else
                    {
                        if (item.count != 0)
                        {
                            RemoveItem(item, cell);
                        }
                    }
                }

                Refresh();
                return item;
            }
            else if (item.count > 0)
            {
                items[itemLoc].count -= 1;

                if (items[itemLoc].count <= 0)
                {
                    items.Remove(items[itemLoc]);
                    cells[cell - 1] = false;
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
            RectTransform newItem = Instantiate(stackPrefab, container).GetComponent<RectTransform>();
            newItem.position = container.transform.position;
            newItem.anchoredPosition = cellPos[currentStack.i].anchoredPosition;
            newItem.GetComponent<Image>().sprite = data.items[items[i].itemType].sprite;
            newItem.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = items[i].count.ToString();
            visuals.Add(newItem.gameObject);
            cells[currentStack.i] = true;
            i++;
        }

        data.worldData.hotbar = items;
    }

    public void Kill()
    {
        foreach (GameObject currentThing in visuals)
        {
            Destroy(currentThing);
        }
        visuals.Clear();
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

    public void RunSelecter(int cell)
    {
        selecter.SetActive(true);
        bool found = false;
        foreach (Stack currentItem in items)
        {
            if (currentItem.i == cell)
            {
                found = true;
                selectedItem = data.items[currentItem.itemType];
            }
        }
        if (!found)
        {
            selectedItem = null;
        }
        selecter.GetComponent<RectTransform>().anchoredPosition = cellPos[cell].anchoredPosition;
        
        selectedCell = cell + 1;
    }
}
