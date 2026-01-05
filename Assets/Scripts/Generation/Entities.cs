using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entities : MonoBehaviour
{
    public Generation gen;
    public Transform player;
    public GameObject ui;

    public List<GameObject> entities;

    public LayerMask whatIsChunk;
    public float checkRadius;

    public List<GameObject> remove;

    void Awake()
    {
        remove = new List<GameObject>();
        entities = new List<GameObject>();
    }

    void Update()
    {
        if (ui.GetComponent<UIManager>().inGame)
        {
            foreach (GameObject currentMob in entities)
            {
                if (currentMob != null)
                {
                    Collider2D inChunk = Physics2D.OverlapCircle(new Vector3((Mathf.RoundToInt((currentMob.transform.position.x + 0) / 32) * 32) + 0, 0, 0), checkRadius, whatIsChunk);

                    foreach (int currentChunk in gen.chunksInWorld)
                    {
                        GameObject _chunk = GameObject.Find("Chunk" + currentChunk);
                        if (_chunk.GetComponent<ChunkGeneration>().entities.Contains(currentMob))
                        {
                            _chunk.GetComponent<ChunkGeneration>().entities.Remove(currentMob);
                        }
                    }

                    if (inChunk != null)
                    {
                        inChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(currentMob);
                    }
                }
            }
            if (remove.Count > 0)
            {
                foreach (GameObject currentEntity in remove)
                {
                    entities.Remove(currentEntity);
                }
                
                remove.Clear();
            }
        }
        else
        {
            foreach (GameObject currentMob in entities)
            {
                Destroy(currentMob);
            }
            entities.Clear();
        }
    }   
}
