using UnityEngine;
using UnityEngine.UI;

public class NotificationPopper : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject notificationPrefab;   // PFB_NotificationIcon

    [Header("Spawn")]
    public Vector3 localOffset = new Vector3(0f, 0.35f, 0f);

    [Header("Timing")]
    public float intervalMin = 0.6f;
    public float intervalMax = 1.0f;

    [Header("Limit (prevents clutter/perf issues)")]
    public int maxActivePopups = 3;

    [Header("Optional icons (add later)")]
    public Sprite[] icons;

    private float nextTime;
    private bool active = true;

    private void Awake()
    {
        ScheduleNext();
    }

    private void Update()
    {
        if (!active) return;
        if (notificationPrefab == null) return;

        if (Time.time >= nextTime)
        {
            // Don’t exceed max active popups attached to this object
            int activeCount = GetComponentsInChildren<NotificationPopup>(true).Length;
            if (activeCount < maxActivePopups)
                SpawnOnce();

            ScheduleNext();
        }
    }

    private void ScheduleNext()
    {
        nextTime = Time.time + Random.Range(intervalMin, intervalMax);
    }

    private void SpawnOnce()
    {
        var go = Instantiate(notificationPrefab, transform);
        go.transform.localPosition = localOffset;
        go.transform.localRotation = Quaternion.identity;

        // Random icon if you add them later
        if (icons != null && icons.Length > 0)
        {
            var img = go.GetComponentInChildren<Image>();
            if (img != null)
                img.sprite = icons[Random.Range(0, icons.Length)];
        }
    }

    public void StopNotifications()
    {
        active = false;
    }
}
