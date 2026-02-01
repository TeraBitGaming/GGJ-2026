using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    private AudioSource source;

    [SerializeField]
    private SerializedDictionary<String, AudioClip> audioLibrary;

    [SerializeField]
    private List<String> lib_Keys = new List<String>();

    [SerializeField]
    private List<AudioClip> lib_Values = new List<AudioClip>();

    public bool stopMusic = false;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        for (int index = 0; index < lib_Keys.Count; index++)
        {
            audioLibrary.Add(lib_Keys[index], lib_Values[index]);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
        playSound("Music");
        playSound("Jump");
        playSound("On");
    }

    public void playSound(string soundKey)
    {
        AudioClip soundClip;
        audioLibrary.TryGetValue(soundKey, out soundClip);
        Debug.Log(soundClip.name);
        source.PlayOneShot(soundClip);
    }

    public void muteMainTrack()
    {
        source.Stop();
    }

    void Update()
    {
        if (stopMusic)
        {
            muteMainTrack();
        }
    }
}
