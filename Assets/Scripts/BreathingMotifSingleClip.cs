using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BreathingMotifSingleClip : MonoBehaviour
{
    [Header("Link to Calmness")]
    public CalmnessController calmnessController;

    [Header("One clip that contains inhale + exhale")]
    public AudioClip fullBreathClip;

    [Header("Breaths Per Minute (BPM)")]
    public float bpmOverload = 22f;  // fast when overwhelmed
    public float bpmCalm = 8f;       // slow when calm

    [Header("Volume")]
    public float volumeOverload = 0.55f;
    public float volumeCalm = 0.25f;

    [Header("Timing")]
    public float randomJitterSeconds = 0.05f; // human variation
    public bool preventOverlap = true;

    private AudioSource src;
    private float nextBreathTime;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        nextBreathTime = Time.time + 0.2f;
    }

    void Update()
    {
        if (calmnessController == null || fullBreathClip == null) return;

        float c = calmnessController.calmness; // 0 overload -> 1 calm

        // Map calmness to breathing pace + volume
        float bpm = Mathf.Lerp(bpmOverload, bpmCalm, c);
        float secondsPerBreath = 60f / Mathf.Max(1f, bpm);
        src.volume = Mathf.Lerp(volumeOverload, volumeCalm, c);

        if (Time.time >= nextBreathTime)
        {
            src.PlayOneShot(fullBreathClip);

            // If the clip is longer than the target breath interval, either overlap or wait longer.
            float clipDur = fullBreathClip.length;
            float baseDelay = preventOverlap ? Mathf.Max(secondsPerBreath, clipDur) : secondsPerBreath;

            float jitter = Random.Range(-randomJitterSeconds, randomJitterSeconds);
            nextBreathTime = Time.time + Mathf.Max(0.05f, baseDelay + jitter);
        }
    }
}

