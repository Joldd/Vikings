using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager Instance;

    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSFX;

    private List<Sound> L_musics = new List<Sound>();
    private List<Sound> L_SFXs = new List<Sound>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            if (s.isSFX) L_SFXs.Add(s);
            else L_musics.Add(s);
        }

        sliderSFX.value = 0.5f;
        sliderMusic.value = 0.5f;

        Play("Ambiance");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.Name == name);
        if (s != null) s.source.Play();
        else Debug.LogWarning("No soundAttack");
    }

    public void SetVolumeMusic()
    {
        foreach (Sound s in L_musics)
        {
            s.source.volume = sliderMusic.value;
        }
    }

    public void SetVolumeSFX()
    {
        foreach (Sound s in L_SFXs)
        {
            s.source.volume = sliderSFX.value;
        }
    }
}
