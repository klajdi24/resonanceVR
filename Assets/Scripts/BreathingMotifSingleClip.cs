using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BreathingMotifSingleClip : MonoBehaviour
{
    [Header("Link to Calmness")]
    public CalmnessController calmnessController;

    [Header("One clip that contains inhale + exhale")]
    public AudioClip fullBreathClip;

    [Header("Breaths Per Minute (BPM)")]
    public float bpmOverload = 22f;
    public float bpmCalm = 8f;

    [Header("Volume")]
    public float volumeOverload = 0.55f;
    public float volumeCalm = 0.25f;

    [Header("Timing")]
    public float randomJitterSeconds = 0.05f;
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

        float c = calmnessController.calmness;

        float bpm = Mathf.Lerp(bpmOverload, bpmCalm, c);
        float secondsPerBreath = 60f / Mathf.Max(1f, bpm);

        src.volume = Mathf.Lerp(volumeOverload, volumeCalm, c);

        if (Time.time >= nextBreathTime)
        {
            src.PlayOneShot(fullBreathClip);

            float clipDur = fullBreathClip.length;
            float baseDelay = preventOverlap ? Mathf.Max(secondsPerBreath, clipDur) : secondsPerBreath;

            float jitter = Random.Range(-randomJitterSeconds, randomJitterSeconds);
            nextBreathTime = Time.time + Mathf.Max(0.05f, baseDelay + jitter);
        }
    }
}

