using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wyvern : MonoBehaviour
{
    public Transform target;
    private Generation generator;
    private Credits credits;
    private Animator anim;
    private PlayerMove player;

    public float dashProgress;
    public bool dash;
    public float offset;

    public Vector3 pos1;
    public Vector3 pos2;

    public bool startFight;

    public HealthManager healthManager;
    public float health;

    private bool charge;
    private bool chargeReady;

    private bool dmg;

    private Bossbar bossbar;

    void Awake()
    {
        generator = GameObject.Find("WorldGenerator").GetComponent<Generation>();
        credits = GameObject.Find("UIManager").GetComponent<UIManager>().credits;

        anim = GetComponent<Animator>();

        if (GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player").GetComponent<PlayerMove>();
        }
        else
        {
            player = null;
        }

        bossbar = GameObject.Find("Canvas").transform.GetChild(2).GetChild(0).gameObject.GetComponent<Bossbar>();
        bossbar.gameObject.transform.parent.gameObject.SetActive(true);

        bossbar.fullHealth = health;
    }

    void Update()
    {
        bossbar.health = health;

        anim.SetBool("isDash", dash);
        anim.SetBool("isCharge", charge);
        anim.SetBool("chargeReady", chargeReady);

        if (player != null)
        {
            if (target.position.x > transform.position.x && charge && !dash)
            {
                this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (charge && !dash)
            {
                this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }

            if (startFight)
            {
                StartCoroutine(RunAttack(true));

                startFight = false;
            }

            if (dash)
            {
                transform.position = Vector3.Lerp(pos1, pos2, dashProgress);
            }
            else
            {
                pos2 = new Vector3(0, 0, 0);
                transform.position = Vector3.Lerp(transform.position, target.position, 0.005f);
            }
        }

        health = healthManager.health;

        if (health < 1)
        {
            credits.runCredits = true;

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
            bossbar.gameObject.transform.parent.gameObject.SetActive(false);
        }

        if (GameObject.Find("UIManager") != null)
        {
            if (GameObject.Find("UIManager").GetComponent<UIManager>().inGame)
            {
                if (target == null)
                {
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
                    bossbar.gameObject.transform.parent.gameObject.SetActive(false);

                    GameObject.Find("Player").GetComponent<PlayerMove>().bossActive = false;
                }
                else if (Mathf.RoundToInt(target.position.y - 0.2f) + -128 / 2 < 112)
                {
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
                    bossbar.gameObject.transform.parent.gameObject.SetActive(false);

                    GameObject.Find("Player").GetComponent<PlayerMove>().bossActive = false;
                }
                else if (player.dead)
                {
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
                    bossbar.gameObject.transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }

    IEnumerator RunAttack(bool start)
    {
        float attackNumber = 0;

        if (start)
        {
            attackNumber = 3;
        }
        else
        {
            attackNumber = Random.Range(3, 3);
        }

        float waitTime = 0;

        if (start)
        {
            waitTime = 0;
        }
        else
        {
            waitTime = Random.Range(0.1f, 0.8f);
        }

        yield return new WaitForSeconds(waitTime);

        if (attackNumber == 1)
        {
            //scales
        }
        else if (attackNumber == 2)
        {
            //fireball
        }
        else if (attackNumber == 3)
        {
            StartCoroutine(Charge());
        }
    }

    IEnumerator Charge()
    {
        dash = false;
        chargeReady = true;

        yield return new WaitForSeconds(0.05f);

        charge = true;
        chargeReady = false;

        yield return new WaitForSeconds(0.15f);

        StartCoroutine(DashAttack());

        charge = true;
    }

    IEnumerator DashAttack()
    {
        if (!dash)
        {
            dmg = false;

            pos1 = transform.position;
            pos2 = new Vector3(transform.position.x + offset, target.position.y, 0);
            dashProgress = 0;

            if (target.position.x < transform.position.x && offset > 0)
            {
                offset *= -1;
            }
            else if (target.position.x > transform.position.x && offset < 0)
            {
                offset *= -1;
            }
        }

        if (Vector3.Distance(transform.position, target.position) < 2 && !dmg)
        {
            player.hud.UpdateHealth(-3);

            dmg = true;
        }

        dash = true;
        dashProgress += 0.025f;

        yield return new WaitForSeconds(0.01f);

        if (Vector3.Distance(transform.position, pos2) > 1)
        {
            StartCoroutine(DashAttack());
        }
        else
        {
            StartCoroutine(RunAttack(false));
            dash = false;

            charge = false;
            chargeReady = false;
        }
    }
}
