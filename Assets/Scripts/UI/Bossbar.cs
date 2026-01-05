using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bossbar : MonoBehaviour
{
    public float fullHealth;
    public float health;
    
    void Update()
    {
        transform.localScale = new Vector3(Mathf.Lerp(0, 1, health / fullHealth), 1, 1);
    }
}
