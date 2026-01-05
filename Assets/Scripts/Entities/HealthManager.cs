using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class HealthManager : MonoBehaviour
{
    public float health;
    public UnityEngine.Experimental.Rendering.Universal.Light2D lightObject;

    public bool glow;
    public Color glowColor;

    void Awake()
    {
        if (glow)
        {
            lightObject.intensity = 0.5f;
            lightObject.color = glowColor;
        }
        else
        {
            lightObject.intensity = 0;
            lightObject.color = new Color(255, 255, 255);
        }

        Debug.Log(lightObject.intensity);
        Debug.Log(lightObject.color);
    }

    public void Damage(float amount)
    {
        health -= amount;

        StartCoroutine(Hurt());
    }

    public IEnumerator Hurt()
    { 
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        lightObject.intensity = 0.125f;
        lightObject.color = new Color(255, 0, 0);

        yield return new WaitForSeconds(0.2f);

        if (this != null)
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);

            if (glow)
            {
                lightObject.intensity = 0.5f;
                lightObject.color = glowColor;
            }
            else
            {
                lightObject.intensity = 0;
                lightObject.color = new Color(255, 255, 255);
            }
        }
    }
}
