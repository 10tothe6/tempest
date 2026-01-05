using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : MonoBehaviour
{
    private Generation generator;
    private Transform player;

    private float targetX;
    private float targetY;

    private float posX;
    private float posY;

    public HealthManager healthManager;
    public float health;

    void Awake()
    {
        posX = transform.position.x;
        posY = transform.position.y;

        targetX = Random.Range(-2, 2);
        targetY = Random.Range(-2, 2);

        generator = GameObject.Find("WorldGenerator").GetComponent<Generation>();
    }

    void Update()
    {
        if (GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player").transform;
        }

        if (player.position.x > transform.position.x && Vector3.Distance(transform.position, player.position) < 8)
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }

        if (Vector3.Distance(transform.position, player.position) < 8)
        {
            transform.position = Vector3.Lerp(transform.position, player.position, 0.005f);

            posX = transform.position.x;
            posY = transform.position.y;

            targetX = Random.Range(-2, 2);
            targetY = Random.Range(-2, 2);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(posX + (targetX), posY + (targetY), 0), 0.001f);
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

            GameObject.Find("UIManager").GetComponent<UIManager>().session.spiritsKilled++;

            if (GameObject.Find("UIManager").GetComponent<UIManager>().session.spiritsKilled >= 10 && GameObject.Find("UIManager").GetComponent<UIManager>().session.achievements[5] == false)
            {
                StartCoroutine(GameObject.Find("UIManager").GetComponent<UIManager>().Achievement(5));
                Debug.Log("Achievement Get!");
            }

            GameObject dropItem = Instantiate(GameObject.Find("Data").GetComponent<GameData>().entities[7], new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            dropItem.GetComponent<SpriteRenderer>().sprite = GameObject.Find("Data").GetComponent<GameData>().items[44].sprite;

            dropItem.GetComponent<DroppedItem>().inventory = GameObject.Find("Player").GetComponent<OpenMenu>();
            dropItem.GetComponent<DroppedItem>().hotbar = GameObject.Find("Player").GetComponent<Hotbar>();
            dropItem.GetComponent<DroppedItem>().itemId = 44;
            dropItem.GetComponent<DroppedItem>().count = 2;

            GameObject.Find("WorldGenerator").GetComponent<Generation>().currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(dropItem);
            GameObject.Find("WorldGenerator").GetComponent<Generation>().entityScript.GetComponent<Entities>().entities.Add(dropItem);

            dropItem.GetComponent<DroppedItem>().Initialize();

            Destroy(this.gameObject);
        }

        if (Vector3.Distance(transform.position, new Vector3(posX + (targetX), posY + (targetY), 0)) < 0.1f)
        {
            targetX = Random.Range(-2, 2);
            targetY = Random.Range(-2, 2);

            posX = transform.position.x;
            posY = transform.position.y;
        }

        if (Vector3.Distance(transform.position, player.position) < 0.5f)
        {
            GameObject.Find("UIManager").GetComponent<HUD>().UpdateHealth(-10);

            transform.position = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
        }
    }
}
