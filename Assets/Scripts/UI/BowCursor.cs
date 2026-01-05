using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowCursor : MonoBehaviour
{
    public Sprite[] chargeSprites;
    public bool isShoot;

    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        anim.SetBool("Shoot", isShoot);
    }
}
