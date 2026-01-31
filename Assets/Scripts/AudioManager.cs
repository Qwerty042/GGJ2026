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

    // Schedule tracks to start slightly in the future
    private double dspStartTime;

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
                track.source.loop = true; // keep looping
                track.source.playOnAwake = false;
                track.source.volume = 0f; // start muted
            }

            // Schedule all tracks to start in sync
            dspStartTime = AudioSettings.dspTime + 0.1; // 100ms delay to schedule
            foreach (var track in tracks)
            {
                track.source.PlayScheduled(dspStartTime);
            }

            // Make one track audible
            SetVolume("Background", 1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Play a track immediately (if not already playing)
    /// </summary>
    public void Play(string trackName)
    {
        var track = tracks.Find(t => t.name == trackName);
        if (track != null && !track.source.isPlaying)
        {
            track.source.PlayScheduled(AudioSettings.dspTime + 0.05);
        }
    }

    /// <summary>
    /// Stop a track
    /// </summary>
    public void Stop(string trackName)
    {
        var track = tracks.Find(t => t.name == trackName);
        if (track != null)
            track.source.Stop();
    }

    /// <summary>
    /// Set volume instantly
    /// </summary>
    public void SetVolume(string trackName, float volume)
    {
        var track = tracks.Find(t => t.name == trackName);
        if (track != null)
            track.source.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// Instantly switch active track (mute all others)
    /// </summary>
    public void SwitchTrack(string trackName)
    {
        foreach (var track in tracks)
        {
            track.source.volume = (track.name == trackName) ? 1f : 0f;
        }
    }

    /// <summary>
    /// Resync all tracks to a reference track
    /// </summary>
    public void ResyncTracks(string referenceTrackName)
    {
        var reference = tracks.Find(t => t.name == referenceTrackName);
        if (reference == null) return;

        float refTime = reference.source.time;
        foreach (var track in tracks)
        {
            if (track.name != referenceTrackName)
                track.source.time = refTime;
        }
    }
}
