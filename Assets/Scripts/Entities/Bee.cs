using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour
{
    private Generation generator;

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
        if (Vector3.Distance(transform.position, GameObject.Find("Player").transform.position) < 6)
        {
            transform.position = Vector3.Lerp(transform.position, GameObject.Find("Player").transform.position, 0.015f);

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

            Destroy(this.gameObject);
        }

        if (Vector3.Distance(transform.position, new Vector3(posX + (targetX), posY + (targetY), 0)) < 0.1f)
        {
            targetX = Random.Range(-2, 2);
            targetY = Random.Range(-2, 2);

            posX = transform.position.x;
            posY = transform.position.y;
        }

        if (Vector3.Distance(transform.position, GameObject.Find("Player").transform.position) < 0.5f)
        {
            GameObject.Find("UIManager").GetComponent<HUD>().UpdateHealth(-6);
            healthManager.health = 0;

            transform.position = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
        }
    }
}
