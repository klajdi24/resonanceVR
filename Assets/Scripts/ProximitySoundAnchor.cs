using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProximitySoundAnchor : MonoBehaviour
{
    public Transform player;
    public float minHearDistance = 1.2f;
    public float maxHearDistance = 7.0f;
    [Range(0f, 1f)] public float maxVolume = 0.85f;
    [Range(0.5f, 4f)] public float falloffPower = 1.8f;
    public float volumeLerpSpeed = 6f;

    AudioSource src;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        src.spatialBlend = 1f;
        src.loop = true;
        src.playOnAwake = false;
        src.volume = 0f;

        if (player == null && Camera.main != null)
            player = Camera.main.transform;
    }

    void OnEnable()
    {
        if (src.clip != null && !src.isPlaying) src.Play();
        src.volume = 0f;
    }

    void OnDisable()
    {
        if (src.isPlaying) src.Stop();
    }

    void Update()
    {
        if (player == null || src.clip == null) return;

        float d = Vector3.Distance(player.position, transform.position);

        float target = 0f;
        if (d <= maxHearDistance)
        {
            float t = Mathf.InverseLerp(maxHearDistance, minHearDistance, d);
            t = Mathf.Clamp01(t);
            t = Mathf.Pow(t, falloffPower);
            target = t * maxVolume;
        }

        src.volume = Mathf.Lerp(src.volume, target, Time.deltaTime * volumeLerpSpeed);
    }
}
