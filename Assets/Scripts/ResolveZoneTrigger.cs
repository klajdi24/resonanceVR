using UnityEngine;


public class ResolveZoneTrigger : MonoBehaviour
{
    public CalmnessEvents calmnessEvents;

    [Header("Orb feedback")]
    public float shrinkMultiplier = 0.4f;
    public bool disableGrabAfterResolve = true;
    public bool disablePhysicsAfterResolve = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("NoiseObject")) return;

        // Prevent double counting
        if (other.GetComponent<ResolvedMarker>() != null) return;
        other.gameObject.AddComponent<ResolvedMarker>();

        // Increase calmness
        if (calmnessEvents != null) calmnessEvents.AddCalmness();

        // Visual feedback: shrink + reposition slightly
        other.transform.localScale *= shrinkMultiplier;
        other.transform.position = transform.position + Vector3.up * 0.15f;

        // Stop further interaction
        if (disableGrabAfterResolve)
        {
            var grab = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grab) grab.enabled = false;
        }

        if (disablePhysicsAfterResolve)
        {
            var rb = other.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true;
        }
    }

    // Small marker component so we don’t resolve the same orb twice
    private class ResolvedMarker : MonoBehaviour { }
}

