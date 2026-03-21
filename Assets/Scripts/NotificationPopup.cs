using UnityEngine;
using UnityEngine.UI;

public class NotificationPopup : MonoBehaviour
{
    [Header("Life")]
    public float lifetime = 1.2f;
    public float floatUp = 0.12f;
    [Range(0f, 1f)] public float holdFraction = 0.8f;

    [Header("Follow (set by spawner)")]
    public Transform followTarget;         // PingPoint
    public Vector3 followOffset = Vector3.zero;

    private CanvasGroup group;
    private float t;
    private Vector3 spawnWorldPos;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        if (group == null) group = gameObject.AddComponent<CanvasGroup>();
        group.alpha = 1f;

        spawnWorldPos = transform.position;
    }

    void Update()
    {
        t += Time.deltaTime;
        float k = Mathf.Clamp01(t / Mathf.Max(0.01f, lifetime));

        // Base position follows target if assigned, otherwise stays where spawned
        Vector3 basePos = (followTarget != null)
            ? followTarget.position + followOffset
            : spawnWorldPos;

        // Float up relative to base position
        transform.position = basePos + Vector3.up * (floatUp * k);

        // Hold then fade near end
        float fadeT = Mathf.InverseLerp(holdFraction, 1f, k);
        group.alpha = 1f - Mathf.Clamp01(fadeT);

        if (k >= 1f)
            Destroy(gameObject);
    }
}
