using UnityEngine;

public class NotificationPopup : MonoBehaviour
{
    [Header("Life")]
    public float lifetime = 1.2f;
    public float floatUp = 0.10f;

    [Header("Follow Target (set by spawner)")]
    public Transform followTarget;          // phone/laptop root
    public Renderer targetRenderer;         // optional, helps bounds
    public Collider targetCollider;         // optional, helps bounds

    [Header("Placement")]
    public float upOffset = 0.18f;          // how high above object
    public float towardCameraOffset = 0.10f; // pushes icon toward player
    public float minAboveGround = 0.05f;    // never go below this above ground

    [Header("Ground")]
    public LayerMask groundMask;            // set to your Floor layer (optional)

    private float t;
    private Vector3 spawnWorldPos;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        spawnWorldPos = transform.position;
    }

    private void Update()
    {
        t += Time.deltaTime;
        float k = Mathf.Clamp01(t / Mathf.Max(0.01f, lifetime));

        // base position (follows target if set)
        Vector3 basePos = (followTarget != null) ? ComputeAnchorPosition() : spawnWorldPos;

        // float up a bit over time
        transform.position = basePos + Vector3.up * (floatUp * k);

        // always face the player (billboard)
        if (cam != null)
        {
            Vector3 toCam = (cam.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(toCam, Vector3.up);
        }

        if (k >= 1f)
            Destroy(gameObject);
    }

    private Vector3 ComputeAnchorPosition()
    {
        // Get bounds from renderer/collider (most accurate)
        Bounds b;
        bool hasBounds = false;

        if (targetRenderer != null)
        {
            b = targetRenderer.bounds;
            hasBounds = true;
        }
        else if (targetCollider != null)
        {
            b = targetCollider.bounds;
            hasBounds = true;
        }
        else
        {
            // fallback if none provided
            b = new Bounds(followTarget.position, Vector3.one * 0.25f);
        }

        // Top of the object in world space (works even upside down)
        Vector3 top = b.center + Vector3.up * b.extents.y;

        // Push slightly toward the camera so it’s readable
        Vector3 camDir = Vector3.zero;
        if (cam != null)
        {
            camDir = (cam.transform.position - top);
            camDir.y = 0f; // keep it horizontal (prevents pushing down into floor)
            if (camDir.sqrMagnitude > 0.0001f) camDir.Normalize();
        }

        Vector3 desired = top + Vector3.up * upOffset + camDir * towardCameraOffset;

        // Clamp above ground (optional but good)
        // If you don’t want to set groundMask, just rely on min Y below.
        if (groundMask.value != 0)
        {
            // ray from above down to find ground
            if (Physics.Raycast(desired + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f, groundMask))
            {
                desired.y = Mathf.Max(desired.y, hit.point.y + minAboveGround);
            }
        }

        return desired;
    }
}