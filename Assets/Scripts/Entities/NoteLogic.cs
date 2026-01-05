using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteLogic : MonoBehaviour
{
    public string noteName;

    public LayerMask player;
    public Generation generation;

    private float a;

    void Awake()
    {
        a = 0;
    }

    void Update()
    {
        bool touchingPlayer = Physics2D.OverlapCircle(transform.position, 0.5f, player);

        if (touchingPlayer)
        {
            if (generation.foundNotes.Count < generation.dataObject.GetComponent<GameData>().notes.Count && !generation.foundNotes.Contains(noteName))
            {
                generation.foundNotes.Add(noteName);
            }
            
            generation.currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Remove(this.gameObject);
            generation.entityScript.GetComponent<Entities>().entities.Remove(this.gameObject);
            Destroy(this.gameObject);
        }

        transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(a) / 2000, transform.position.z);
        a += 0.005f;
    }
}
