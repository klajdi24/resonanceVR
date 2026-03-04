using UnityEngine;
using UnityEngine.UI;

public class NotificationPopup : MonoBehaviour
{
    public float lifetime = 1.2f;
    public float floatUp = 0.18f;

    [Tooltip("How long it stays fully visible before fading")]
    public float holdFraction = 0.7f;

    private CanvasGroup group;
    private Vector3 startLocalPos;
    private float t;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        if (group == null) group = gameObject.AddComponent<CanvasGroup>();
        startLocalPos = transform.localPosition;
        group.alpha = 1f;
    }

    void Update()
    {
        t += Time.deltaTime;
        float k = Mathf.Clamp01(t / Mathf.Max(0.01f, lifetime));

        // Float up
        transform.localPosition = startLocalPos + Vector3.up * (floatUp * k);

        // Hold then fade near end
        float fadeStart = holdFraction;
        float fadeT = Mathf.InverseLerp(fadeStart, 1f, k);
        group.alpha = 1f - Mathf.Clamp01(fadeT);

        if (k >= 1f) Destroy(gameObject);
    }
}
