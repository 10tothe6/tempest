using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public UIManager ui;

    public AudioClip[] sfx;
    public AudioClip[] ambient;
    public AudioSource source;
    public AudioSource ambientSource;

    private float ambientTime;
    private int ambientTrack;

    void Update()
    {
        ambientSource.volume = ui.ambientVolume;
        source.volume = ui.soundVolume;

        if (!ui.inGame)
        {
            ambientSource.Stop();
            ambientSource.clip = null;
        }

        if (Time.time > ambientTime && ambientTime != 0 && ui.inGame)
        {
            if (ambientSource.time == 0)
            {
                PlayAmbient(ambientTrack);

                ambientTime = Random.Range(2, 8);
                ambientTrack = Random.Range(1, ambient.Length) - 1;
            }
        }
    }

    public void PlaySound(int index)
    {
        source.clip = sfx[index];
        source.Play();
    }

    public void PlayAmbient(int index)
    {
        ambientSource.clip = ambient[index];
        ambientSource.Play();
    }

    public void StopSound()
    {
        source.Stop();
    }

    public void StopAmbient()
    {
        ambientSource.Stop();
    }

    public void Initialize() 
    {
        ambientTime = Time.time + Random.Range(2, 8);
        ambientTrack = Random.Range(1, ambient.Length) - 1;
    }
}
