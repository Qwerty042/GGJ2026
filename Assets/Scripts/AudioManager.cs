using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Track
    {
        public string name;
        public AudioClip clip;
        [HideInInspector] public AudioSource source;
    }

    public List<Track> tracks;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (var track in tracks)
            {
                track.source = gameObject.AddComponent<AudioSource>();
                track.source.clip = track.clip;
                track.source.loop = true; // keeps looping across scenes
                track.source.playOnAwake = false;
                track.source.volume = 0f;
                track.source.Play();
            }

            SetVolume("Background", 1f);
        }
        else
        {
            Destroy(gameObject); // ensure only one persists
        }
    }

    public void Play(string trackName)
    {
        var track = tracks.Find(t => t.name == trackName);
        if (track != null && !track.source.isPlaying)
            track.source.Play();
    }

    public void Stop(string trackName)
    {
        var track = tracks.Find(t => t.name == trackName);
        if (track != null)
            track.source.Stop();
    }

    public void SetVolume(string trackName, float volume)
    {
        var track = tracks.Find(t => t.name == trackName);
        if (track != null)
            track.source.volume = Mathf.Clamp01(volume);
    }
}
