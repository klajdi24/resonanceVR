using UnityEngine;

public class NotificationPinger : MonoBehaviour
{
    [Header("UI Popup")]
    public GameObject popupPrefab;

    [Tooltip("Optional. If assigned, used as a starting reference position; popup will still correct itself using bounds.")]
    public Transform spawnPoint;

    [Tooltip("Used only if spawnPoint is null. Local offset from this object.")]
    public Vector3 localOffset = new Vector3(0f, 0.25f, 0f);

    [Header("Timing")]
    public float intervalMin = 0.6f;
    public float intervalMax = 1.0f;

    [Header("Limit")]
    public int maxActivePopups = 3;

    [Header("Spawn Parent (NotificationRoot)")]
    public Transform popupRoot;

    [Header("Optional ground clamp (recommended)")]
    public string floorLayerName = "Floor"; 

    [Header("Sound")]
    public AudioSource sfxSource;
    public AudioClip pingClip;
    [Range(0f, 1f)] public float pingVolume = 0.7f;

    private float nextTime;
    private bool active = true;
    private Transform myPopupContainer;

    private Renderer cachedRenderer;
    private Collider cachedCollider;
    private LayerMask floorMask;

    private void Awake()
    {
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();

        cachedRenderer = GetComponentInChildren<Renderer>();
        cachedCollider = GetComponentInChildren<Collider>();

        
        floorMask = LayerMask.GetMask(floorLayerName);

        
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

    private void Update()
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

    private void ScheduleNext()
    {
        nextTime = Time.time + Random.Range(intervalMin, intervalMax);
    }

    private void SpawnPopup()
    {
        
        Vector3 pos = (spawnPoint != null)
            ? spawnPoint.position
            : transform.TransformPoint(localOffset);

        GameObject go = (myPopupContainer != null)
            ? Instantiate(popupPrefab, myPopupContainer)
            : Instantiate(popupPrefab);

        go.transform.position = pos;
        go.transform.rotation = Quaternion.identity;
        go.SetActive(true);

        
        var popup = go.GetComponent<NotificationPopup>();
        if (popup != null)
        {
            popup.followTarget = transform;
            popup.targetRenderer = cachedRenderer;
            popup.targetCollider = cachedCollider;

            
            if (floorMask.value != 0)
                popup.groundMask = floorMask;
        }
    }

    private void PlayPing()
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