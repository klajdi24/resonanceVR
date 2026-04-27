using UnityEngine;

public class PhoenixOrbitAtFullCalmness : MonoBehaviour
{
    [Header("Calmness")]
    public CalmnessController calmnessController;
    [Range(0f, 1f)] public float startAtCalmness = 0.99f;

    [Header("Orbit Target")]
    public Transform treeTarget;

    [Header("Orbit Settings")]
    public float radius = 3f;
    public float height = 2.5f;
    public float orbitSpeed = 35f;
    public float bobAmount = 0.35f;
    public float bobSpeed = 1.5f;
    public float moveSmoothness = 4f;
    public float turnSpeed = 5f;

    [Header("Animation")]
    public Animator phoenixAnimator;
    public string flyingAnimationStateName = "";
    public bool disableAnimatorUntilFullCalmness = true;

    [Header("Visibility")]
    public bool hideRenderersUntilFullCalmness = true;

    private Renderer[] renderers;
    private bool startedFlying;
    private float angle;

    private void Awake()
    {
        if (calmnessController == null)
            calmnessController = FindFirstObjectByType<CalmnessController>();

        if (phoenixAnimator == null)
            phoenixAnimator = GetComponentInChildren<Animator>(true);

        renderers = GetComponentsInChildren<Renderer>(true);

        if (hideRenderersUntilFullCalmness)
            SetRenderers(false);

        if (disableAnimatorUntilFullCalmness && phoenixAnimator != null)
            phoenixAnimator.enabled = false;

        angle = Random.Range(0f, 360f);
    }

    private void Update()
    {
        if (calmnessController == null || treeTarget == null) return;

        if (!startedFlying)
        {
            if (calmnessController.calmness >= startAtCalmness)
            {
                StartFlying();
            }
            else
            {
                return;
            }
        }

        FlyAroundTree();
    }

    private void StartFlying()
    {
        startedFlying = true;

        SetRenderers(true);

        if (phoenixAnimator != null)
        {
            phoenixAnimator.enabled = true;

            if (!string.IsNullOrEmpty(flyingAnimationStateName))
                phoenixAnimator.Play(flyingAnimationStateName, 0, 0f);
        }

        
        transform.position = GetOrbitPosition();
    }

    private void FlyAroundTree()
    {
        angle += orbitSpeed * Time.deltaTime;

        Vector3 targetPos = GetOrbitPosition();

        Vector3 oldPos = transform.position;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSmoothness);

        Vector3 moveDir = transform.position - oldPos;

        if (moveDir.sqrMagnitude > 0.00001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
        }
    }

    private Vector3 GetOrbitPosition()
    {
        float rad = angle * Mathf.Deg2Rad;

        Vector3 center = treeTarget.position;

        float x = Mathf.Cos(rad) * radius;
        float z = Mathf.Sin(rad) * radius;
        float y = height + Mathf.Sin(Time.time * bobSpeed) * bobAmount;

        return center + new Vector3(x, y, z);
    }

    private void SetRenderers(bool visible)
    {
        if (renderers == null) return;

        foreach (Renderer r in renderers)
        {
            if (r != null)
                r.enabled = visible;
        }
    }
}
