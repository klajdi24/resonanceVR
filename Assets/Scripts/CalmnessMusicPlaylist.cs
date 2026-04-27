using UnityEngine;
using System.Collections;

public class CalmnessMusicPlaylist : MonoBehaviour
{
    [Header("Reference")]
    public CalmnessController calmnessController;

    [Header("Start Condition")]
    [Range(0f, 1f)] public float startAtCalmness = 1.0f; 
    public bool startOnlyOnce = true; 

    [Header("Tracks (alternate)")]
    public AudioClip trackA;
    public AudioClip trackB;

    [Header("Audio")]
    public AudioSource musicSource;
    [Range(0f, 1f)] public float volume = 0.8f;

    private bool hasStarted = false;
    private Coroutine playlistRoutine;

    private void Awake()
    {
        if (calmnessController == null)
            calmnessController = FindFirstObjectByType<CalmnessController>();

        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();

        if (musicSource != null)
        {
            musicSource.playOnAwake = false;
            musicSource.loop = false;
            musicSource.spatialBlend = 0f; 
            musicSource.volume = volume;
        }
    }

    private void Update()
    {
        if (hasStarted && startOnlyOnce) return;
        if (calmnessController == null) return;

        if (calmnessController.calmness >= startAtCalmness - 0.0001f)
        {
            StartPlaylist();
        }
    }

    private void StartPlaylist()
    {
        if (hasStarted) return;
        if (musicSource == null || trackA == null || trackB == null) return;

        hasStarted = true;
        playlistRoutine = StartCoroutine(PlaylistLoop());
    }

    private IEnumerator PlaylistLoop()
    {
        while (true)
        {
            yield return PlayTrack(trackA);
            yield return PlayTrack(trackB);
        }
    }

    private IEnumerator PlayTrack(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.volume = volume;
        musicSource.Play();

        
        while (musicSource.isPlaying)
            yield return null;

        
        yield return new WaitForSeconds(0.1f);
    }
}
