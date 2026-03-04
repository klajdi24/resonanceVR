using UnityEngine;

public class NotificationPinger : MonoBehaviour
{
    [Header("UI Popup")]
    public GameObject popupPrefab;              // your PFB_NotificationIcon
    public Transform spawnPoint;                // optional child anchor
    public Vector3 localOffset = new Vector3(0f, 0.25f, 0f);

    [Header("Timing (feels like a phone going off)")]
    public float intervalMin = 0.6f;
    public float intervalMax = 1.0f;

    [Header("Limit (prevents spam/perf issues)")]
    public int maxActivePopups = 3;

    [Header("Sound")]
    public AudioSource sfxSource;
    public AudioClip pingClip;
    [Range(0f, 1f)] public float pingVolume = 0.7f;

    private float nextTime;
    private bool active = true;

    void Awake()
    {
        ScheduleNext();
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!active) return;
        if (popupPrefab == null) return;

        if (Time.time >= nextTime)
        {
            // Count popups currently parented to this object
            int activeCount = GetComponentsInChildren<NotificationPopup>(true).Length;
            if (activeCount < maxActivePopups)
            {
                SpawnPopup();
                PlayPing();
            }

            ScheduleNext();
        }
    }

    void ScheduleNext()
    {
        nextTime = Time.time + Random.Range(intervalMin, intervalMax);
    }

    void SpawnPopup()
    {
        Transform parent = transform;
        Vector3 pos = transform.position;

        if (spawnPoint != null)
            pos = spawnPoint.position;
        else
            pos = transform.TransformPoint(localOffset);

        // Spawn in world, then parent to object so it follows slightly if moved
        var go = Instantiate(popupPrefab, pos, Quaternion.identity);
        go.transform.SetParent(parent, true);
    }

    void PlayPing()
    {
        if (sfxSource != null && pingClip != null)
            sfxSource.PlayOneShot(pingClip, pingVolume);
    }

    public void StopPings()
    {
        active = false;
    }
}