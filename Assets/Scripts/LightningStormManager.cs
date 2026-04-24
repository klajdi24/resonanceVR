using System.Collections.Generic;
using UnityEngine;

public class LightningStormManager : MonoBehaviour
{
    [Header("Calmness")]
    public CalmnessController calmnessController;

    [Tooltip("Lightning stops when calmness reaches this value.")]
    [Range(0f, 1f)] public float stopAtCalmness = 0.99f;

    [Header("Lightning Prefabs")]
    public GameObject[] lightningPrefabs;

    [Tooltip("How long each spawned lightning effect stays before being destroyed.")]
    public float lightningLifetime = 1.5f;

    [Header("Spawn Area")]
    public Vector3 areaSize = new Vector3(8f, 4f, 8f);
    public float minHeight = 1.5f;
    public float maxHeight = 5f;

    [Header("Timing")]
    public float intervalMinAtOverload = 0.6f;
    public float intervalMaxAtOverload = 1.5f;

    public float intervalMinNearCalm = 3f;
    public float intervalMaxNearCalm = 6f;

    [Header("Sound")]
    public AudioClip[] lightningSounds;
    [Range(0f, 1f)] public float volumeAtOverload = 0.8f;
    [Range(0f, 1f)] public float volumeNearCalm = 0.15f;
    public float soundMaxDistance = 18f;

    [Header("Behaviour")]
    public bool clearExistingLightningAtFullCalmness = true;
    public bool stopCompletelyAtFullCalmness = true;

    private float nextStrikeTime;
    private bool stopped;
    private readonly List<GameObject> activeLightning = new List<GameObject>();

    private void Awake()
    {
        if (calmnessController == null)
            calmnessController = FindFirstObjectByType<CalmnessController>();

        ScheduleNextStrike();
    }

    private void Update()
    {
        if (stopped) return;

        float calmness = calmnessController != null
            ? Mathf.Clamp01(calmnessController.calmness)
            : 0f;

        if (calmness >= stopAtCalmness && stopCompletelyAtFullCalmness)
        {
            StopLightningStorm();
            return;
        }

        if (Time.time >= nextStrikeTime)
        {
            SpawnLightning(calmness);
            ScheduleNextStrike();
        }
    }

    private void SpawnLightning(float calmness)
    {
        if (lightningPrefabs == null || lightningPrefabs.Length == 0)
            return;

        GameObject prefab = lightningPrefabs[Random.Range(0, lightningPrefabs.Length)];
        if (prefab == null) return;

        Vector3 pos = GetRandomPosition();

        GameObject lightning = Instantiate(prefab, pos, Random.rotation);
        activeLightning.Add(lightning);

        ParticleSystem[] particles = lightning.GetComponentsInChildren<ParticleSystem>(true);
        foreach (ParticleSystem ps in particles)
        {
            ps.Play();
        }

        PlayLightningSound(pos, calmness);

        Destroy(lightning, lightningLifetime);
        StartCoroutine(RemoveFromListAfterDelay(lightning, lightningLifetime));
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 center = transform.position;

        float x = Random.Range(-areaSize.x * 0.5f, areaSize.x * 0.5f);
        float y = Random.Range(minHeight, maxHeight);
        float z = Random.Range(-areaSize.z * 0.5f, areaSize.z * 0.5f);

        return center + new Vector3(x, y, z);
    }

    private void PlayLightningSound(Vector3 position, float calmness)
    {
        if (lightningSounds == null || lightningSounds.Length == 0)
            return;

        AudioClip clip = lightningSounds[Random.Range(0, lightningSounds.Length)];
        if (clip == null) return;

        float volume = Mathf.Lerp(volumeAtOverload, volumeNearCalm, calmness);

        GameObject soundObj = new GameObject("LightningSound");
        soundObj.transform.position = position;

        AudioSource source = soundObj.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = 1f;
        source.minDistance = 2f;
        source.maxDistance = soundMaxDistance;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.Play();

        Destroy(soundObj, clip.length + 0.2f);
    }

    private void ScheduleNextStrike()
    {
        float calmness = calmnessController != null
            ? Mathf.Clamp01(calmnessController.calmness)
            : 0f;

        float min = Mathf.Lerp(intervalMinAtOverload, intervalMinNearCalm, calmness);
        float max = Mathf.Lerp(intervalMaxAtOverload, intervalMaxNearCalm, calmness);

        nextStrikeTime = Time.time + Random.Range(min, max);
    }

    public void StopLightningStorm()
    {
        stopped = true;

        if (!clearExistingLightningAtFullCalmness)
            return;

        for (int i = activeLightning.Count - 1; i >= 0; i--)
        {
            if (activeLightning[i] != null)
                Destroy(activeLightning[i]);
        }

        activeLightning.Clear();
    }

    private System.Collections.IEnumerator RemoveFromListAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
            activeLightning.Remove(obj);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Vector3 center = transform.position + new Vector3(0f, (minHeight + maxHeight) * 0.5f, 0f);
        Vector3 size = new Vector3(areaSize.x, maxHeight - minHeight, areaSize.z);

        Gizmos.DrawWireCube(center, size);
    }
}
