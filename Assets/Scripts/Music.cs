using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public UIManager ui;

    public AudioClip[] tracks;
    public AudioSource source;

    public bool isDay;
    public bool isMenu;

    private bool running;
    private float musicVolume;

    void Awake()
    {
        musicVolume = 1;
    }

    void Update()
    {
        if (isMenu && !running)
        {
            PlayMusic(0, 1);
        }
        else if (isDay && !running)
        {
            PlayMusic(1, 1);
        }
        else if (!isDay && !running)
        {
            PlayMusic(2, 1);
        }

        source.volume = ui.musicVolume * musicVolume;
    }

    public void PlayMusic(int track, float volume)
    {
        source.clip = tracks[track];
        musicVolume = volume;
        StartCoroutine(MusicPlayer());

        running = true;
    }

    public void StopMusic()
    {
        source.Stop();
        StopCoroutine(MusicPlayer());

        running = false;
    }

    IEnumerator MusicPlayer()
    {
        yield return new WaitForSeconds(Random.Range(2, 6));

        source.Play();

        yield return new WaitUntil(() => source.time >= source.clip.length);

        yield return new WaitForSeconds(Random.Range(2, 6));

        StopMusic();
    }
}
