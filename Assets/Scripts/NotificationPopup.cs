using UnityEngine;

public class NotificationPopup : MonoBehaviour
{
    [Header("Life")]
    public float lifetime = 1.2f;
    public float floatUp = 0.10f;

    [Header("Follow Target (set by spawner)")]
    public Transform followTarget;          
    public Renderer targetRenderer;        
    public Collider targetCollider;         

    [Header("Placement")]
    public float upOffset = 0.18f;          
    public float towardCameraOffset = 0.10f; 
    public float minAboveGround = 0.05f;    

    [Header("Ground")]
    public LayerMask groundMask;           

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

        
        Vector3 basePos = (followTarget != null) ? ComputeAnchorPosition() : spawnWorldPos;

        
        transform.position = basePos + Vector3.up * (floatUp * k);

        
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
            
            b = new Bounds(followTarget.position, Vector3.one * 0.25f);
        }

        
        Vector3 top = b.center + Vector3.up * b.extents.y;

        
        Vector3 camDir = Vector3.zero;
        if (cam != null)
        {
            camDir = (cam.transform.position - top);
            camDir.y = 0f; 
            if (camDir.sqrMagnitude > 0.0001f) camDir.Normalize();
        }

        Vector3 desired = top + Vector3.up * upOffset + camDir * towardCameraOffset;

        
        
        if (groundMask.value != 0)
        {
            
            if (Physics.Raycast(desired + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f, groundMask))
            {
                desired.y = Mathf.Max(desired.y, hit.point.y + minAboveGround);
            }
        }

        return desired;
    }
}