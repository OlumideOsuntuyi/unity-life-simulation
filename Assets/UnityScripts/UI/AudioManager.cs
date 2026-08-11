using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private List<Audio> audioList = new();
    [SerializeField] private AudioMixer musicMixer;
    public Dictionary<string, Audio> audios = new();
    [Range(0, 1), SerializeField] private float volume = 1;
    float vol = -1;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        audios = new();
        foreach (var audio in audioList)
        {
            audios.Add(audio.key.ToLower().Trim(), audio);
        }
    }
    private void Update()
    {
        if(vol != volume)
        {
            SetVolume(volume);
            vol = volume;
        }
    }
    internal void SetVolume(float volume)
    {
        this.volume = volume;
        foreach (var src in audios)
        {
            src.Value.source.volume = src.Value.volume * volume;
        }
    }
    public void Load()
    {
        foreach (var kvp in audios)
        {
            kvp.Value.Load();
        }
    }
    public void OnButton()
    {
        Play("on_button");
    }
    public void Play(string key)
    {
        if (audios.TryGetValue(key, out var value))
        {
            value.Play();
        }
    }
    public void Stop(string key)
    {
        if (audios.TryGetValue(key, out var value))
        {
            value.Stop();
        }
    }

    [System.Serializable]
    public class Audio
    {
        public string key;
        public AudioSource source;
        public bool sfx;
        public bool loop;
        public bool forcePlay = true;
        public bool mute;
        [Range(0, 1)] public float volume;
        public void Load()
        {

        }
        public void Play()
        {
            if (!mute)
            {
                if (!forcePlay && source.isPlaying)
                {
                    return;
                }
                if (source.isPlaying && forcePlay)
                {
                    source.Stop();
                }
                source.Play();
            }
            else
            {
                Stop();
            }
        }
        public void Stop()
        {
            if (source.isPlaying)
            {
                source.Stop();
            }
        }
    }
}
