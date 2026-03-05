using UnityEngine;

public class ResolveZoneTrigger : MonoBehaviour
{
    public CalmnessEvents calmnessEvents;

    public float shrinkMultiplier = 0.4f;

    public bool disableGrabAfterResolve = true;
    public bool disablePhysicsAfterResolve = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("NoiseObject")) return;

        if (other.GetComponent<ResolvedMarker>() != null) return;

        other.gameObject.AddComponent<ResolvedMarker>();

        if (calmnessEvents != null)
            calmnessEvents.AddCalmness();

        // ✅ NEW: stop notification pings (works even if collider is a child)
        var pinger = other.GetComponentInParent<NotificationPinger>();
        if (pinger != null)
            pinger.StopPings();

        other.transform.localScale *= shrinkMultiplier;

        other.transform.position = transform.position + Vector3.up * 0.15f;

        if (disableGrabAfterResolve)
        {
            var grab =
                other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

            // If XRGrabInteractable is on the parent, disable that too:
            if (grab == null)
                grab = other.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

            if (grab) grab.enabled = false;
        }

        if (disablePhysicsAfterResolve)
        {
            var rb = other.GetComponent<Rigidbody>();

            // If Rigidbody is on the parent, disable that too:
            if (rb == null)
                rb = other.GetComponentInParent<Rigidbody>();

            if (rb) rb.isKinematic = true;
        }
    }

    private class ResolvedMarker : MonoBehaviour { }
}

