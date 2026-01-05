using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    private int timer;
    private bool pickup;

    public int itemId;
    public int count;

    public LayerMask player;
    public Hotbar hotbar;
    public OpenMenu inventory;

    private bool added;

    private GameData data;

    private Stack itemToAdd;

    void Awake()
    {
        timer = 1;
        StartCoroutine("Cooldown");

        added = false;

        data = GameObject.Find("Data").GetComponent<GameData>();
    }

    IEnumerator Cooldown()
    {
        if (timer > 0)
        {
            timer--;
        }
        else
        {
            pickup = true;
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine("Cooldown");
    }

    public void Initialize()
    {
        itemToAdd = new Stack();
        itemToAdd.itemType = itemId;
        itemToAdd.count = count;
    }

    void Update()
    {
        if (pickup)
        {
            bool touchingPlayer = Physics2D.OverlapCircle(transform.position, 0.5f, player);

            if (touchingPlayer)
            {
                foreach (Stack currentItem in hotbar.items)
                {
                    if (currentItem.itemType == itemToAdd.itemType && currentItem.count < data.items[currentItem.itemType].maxStackSize && !added)
                    {
                        if (currentItem.count + itemToAdd.count <= data.items[currentItem.itemType].maxStackSize)
                        {
                            currentItem.count += itemToAdd.count;
                            added = true;

                            itemToAdd.count = 0;
                        }
                        else
                        {
                            itemToAdd.count -= data.items[currentItem.itemType].maxStackSize - currentItem.count;
                            currentItem.count += data.items[currentItem.itemType].maxStackSize - currentItem.count;
                        }

                        hotbar.Refresh();
                    }
                }

                if (!added)
                {
                    if (hotbar.items.Count < 8)
                    {
                        int i = 0;
                        foreach (bool currentCell in hotbar.cells)
                        {
                            if (currentCell == false)
                            {
                                itemToAdd.i = i;

                                break;
                            }

                            i++;
                        }

                        if (!added)
                        {
                            hotbar.AddItem(itemToAdd);
                            added = true;
                        }
                    }
                    else if (inventory.inventoryItems.Count < 24)
                    {
                        foreach (Stack currentItem in inventory.inventoryItems)
                        {
                            if (currentItem.itemType == itemToAdd.itemType && currentItem.count < data.items[currentItem.itemType].maxStackSize && !added)
                            {
                                if (currentItem.count + itemToAdd.count <= data.items[currentItem.itemType].maxStackSize)
                                {
                                    currentItem.count += itemToAdd.count;
                                    added = true;

                                    itemToAdd.count = 0;
                                }
                                else
                                {
                                    itemToAdd.count -= data.items[currentItem.itemType].maxStackSize - currentItem.count;
                                    currentItem.count += data.items[currentItem.itemType].maxStackSize - currentItem.count;
                                }
                            }
                        }

                        if (!added)
                        {
                            bool[] itemList = new bool[24];

                            foreach (Stack currentItem in inventory.inventoryItems)
                            {
                                itemList[currentItem.i] = true;
                            }

                            for (int _i = 0; _i < itemList.Length; _i++)
                            {
                                if (!itemList[_i])
                                {
                                    itemToAdd.i = _i;

                                    break;
                                }
                            }

                            if (!added)
                            {
                                inventory.inventoryItems.Add(itemToAdd);
                                added = true;
                            }
                        }
                    }
                }
            }

            if (added)
            {
                hotbar.generator.gameObject.GetComponent<Entities>().remove.Add(this.gameObject);
                hotbar.generator.currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Remove(this.gameObject);

                if (!hotbar.generator.gameObject.GetComponent<Entities>().entities.Contains(this.gameObject) && !hotbar.generator.currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Contains(this.gameObject))
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
