using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Experimental.Rendering.LWRP;

public class PlayerMove : MonoBehaviour
{

    public float speed;
    public float rollSpeed;
    private float moveDirX;
    private float moveDirY;

    public Music musicObject;
    public Transform cameraObject;
    public Transform generatorObject;
    public GameData data;
    public Rigidbody2D rb;
    public Audio audioObject;
    public StatusManager effect;

    public bool fly;
    private bool flipX;

    private bool rollTimer;

    public float jumpHeight;
    public int extraJumps;
    public int maxExtraJumps;
    public bool isGrounded;
    public bool touchingWall;
    public bool touchingWallLeft;
    public bool touchingWallRight;
    public Transform left;
    public Transform right;
    public float checkRadius;
    public Vector3 boxDims;
    public LayerMask whatIsGround;
    public LayerMask vehicle;
    public Transform groundCheck;
    public Collider2D onAirship;
    public float friction;

    public GameObject cursor;

    public UIManager ui;
    public HUD hud;

    public bool isMove = true;

    public float takeoff;
    public float landing;

    public Color nightColor;
    public Color midColor;

    public GameObject sunMoon;

    public Sprite sun;
    public Sprite moon;
    public Sprite blueMoon;

    public GameObject sky;
    public bool isDay;
    public float time;

    public GameObject globalLight;

    public bool enableAirships;
    public bool isCrouch;

    public bool interacting;
    public bool dead;

    private bool walkActive;
    public Animator[] playerAnimators;
    public Image[] playerPreviews;

    public int property;
    public float[] h;
    public float[] s;
    public float[] v;

    private bool walking;
    private bool jumping;

    public bool flyEffect;
    public bool climbing;

    public int dayCount;

    public bool blueMoonActive;
    public Color blueMoonColor;
    public Color regColor;

    public System.Random pRandom;

    public bool isSwing;
    public bool isAttack;
    public bool isRoll;
    public bool isShoot;

    public bool shoot;
    public bool shoot2;

    public BoxCollider2D swordTrigger;
    public List<GameObject> targets;

    public GameObject arrowGuide;

    public GameObject storm;
    public bool stormy;
    public bool bossActive;

    public Image[] sliderBackgrounds;

    public UnityEngine.Experimental.Rendering.Universal.Light2D lightObject;

    private float spawnTimer;
    private float spawnInterval;
    private int soulNumber;

    private Vector3 offset;
    public GameObject hurt;

    public Image ammoType;
    public Sprite nullSprite;

    private Vector3 bowDir;
    private float bowPower;
    public float bowMult;
    public bool lockCursor;

    IEnumerator TimeControl()
    {
        if (time <= 480)
        {
            time++;
        }

        yield return new WaitForSeconds(0.6f);

        StartCoroutine("TimeControl");
    }

    public void InitializeMoon()
    {
        pRandom = new System.Random(Mathf.RoundToInt(generatorObject.GetComponent<Generation>().seed));

        blueMoonActive = false;
        stormy = true;
        bossActive = false;

        offset = new Vector3(0, 0, 0);
    }

    public void SetProperty(int value)
    {
        property = value;
    }

    public void ChangeColor(int item)
    {
        if (property == 0)
        {
            h[item] = ui.menuArray[14].transform.GetChild(11).GetChild(item * 3 + property).GetComponent<Slider>().value;

            foreach (Image currentImage in sliderBackgrounds)
            {
                if (System.Array.IndexOf(sliderBackgrounds, currentImage) >= item * 2 && System.Array.IndexOf(sliderBackgrounds, currentImage) < (item * 2) + 2)
                {
                    if (System.Array.IndexOf(sliderBackgrounds, currentImage) >= item * 2 + 1 && System.Array.IndexOf(sliderBackgrounds, currentImage) < (item * 2) + 2)
                    {
                        currentImage.color = Color.HSVToRGB(ui.menuArray[14].transform.GetChild(11).GetChild(item * 3 + 0).GetComponent<Slider>().value, ui.menuArray[14].transform.GetChild(11).GetChild(item * 3 + 1).GetComponent<Slider>().value, 1);
                    }
                    else
                    {
                        currentImage.color = Color.HSVToRGB(ui.menuArray[14].transform.GetChild(11).GetChild(item * 3 + 0).GetComponent<Slider>().value, 1, 1);
                    }
                }
            }

        }
        else if (property == 1)
        {
            s[item] = ui.menuArray[14].transform.GetChild(11).GetChild(item * 3 + property).GetComponent<Slider>().value;

            foreach (Image currentImage in sliderBackgrounds)
            {
                if (System.Array.IndexOf(sliderBackgrounds, currentImage) >= item * 2 && System.Array.IndexOf(sliderBackgrounds, currentImage) < (item * 2) + 2)
                {
                    if (System.Array.IndexOf(sliderBackgrounds, currentImage) >= item * 2 + 1 && System.Array.IndexOf(sliderBackgrounds, currentImage) < (item * 2) + 2)
                    {
                        currentImage.color = Color.HSVToRGB(ui.menuArray[14].transform.GetChild(11).GetChild(item * 3 + 0).GetComponent<Slider>().value, ui.menuArray[14].transform.GetChild(11).GetChild(item * 3 + 1).GetComponent<Slider>().value, 1);
                    }
                    else
                    {
                        currentImage.color = Color.HSVToRGB(ui.menuArray[14].transform.GetChild(11).GetChild(item * 3 + 0).GetComponent<Slider>().value, 1, 1);
                    }
                }
            }
        }
        else if (property == 2)
        {
            v[item] = ui.menuArray[14].transform.GetChild(11).GetChild(item * 3 + property).GetComponent<Slider>().value;

            foreach (Image currentImage in sliderBackgrounds)
            {
                if (System.Array.IndexOf(sliderBackgrounds, currentImage) >= item * 2 && System.Array.IndexOf(sliderBackgrounds, currentImage) < (item * 2) + 2)
                {
                    if (System.Array.IndexOf(sliderBackgrounds, currentImage) >= item * 2 + 1 && System.Array.IndexOf(sliderBackgrounds, currentImage) < (item * 2) + 2)
                    {
                        currentImage.color = Color.HSVToRGB(ui.menuArray[14].transform.GetChild(11).GetChild(item * 3 + 0).GetComponent<Slider>().value, ui.menuArray[14].transform.GetChild(11).GetChild(item * 3 + 1).GetComponent<Slider>().value, 1);
                    }
                    else
                    {
                        currentImage.color = Color.HSVToRGB(ui.menuArray[14].transform.GetChild(11).GetChild(item * 3 + 0).GetComponent<Slider>().value, 1, 1);
                    }
                }
            }
        }

        playerAnimators[item].gameObject.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(h[item], s[item], v[item]);
        playerPreviews[item].color = Color.HSVToRGB(h[item], s[item], v[item]);
    }

    IEnumerator SpawnArrow(int ammoType, int bowType)
    {
        lockCursor = false;
        GameObject.Find("Cursor").GetComponent<BowCursor>().isShoot = true;

        yield return new WaitUntil(() => !Input.GetMouseButton(1));

        GameObject.Find("Cursor").GetComponent<BowCursor>().isShoot = false;

        Stack newStack = new Stack();
        newStack.itemType = GetComponent<Hotbar>().GetAmmoType();
        newStack.count = 1;

        this.gameObject.GetComponent<Hotbar>().RemoveItem(newStack, 0);

        isShoot = false;
        isAttack = false;

        GameObject guide = Instantiate(arrowGuide, transform.position, Quaternion.identity);
        if (cursor.transform.position.x < transform.position.x && !flipX || cursor.transform.position.x > transform.position.x && flipX)
        {
            guide.transform.up = bowDir * -1;
        }
        else
        {
            guide.transform.up = bowDir;
        }

        if (bowType == 59)
        {
            guide.GetComponent<Rigidbody2D>().gravityScale = 0;
        }
        else
        {
            guide.GetComponent<Rigidbody2D>().gravityScale = 1;
        }

        guide.GetComponent<Rigidbody2D>().AddForce(guide.transform.up * ((1 + bowPower) * bowMult), ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.2f);

        GameObject newArrow = Instantiate(data.entities[9], transform.position, Quaternion.identity);
        newArrow.transform.up = guide.transform.position - newArrow.transform.position;

        if (bowType == 59)
        {
            newArrow.GetComponent<Arrow>().grav = false;
        }
        else
        {
            newArrow.GetComponent<Arrow>().grav = true;
        }

        newArrow.GetComponent<Rigidbody2D>().AddForce(newArrow.transform.up * 20, ForceMode2D.Impulse);

        newArrow.GetComponent<SpriteRenderer>().sprite = data.items[ammoType].sprite;
        newArrow.GetComponent<Arrow>().guide = guide;
        newArrow.GetComponent<Arrow>().damage = data.items[ammoType].damage;
        newArrow.GetComponent<Arrow>().mult = data.items[bowType].damage;

        if (ammoType == 55)
        {
            newArrow.GetComponent<Arrow>().glow = true;
        }
        else
        {
            newArrow.GetComponent<Arrow>().glow = false;
        }

        lockCursor = true;
    }

    void Update()
    {
        bowDir = cursor.transform.position - transform.position;
        bowPower = Vector3.Distance(cursor.transform.position, transform.position);

        if (GetComponent<Hotbar>().selectedItem != null)
        {
            if (GetComponent<Hotbar>().GetAmmoType() != -1 && GetComponent<Hotbar>().selectedItem.isBow && !dead)
            {
                ammoType.sprite = data.items[GetComponent<Hotbar>().GetAmmoType()].sprite;
            }
            else
            {
                ammoType.sprite = nullSprite;
            }
        }

        if (!dead)
        {
            transform.GetChild(8).gameObject.SetActive(false);
        }

        if (stormy)
        {
            storm.transform.position = new Vector3(transform.position.x, storm.transform.position.y, storm.transform.position.z);

            if (Mathf.RoundToInt(transform.position.y - 0.2f) + -128 / 2 >= 128 && !bossActive)
            {
                GameObject boss = Instantiate(data.entities[6], transform.position, Quaternion.identity);

                boss.GetComponent<Wyvern>().target = this.gameObject.transform;
                boss.GetComponent<Wyvern>().startFight = true;

                generatorObject.gameObject.GetComponent<Generation>().currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(boss);
                generatorObject.gameObject.GetComponent<Generation>().entityScript.GetComponent<Entities>().entities.Add(boss);

                bossActive = true;
            }
        }

        data.worldData.h = h;
        data.worldData.s = s;
        data.worldData.v = v;

        foreach (Animator currentAnimator in playerAnimators)
        {
            currentAnimator.SetBool("IsWalk", walking);
            currentAnimator.SetBool("IsJump", jumping);
            currentAnimator.SetBool("IsCrouch", isCrouch);
            currentAnimator.SetBool("IsAttack", isSwing);
            currentAnimator.SetBool("IsShoot", isShoot);
            currentAnimator.SetBool("IsRoll", isRoll);
            currentAnimator.SetBool("IsDead", dead);
        }

        swordTrigger.gameObject.GetComponent<Animator>().SetBool("Attack", isAttack);

        if (GetComponent<Hotbar>().selectedItem != null)
        {
            swordTrigger.gameObject.GetComponent<Animator>().SetInteger("ItemType", data.items.IndexOf(GetComponent<Hotbar>().selectedItem));
        }
        else
        {
            swordTrigger.gameObject.GetComponent<Animator>().SetInteger("ItemType", 0);
        }

        if (GetComponent<Hotbar>().selectedItem != null && !ui.menuArray[1].activeSelf && !ui.menuArray[8].activeSelf && !ui.menuArray[11].activeSelf && !ui.menuArray[12].activeSelf && !ui.menuArray[16].activeSelf)
        {
            if (Input.GetMouseButtonDown(1) && !playerAnimators[0].gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Player_Attack_Robe") && GetComponent<Hotbar>().selectedItem.isSword)
            {
                swordTrigger.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
                isSwing = true;
                isAttack = true;

                foreach (GameObject currentObject in targets)
                {
                    if (currentObject.GetComponent<HealthManager>() != null)
                    {
                        currentObject.GetComponent<HealthManager>().Damage(GetComponent<Hotbar>().selectedItem.damage);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1) && !playerAnimators[0].gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Player_Shoot_Robe") && this.gameObject.GetComponent<Hotbar>().selectedItem.isBow)
            {
                if (GetComponent<Hotbar>().GetAmmoType() != -1)
                {
                    swordTrigger.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
                    isShoot = true;
                    isAttack = true;

                    foreach (Animator currentAnimator in playerAnimators)
                    {
                        currentAnimator.SetBool("IsShoot", true);
                    }

                    StartCoroutine(SpawnArrow(GetComponent<Hotbar>().GetAmmoType(), data.items.IndexOf(GetComponent<Hotbar>().selectedItem)));
                }
                else
                {
                    isSwing = false;

                    swordTrigger.gameObject.GetComponent<SpriteRenderer>().sprite = null;
                }
            }
            else
            {
                isSwing = false;

                swordTrigger.gameObject.GetComponent<SpriteRenderer>().sprite = null;
            }
        }
        else
        {
            isSwing = false;

            swordTrigger.gameObject.GetComponent<SpriteRenderer>().sprite = null;
        }

        data.worldData.time = time;
        data.worldData.isDay = isDay;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        if (!isDay)
        {
            musicObject.isDay = false;

            if (time > spawnTimer + spawnInterval && soulNumber < dayCount)
            {
                Debug.Log("Watch Out");

                int spawnChance = UnityEngine.Random.Range(1, 20);
                if (spawnChance > 15 && dayCount > 3)
                {
                    GameObject newThing = Instantiate(data.entities[13], new Vector3(transform.position.x + UnityEngine.Random.Range(-10, 10), transform.position.y + UnityEngine.Random.Range(1, 5), 0), Quaternion.identity);
                    generatorObject.gameObject.GetComponent<Generation>().currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(newThing);
                    generatorObject.gameObject.GetComponent<Generation>().entityScript.GetComponent<Entities>().entities.Add(newThing);
                }
                else if (spawnChance > 7 && dayCount > 2)
                {
                    GameObject newThing = Instantiate(data.entities[5], new Vector3(transform.position.x + UnityEngine.Random.Range(-10, 10), transform.position.y + UnityEngine.Random.Range(1, 5), 0), Quaternion.identity);
                    generatorObject.gameObject.GetComponent<Generation>().currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(newThing);
                    generatorObject.gameObject.GetComponent<Generation>().entityScript.GetComponent<Entities>().entities.Add(newThing);
                }
                else
                {
                    GameObject newThing = Instantiate(data.entities[4], new Vector3(transform.position.x + UnityEngine.Random.Range(-10, 10), transform.position.y + UnityEngine.Random.Range(1, 5), 0), Quaternion.identity);
                    generatorObject.gameObject.GetComponent<Generation>().currentChunk.gameObject.GetComponent<ChunkGeneration>().entities.Add(newThing);
                    generatorObject.gameObject.GetComponent<Generation>().entityScript.GetComponent<Entities>().entities.Add(newThing);
                }

                spawnTimer = time;
                soulNumber++;
            }

            if (time < 360)
            {
                if (time < 240)
                {
                    sky.GetComponent<SpriteRenderer>().color = nightColor;

                    if (transform.position.y > 0)
                    {
                        globalLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = 0.15f;
                    }
                    else
                    {
                        globalLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = 0.15f + (transform.position.y / 150);
                    }

                    sunMoon.transform.localPosition = new Vector3(sunMoon.transform.localPosition.x, 10, sunMoon.transform.localPosition.z);
                }
                else
                {
                    sky.GetComponent<SpriteRenderer>().color = Color.Lerp(nightColor, midColor, (time - 240) / 120);

                    if (transform.position.y > 0)
                    {
                        globalLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = Mathf.Lerp(0.15f, 0.3f, (time - 240) / 120);
                    }
                    else
                    {
                        globalLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = Mathf.Lerp(0.15f + (transform.position.y / 150), 0.3f + (transform.position.y / 150), (time - 240) / 120);
                    }

                    sunMoon.transform.localPosition = new Vector3(sunMoon.transform.localPosition.x, Mathf.Lerp(10, 0, (time - 240) / 120), sunMoon.transform.localPosition.z);
                } 
            }
            else
            {
                sky.GetComponent<SpriteRenderer>().color = Color.Lerp(midColor, data.biomes[generatorObject.gameObject.GetComponent<Generation>().currentBiome].skyColor, (time - 360) / 120);
                
                if (transform.position.y > 0)
                {
                    globalLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = Mathf.Lerp(0.3f, 0.55f, (time - 360) / 120);
                }
                else
                {
                    globalLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = Mathf.Lerp(0.3f + (transform.position.y / 150), 0.55f + (transform.position.y / 150), (time - 360) / 120);
                }

                sunMoon.transform.localPosition = new Vector3(sunMoon.transform.localPosition.x, Mathf.Lerp(0, -10, (time - 360) / 120), sunMoon.transform.localPosition.z);
            }
        }

        if (isDay)
        {
            musicObject.isDay = true;

            if (time < 360)
            {
                if (time < 240)
                {
                    sky.GetComponent<SpriteRenderer>().color = data.biomes[generatorObject.gameObject.GetComponent<Generation>().currentBiome].skyColor;

                    if (transform.position.y > 0)
                    {
                        globalLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = 0.55f;
                    }
                    else
                    {
                        globalLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = 0.55f + (transform.position.y / 150);
                    }

                    sunMoon.transform.localPosition = new Vector3(sunMoon.transform.localPosition.x, 10, sunMoon.transform.localPosition.z);
                }
                else
                {
                    sky.GetComponent<SpriteRenderer>().color = Color.Lerp(data.biomes[generatorObject.gameObject.GetComponent<Generation>().currentBiome].skyColor, midColor, (time - 240) / 120);

                    if (transform.position.y > 0)
                    {
                        globalLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = Mathf.Lerp(0.55f, 0.3f, (time - 240) / 120);
                    }
                    else
                    {
                        globalLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = Mathf.Lerp(0.55f + (transform.position.y / 150), 0.3f + (transform.position.y / 150), (time - 240) / 120);
                    }

                    sunMoon.transform.localPosition = new Vector3(sunMoon.transform.localPosition.x, Mathf.Lerp(10, 0, (time - 240) / 120), sunMoon.transform.localPosition.z);
                }
            }
            else
            {
                sky.GetComponent<SpriteRenderer>().color = Color.Lerp(midColor, nightColor, (time - 360) / 120);

                if (transform.position.y > 0)
                {
                    globalLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = Mathf.Lerp(0.3f, 0.15f, (time - 360) / 120);
                }
                else
                {
                    globalLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().intensity = Mathf.Lerp(0.3f + (transform.position.y / 150), 0.15f + (transform.position.y / 150), (time - 360) / 120);
                }

                sunMoon.transform.localPosition = new Vector3(sunMoon.transform.localPosition.x, Mathf.Lerp(0, -10, (time - 360) / 120), sunMoon.transform.localPosition.z);
            }
        }

        if (time > 480)
        {
            globalLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>().color = regColor;

            isDay = !isDay;
            time = 0;

            if (isDay)
            {
                dayCount++;

                StartCoroutine(ui.NewDay());

                sunMoon.GetComponent<SpriteRenderer>().sprite = sun;

                pRandom = new System.Random(Mathf.RoundToInt(generatorObject.GetComponent<Generation>().seed));
            }

            if (!isDay)
            {
                spawnTimer = time;
                spawnInterval = 60;
                soulNumber = 0;

                sunMoon.GetComponent<SpriteRenderer>().sprite = moon;
            }

            musicObject.StopMusic();
        }

        data.worldData.playerPosX = this.gameObject.transform.position.x;
        data.worldData.playerPosY = this.gameObject.transform.position.y;
        data.worldData.playerPosZ = this.gameObject.transform.position.z;
        data.worldData.health = hud.health;
        data.worldData.hunger = hud.hunger;

        onAirship = Physics2D.OverlapCircle(groundCheck.position, checkRadius * 2, vehicle);
        if (onAirship != null)
        {
            rb.gravityScale = 0;
            takeoff = transform.position.y;
            transform.SetParent(onAirship.gameObject.transform);

            if (onAirship.gameObject.transform.parent.gameObject.GetComponent<Airship>().touchingLever || onAirship.gameObject.transform.parent.gameObject.GetComponent<Airship>().touchingHandle)
            {
                if (Input.GetKeyDown("f"))
                {
                    interacting = !interacting;
                    onAirship.gameObject.transform.parent.gameObject.GetComponent<Airship>().interact = !onAirship.gameObject.transform.parent.gameObject.GetComponent<Airship>().interact;
                }
            }

            if (!onAirship.gameObject.transform.parent.gameObject.GetComponent<Airship>().touchingLever && !onAirship.gameObject.transform.parent.gameObject.GetComponent<Airship>().touchingHandle)
            {
                interacting = false;
                onAirship.gameObject.transform.parent.gameObject.GetComponent<Airship>().interact = false;
            }
        }
        else
        {
            transform.SetParent(null);
        }

        if (ui.GetComponent<UIManager>().menuArray[8].activeSelf || ui.GetComponent<UIManager>().menuArray[11].activeSelf || ui.GetComponent<UIManager>().menuArray[12].activeSelf || ui.GetComponent<UIManager>().menuArray[16].activeSelf || interacting || dead || isCrouch)
        {
            isMove = false;
        }
        else
        {
            isMove = true;
        }

        if (flyEffect || climbing || generatorObject.gameObject.GetComponent<Generation>().spectate)
        {
            fly = true;
        }
        else
        {
            fly = false;
        }

        if (!isDay && blueMoonActive)
        {
            effect.TriggerEffect(4, 2);
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    IEnumerator Walk()
    {
        walkActive = true;

        if (Input.GetKey("a") || Input.GetKey("d"))
        {
            if (isGrounded && isMove)
            {
                audioObject.PlaySound(1);
            }
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine("Walk");
    }

    public IEnumerator Death()
    {
        jumping = false;
        dead = true;

        yield return new WaitForSeconds(2.8f);

        ui.SwitchMenu(10);

        transform.GetChild(8).gameObject.SetActive(true);
    }

    public IEnumerator ScreenShake()
    {
        hurt.SetActive(true);
        offset = new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), 0);

        cameraObject.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-5, 5));

        yield return new WaitForSeconds(0.02f);

        offset = new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), 0);

        cameraObject.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-5, 5));

        yield return new WaitForSeconds(0.02f);

        offset = new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), 0);

        cameraObject.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-5, 5));

        yield return new WaitForSeconds(0.02f);

        offset = new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), 0);

        cameraObject.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-5, 5));

        yield return new WaitForSeconds(0.02f);

        offset = new Vector3(0, 0, 0);
        cameraObject.transform.eulerAngles = new Vector3(0, 0, 0);

        yield return new WaitForSeconds(0.1f);

        hurt.SetActive(false);
    }

    IEnumerator Roll()
    {
        isRoll = true;

        yield return new WaitForSeconds(0.35f);

        isRoll = false;

        yield return new WaitForSeconds(0.8f);

        rollTimer = false;
    }

    void Move()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !rollTimer && walking)
        {
            rollTimer = true;

            StartCoroutine(Roll());
        }

        if (!walkActive)
        {
            StartCoroutine("Walk");
        }

        if (isGrounded)
        {
            if (!walking || !isMove)
            {
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, friction), rb.velocity.y);
            }
        }

        cursor.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
        if (lockCursor)
        {
            cursor.transform.position = new Vector3(Mathf.RoundToInt(cursor.transform.position.x), Mathf.RoundToInt(cursor.transform.position.y), 0);
        }

        moveDirX = Input.GetAxis("Horizontal");
        moveDirY = Input.GetAxis("Vertical");

        cameraObject.position = Vector3.Lerp(cameraObject.position, new Vector3(transform.position.x, transform.position.y, -10) + offset, 0.05f);
        generatorObject.position = new Vector3(transform.position.x, transform.position.y, -10);

        touchingWallLeft = Physics2D.OverlapBox(left.position, boxDims, 1, whatIsGround);
        touchingWallRight = Physics2D.OverlapBox(right.position, boxDims, 1, whatIsGround);

        if (touchingWallRight || touchingWallLeft)
        {
            touchingWall = true;
        }
        else
        {
            touchingWall = false;
        }

        if (moveDirX < 0 && !dead)
        {
            foreach (Animator currentAnimator in playerAnimators)
            {
                currentAnimator.gameObject.GetComponent<SpriteRenderer>().flipX = true;
                flipX = true;
            }

            swordTrigger.gameObject.GetComponent<SpriteRenderer>().flipX = true;
            swordTrigger.offset = new Vector2(-0.6f, 0.25f);
        }

        if (moveDirX > 0 && !dead)
        {
            foreach (Animator currentAnimator in playerAnimators)
            {
                currentAnimator.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                flipX = false;
            }

            swordTrigger.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            swordTrigger.offset = new Vector2(0.6f, 0.25f);
        }

        if (Input.GetKey("s") && isGrounded || Input.GetKey(KeyCode.DownArrow) && isGrounded || Input.GetKey(KeyCode.LeftControl) && isGrounded)
        {
            isCrouch = true;
            walking = false;
        }
        else
        {
            isCrouch = false;
        }

        if (fly && isMove)
        {
            rb.gravityScale = 0;
            rb.drag = 10;

            if (Input.GetKey("a") || Input.GetKey("d"))
            {
                rb.velocity = new Vector3(moveDirX * speed, rb.velocity.y);
            }

            if (Input.GetKey("w") || Input.GetKey(KeyCode.Space))
            {
                rb.velocity = new Vector3(rb.velocity.x, speed);
            }

            if (Input.GetKey("s"))
            {
                rb.velocity = new Vector3(rb.velocity.x, -speed);
            }
        }
        else if (isMove)
        {
            rb.gravityScale = 10;
            rb.drag = 0;

            if (Input.GetKey("a") || Input.GetKey("d") || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                if (!isRoll && !playerAnimators[0].gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Player_Attack_Robe"))
                {
                    rb.velocity = new Vector3(moveDirX * speed, rb.velocity.y);
                    walking = true;
                }
                else if (!playerAnimators[0].gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Player_Attack_Robe"))
                {
                    rb.velocity = new Vector3(moveDirX * rollSpeed, rb.velocity.y);
                    walking = true;
                }
                else
                {
                    walking = false;
                }
            }
            else
            {
                walking = false;
            }

            if (isGrounded)
            {
                jumping = false;

                landing = transform.position.y;
                if (landing + 10 < takeoff)
                {
                    hud.UpdateHealth(Mathf.Round(-(takeoff - landing)));
                }

                if (hud.health < 0)
                {
                    hud.health = 0;
                }
                
                takeoff = landing;
                extraJumps = maxExtraJumps;
            }

            if (Input.GetKeyDown("w") && extraJumps > 0 || Input.GetKeyDown(KeyCode.Space) && extraJumps > 0 || Input.GetKey(KeyCode.UpArrow) && extraJumps > 0)
            {
                jumping = true;

                extraJumps--;
                takeoff = transform.position.y;
                rb.velocity = new Vector3(rb.velocity.x + moveDirX * speed, jumpHeight);
            }
        }
    }
}
