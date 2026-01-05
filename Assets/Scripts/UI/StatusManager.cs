using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StatusManager : MonoBehaviour
{
    public Generation generator;
    public PlayerMove player;
    public Hotbar hotbar;

    public List<Effect> activeEffects;
    public List<int> activeEffectIds;

    public GameObject flyParticleObject;
    public GameObject flyParticle1;
    public GameObject flyParticle2;

    public Image[] effectDisplays;
    public Sprite[] effectSprites;

    public GameObject blueFilter;
    
    public Volume mainVolume;
    public Color blindColor;
    public Color normalColor;

    void Awake()
    {
        player.maxExtraJumps = 1;

        player.flyEffect = false;
        flyParticleObject.SetActive(false);
    }

    void Update()
    {
        if (generator.begin)
        {
            activeEffectIds.Clear();
            foreach (Image currentDisplay in effectDisplays)
            {
                currentDisplay.gameObject.SetActive(false);
            }

            foreach (Effect currentEffect in activeEffects)
            {
                activeEffectIds.Add(currentEffect.id);

                effectDisplays[activeEffects.IndexOf(currentEffect)].gameObject.SetActive(true); 
                effectDisplays[activeEffects.IndexOf(currentEffect)].sprite = effectSprites[currentEffect.id];
            }

            //speed == 0
            if (activeEffectIds.Contains(0))
            {
                player.speed = 14;
                player.rollSpeed = 32;
            }
            else
            {
                player.speed = 8;
                player.rollSpeed = 25;
            }

            //jump boost == 1
            if (activeEffectIds.Contains(1))
            {
                player.jumpHeight = 36;
            }
            else
            {
                player.jumpHeight = 24;
            }

            //regeneration == 2

            //glowing == 3
            if (activeEffectIds.Contains(3))
            {
                player.lightObject.enabled = true;
            }
            else
            {
                player.lightObject.enabled = false;
            }

            //double jump == 4
            if (activeEffectIds.Contains(4))
            {
                player.maxExtraJumps = 2;
            }
            else
            {
                player.maxExtraJumps = 1;
            }

            //defense == 5

            Effect remove = null;

            foreach (Effect currentEffect in activeEffects)
            {
                if (Time.time - currentEffect.timer < 1 && Time.time - currentEffect.timer > -1)
                {
                    Debug.Log($"Effect with id of {currentEffect.id} ran out.");
                    remove = currentEffect;
                }
            }

            if (remove != null)
            {
                activeEffects.Remove(remove);
            }
        }
    }

    public void TurnOnEffect(int effectId)
    {
        Effect newEffect = new Effect();

        newEffect.id = effectId;
        newEffect.timer = -1;

        if (CheckEffect(effectId) == null)
        {
            activeEffects.Add(newEffect);
        }
    }

    public void TurnOffEffect(int effectId)
    {
        if (activeEffects.Count > 0)
        {
            if (CheckEffect(effectId) != null)
            {
                activeEffects.Remove(CheckEffect(effectId));
            }
        }
    }

    public void TriggerEffect(int effectId, float effectTime)
    {
        Effect newEffect = new Effect();

        newEffect.id = effectId;
        newEffect.timer = Time.time + effectTime;

        if (CheckEffect(effectId) == null)
        {
            activeEffects.Add(newEffect);
            Debug.Log($"Effect with id of {effectId} activated.");
        }
    }

    public Effect CheckEffect(int checkId)
    {
        foreach (Effect currentEffect in activeEffects)
        {
            if (currentEffect.id == checkId)
            {
                return currentEffect;
            }
        }

        return null;
    }
}
