using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTrigger : MonoBehaviour
{
    public PlayerMove player;

    void OnTriggerEnter2D(Collider2D collider)
    {
        player.targets.Add(collider.gameObject);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        player.targets.Remove(collider.gameObject);
    }
}
