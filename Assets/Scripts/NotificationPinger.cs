using UnityEngine;

public class NotificationPinger : MonoBehaviour
{
    [Header("UI Popup")]
    public GameObject popupPrefab;

    [Tooltip("Best: assign PingPoint (child of the device) so placement is perfect.")]
    public Transform spawnPoint;

    public Vector3 localOffset = new Vector3(0f, 0.25f, 0f);

    [Header("Timing")]
    public float intervalMin = 0.6f;
    public float intervalMax = 1.0f;

    [Header("Limit")]
    public int maxActivePopups = 3;

    [Header("Spawn Parent (NotificationRoot)")]
    public Transform popupRoot;

    [Header("Sound")]
    public AudioSource sfxSource;
    public AudioClip pingClip;
    [Range(0f, 1f)] public float pingVolume = 0.7f;

    float nextTime;
    bool active = true;
    Transform myPopupContainer;

    void Awake()
    {
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();

        // Create a private container under NotificationRoot (scale-safe + easy to clear)
        if (popupRoot != null)
        {
            var go = new GameObject($"{name}_Popups");
            myPopupContainer = go.transform;
            myPopupContainer.SetParent(popupRoot, false);
            myPopupContainer.localPosition = Vector3.zero;
            myPopupContainer.localRotation = Quaternion.identity;
            myPopupContainer.localScale = Vector3.one;
        }

        ScheduleNext();
    }

    void Update()
    {
        if (!active) return;
        if (popupPrefab == null) return;

        if (Time.time >= nextTime)
        {
            int activeCount = (myPopupContainer != null)
                ? myPopupContainer.GetComponentsInChildren<NotificationPopup>(true).Length
                : FindObjectsByType<NotificationPopup>(FindObjectsSortMode.None).Length;

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
        // Spawn position
        Vector3 pos = (spawnPoint != null)
            ? spawnPoint.position
            : transform.TransformPoint(localOffset);

        // Spawn under container (keeps hierarchy clean + avoids device scale)
        GameObject go = (myPopupContainer != null)
            ? Instantiate(popupPrefab, myPopupContainer)
            : Instantiate(popupPrefab);

        go.transform.position = pos;
        go.transform.rotation = Quaternion.identity;
        go.SetActive(true);

        // NEW: make the popup follow the device while it exists
        var popup = go.GetComponent<NotificationPopup>();
        if (popup != null)
        {
            popup.followTarget = (spawnPoint != null) ? spawnPoint : transform;
            popup.followOffset = Vector3.zero;
        }
    }

    void PlayPing()
    {
        if (sfxSource != null && pingClip != null)
            sfxSource.PlayOneShot(pingClip, pingVolume);
    }

    public void StopPings(bool clearExisting = true)
    {
        active = false;

        if (clearExisting && myPopupContainer != null)
        {
            for (int i = myPopupContainer.childCount - 1; i >= 0; i--)
                Destroy(myPopupContainer.GetChild(i).gameObject);
        }
    }
}