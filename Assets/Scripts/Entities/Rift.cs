using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rift : MonoBehaviour
{
    private Generation generator;

    public HealthManager healthManager;
    public float health;

    private bool open;

    void Awake()
    {
        generator = GameObject.Find("WorldGenerator").GetComponent<Generation>();
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, GameObject.Find("Player").transform.position) < 4 && !open)
        {
            open = true;
            GetComponent<Animator>().SetBool("Open", open);

            GameObject newThing = Instantiate(GameObject.Find("Data").GetComponent<GameData>().entities[4], new Vector3(transform.position.x + 2, transform.position.y, 0), Quaternion.identity);
            generator.currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(newThing);
            generator.entityScript.GetComponent<Entities>().entities.Add(newThing);

            GameObject newThing2 = Instantiate(GameObject.Find("Data").GetComponent<GameData>().entities[4], new Vector3(transform.position.x + -2, transform.position.y, 0), Quaternion.identity);
            generator.currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(newThing2);
            generator.entityScript.GetComponent<Entities>().entities.Add(newThing2);

            GameObject newThing3 = Instantiate(GameObject.Find("Data").GetComponent<GameData>().entities[4], new Vector3(transform.position.x, transform.position.y + 2, 0), Quaternion.identity);
            generator.currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(newThing3);
            generator.entityScript.GetComponent<Entities>().entities.Add(newThing3);
        }

        health = healthManager.health;

        if (health < 1)
        {
            Debug.Log("I am currently dead.");

            generator.entityScript.GetComponent<Entities>().entities.Remove(this.gameObject);
            generator.player.GetComponent<PlayerMove>().targets.Remove(this.gameObject);

            foreach (int currentChunk in generator.chunksInWorld)
            {
                if (GameObject.Find("Chunk" + currentChunk).GetComponent<ChunkGeneration>().entities.Contains(this.gameObject))
                {
                    GameObject.Find("Chunk" + currentChunk).GetComponent<ChunkGeneration>().entities.Remove(this.gameObject);
                }
            }

            GameObject dropItem = Instantiate(GameObject.Find("Data").GetComponent<GameData>().entities[7], new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            dropItem.GetComponent<SpriteRenderer>().sprite = GameObject.Find("Data").GetComponent<GameData>().items[44].sprite;

            dropItem.GetComponent<DroppedItem>().inventory = GameObject.Find("Player").GetComponent<OpenMenu>();
            dropItem.GetComponent<DroppedItem>().hotbar = GameObject.Find("Player").GetComponent<Hotbar>();
            dropItem.GetComponent<DroppedItem>().itemId = 44;
            dropItem.GetComponent<DroppedItem>().count = 4;

            GameObject.Find("WorldGenerator").GetComponent<Generation>().currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(dropItem);
            GameObject.Find("WorldGenerator").GetComponent<Generation>().entityScript.GetComponent<Entities>().entities.Add(dropItem);

            dropItem.GetComponent<DroppedItem>().Initialize();

            Destroy(this.gameObject);
        }

        if (Vector3.Distance(transform.position, GameObject.Find("Player").transform.position) < 1)
        {
            GameObject.Find("UIManager").GetComponent<HUD>().UpdateHealth(-30);
        }
    }
}
