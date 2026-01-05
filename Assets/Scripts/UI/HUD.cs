using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Camera mainCamera;
    public Generation generator;
    public UIManager ui;
    public GameData data;
    public PlayerMove player;
    public Audio audioObject;

    public float health;
    public float maxHealth;

    public bool dead;

    public GameObject healthBar;
    public GameObject hungerBar;
    
    public float hunger;
    public float maxHunger;

    public Sprite[] healthSprites;
    public Sprite[] hungerSprites;

    public void BeginNew()
    {
        health = maxHealth;
        hunger = maxHunger;

        UpdateHealthBar();
        UpdateHungerBar();
        StartCoroutine(DrainHunger());
        StartCoroutine(Heal());
    }

    public void BeginLoad()
    {
        health = data.worldData.health;
        hunger = data.worldData.hunger;

        UpdateHealthBar();
        UpdateHungerBar();
        StartCoroutine(DrainHunger());
        StartCoroutine(Heal());
    }

    void Update()
    {
        if (ui.inGame && !player.dead)
        {
            healthBar.SetActive(true);
            hungerBar.SetActive(true);
        }
        else
        {
            healthBar.SetActive(false);
            hungerBar.SetActive(false);
        }

        UpdateHungerBar();
        UpdateHealthBar();
    }

    public void UpdateHealth(float amount)
    {
        if (health > 0)
        {
            if (health + amount <= maxHealth)
            {
                if (amount < 0)
                {
                    StartCoroutine(player.ScreenShake());
                    audioObject.PlaySound(0);

                    if (this.gameObject.GetComponent<StatusManager>().activeEffectIds.Contains(5))
                    {
                        health += amount / 2;
                    }
                    else
                    {
                        health += amount;
                    }
                }
                else
                {
                    health += amount;
                }
            }
            else
            {
                health = maxHealth;
            }
        }
    }

    public void UpdateHunger(float amount)
    {
        if (hunger + amount <= maxHunger)
        {
            hunger += amount;
        }
        else
        {
            hunger = maxHunger;
        }
    }

    IEnumerator Heal()
    {
        if (health < maxHealth && health > 0)
        {
            if (hunger > (maxHunger / 10) * 9)
            {
                health += 1f;
                hunger -= 0.2f;
            }

            if (this.gameObject.GetComponent<StatusManager>().activeEffectIds.Contains(2))
            {
                health += 2f;
            }
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Heal());
    }

    IEnumerator DrainHunger()
    {
        if (ui.inGame)
        {
            if (hunger > 0)
            {
                hunger--;
            }
            else
            {
                UpdateHealth(-1);
            }
        }

        yield return new WaitForSeconds(20);

        StartCoroutine(DrainHunger());
    }
    
    void UpdateHealthBar()
    {
        if (health > (maxHealth / 10) * 9 && ui.inGame)
        {
            healthBar.GetComponent<Image>().sprite = healthSprites[0];
        }
        else if (health > (maxHealth / 10) * 8 && ui.inGame)
        {
            healthBar.GetComponent<Image>().sprite = healthSprites[1];
        }
        else if (health > (maxHealth / 10) * 7 && ui.inGame)
        {
            healthBar.GetComponent<Image>().sprite = healthSprites[2];
        }
        else if (health > (maxHealth / 10) * 6 && ui.inGame)
        {
            healthBar.GetComponent<Image>().sprite = healthSprites[3];
        }
        else if (health > (maxHealth / 10) * 5 && ui.inGame)
        {
            healthBar.GetComponent<Image>().sprite = healthSprites[4];
        }
        else if (health > (maxHealth / 10) * 4 && ui.inGame)
        {
            healthBar.GetComponent<Image>().sprite = healthSprites[5];
        }
        else if (health > (maxHealth / 10) * 3 && ui.inGame)
        {
            healthBar.GetComponent<Image>().sprite = healthSprites[6];
        }
        else if (health > (maxHealth / 10) * 2 && ui.inGame)
        {
            healthBar.GetComponent<Image>().sprite = healthSprites[7];
        }
        else if (health > (maxHealth / 10) * 1 && ui.inGame)
        {
            healthBar.GetComponent<Image>().sprite = healthSprites[8];
        }
        else if (health > 0 && ui.inGame)
        {
            healthBar.GetComponent<Image>().sprite = healthSprites[9];
        }
        else if (ui.inGame)
        {
            healthBar.GetComponent<Image>().sprite = healthSprites[10];
        }
    }

    void UpdateHungerBar ()
    {
        if (hunger > (maxHunger / 10) * 9 && ui.inGame)
        {
            hungerBar.GetComponent<Image>().sprite = hungerSprites[0];
        }
        else if (hunger > (maxHunger / 10) * 8 && ui.inGame)
        {
            hungerBar.GetComponent<Image>().sprite = hungerSprites[1];
        }
        else if (hunger > (maxHunger / 10) * 7 && ui.inGame)
        {
            hungerBar.GetComponent<Image>().sprite = hungerSprites[2];
        }
        else if (hunger > (maxHunger / 10) * 6 && ui.inGame)
        {
            hungerBar.GetComponent<Image>().sprite = hungerSprites[3];
        }
        else if (hunger > (maxHunger / 10) * 5 && ui.inGame)
        {
            hungerBar.GetComponent<Image>().sprite = hungerSprites[4];
        }
        else if (hunger > (maxHunger / 10) * 4 && ui.inGame)
        {
            hungerBar.GetComponent<Image>().sprite = hungerSprites[5];
        }
        else if (hunger > (maxHunger / 10) * 3 && ui.inGame)
        {
            hungerBar.GetComponent<Image>().sprite = hungerSprites[6];
        }
        else if (hunger > (maxHunger / 10) * 2 && ui.inGame)
        {
            hungerBar.GetComponent<Image>().sprite = hungerSprites[7];
        }
        else if (hunger > (maxHunger / 10) * 1 && ui.inGame)
        {
            hungerBar.GetComponent<Image>().sprite = hungerSprites[8];
        }
        else if (hunger > 0 && ui.inGame)
        {
            hungerBar.GetComponent<Image>().sprite = hungerSprites[9];
        }
        else if (ui.inGame)
        {
            hungerBar.GetComponent<Image>().sprite = hungerSprites[10];
        }
    }
}
