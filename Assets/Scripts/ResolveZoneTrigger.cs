using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ResolveZoneTrigger : MonoBehaviour
{
    public CalmnessEvents calmnessEvents;

    public float shrinkMultiplier = 0.4f;

    public bool disableGrabAfterResolve = true;
    public bool disablePhysicsAfterResolve = true;

    [Header("Open Path After First Resolve")]
    public GameObject[] objectsToHideAfterFirstResolve;
    private bool hasOpenedPath = false;

    private void OnTriggerEnter(Collider other)
    {
        TryResolve(other);
    }

    private void TryResolve(Collider other)
    {
        Transform noiseObject = FindNoiseObject(other.transform);
        if (noiseObject == null) return;

        Transform root = GetResolveRoot(noiseObject);

        if (root.GetComponent<ResolvedMarker>() != null) return;

        root.gameObject.AddComponent<ResolvedMarker>();

        if (calmnessEvents != null)
            calmnessEvents.AddCalmness();

        
        OpenPathAfterFirstResolve();

        
        StopNotificationPingers(root);

        
        StopProximitySounds(root);

        
        PlayResolveEffects(root);

        
        root.localScale *= shrinkMultiplier;
        root.position = transform.position + Vector3.up * 0.15f;

        if (disableGrabAfterResolve)
        {
            XRGrabInteractable grab = root.GetComponent<XRGrabInteractable>();

            if (grab == null)
                grab = root.GetComponentInChildren<XRGrabInteractable>();

            if (grab != null)
                grab.enabled = false;
        }

        if (disablePhysicsAfterResolve)
        {
            Rigidbody rb = root.GetComponent<Rigidbody>();

            if (rb == null)
                rb = root.GetComponentInChildren<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }

    private void OpenPathAfterFirstResolve()
    {
        if (hasOpenedPath) return;

        hasOpenedPath = true;

        foreach (GameObject obj in objectsToHideAfterFirstResolve)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    private Transform FindNoiseObject(Transform start)
    {
        Transform current = start;

        while (current != null)
        {
            if (current.CompareTag("NoiseObject"))
                return current;

            current = current.parent;
        }

        return null;
    }

    private Transform GetResolveRoot(Transform noiseObject)
    {
        XRGrabInteractable grab = noiseObject.GetComponentInParent<XRGrabInteractable>();
        if (grab != null)
            return grab.transform;

        Rigidbody rb = noiseObject.GetComponentInParent<Rigidbody>();
        if (rb != null)
            return rb.transform;

        NotificationPinger pinger = noiseObject.GetComponentInParent<NotificationPinger>();
        if (pinger != null)
            return pinger.transform;

        ProximitySoundAnchor sound = noiseObject.GetComponentInParent<ProximitySoundAnchor>();
        if (sound != null)
            return sound.transform;

        return noiseObject;
    }

    private void StopNotificationPingers(Transform root)
    {
        NotificationPinger[] pingers = root.GetComponentsInChildren<NotificationPinger>(true);

        foreach (NotificationPinger pinger in pingers)
        {
            if (pinger == null) continue;

            pinger.StopPings(true);
            pinger.enabled = false;
        }
    }

    private void StopProximitySounds(Transform root)
    {
        ProximitySoundAnchor[] sounds = root.GetComponentsInChildren<ProximitySoundAnchor>(true);

        foreach (ProximitySoundAnchor sound in sounds)
        {
            if (sound == null) continue;

            sound.enabled = false;
        }
    }

    private void PlayResolveEffects(Transform root)
    {
        ResolveEffectOnPlaced[] effects =
            root.GetComponentsInChildren<ResolveEffectOnPlaced>(true);

        foreach (ResolveEffectOnPlaced effect in effects)
        {
            if (effect != null)
                effect.PlayResolveEffect();
        }
    }

    private class ResolvedMarker : MonoBehaviour { }
}

