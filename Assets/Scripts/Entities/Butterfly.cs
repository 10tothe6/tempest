using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butterfly : MonoBehaviour
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

        targetX = 0;
        targetY = 0;

        generator = GameObject.Find("WorldGenerator").GetComponent<Generation>();
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(posX + targetX, posY + targetY, 0), 0.002f);

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

            if (GameObject.Find("UIManager").GetComponent<UIManager>().session.achievements[0] == false)
            {
                StartCoroutine(GameObject.Find("UIManager").GetComponent<UIManager>().Achievement(0));
                Debug.Log("Achievement Get!");
            }

            Destroy(this.gameObject);
        }

        if (Vector3.Distance(transform.position, new Vector3(posX + targetX, posY + targetY, 0)) < 0.1f)
        {
            targetX = Random.Range(-3, 3);
            targetY = Random.Range(-3, 3);

            posX = transform.position.x;
            posY = transform.position.y;
        }
    }
}
