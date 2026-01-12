using UnityEngine;

public class CalmnessEvents : MonoBehaviour
{
    [Header("Reference")]
    public CalmnessController calmnessController;

    [Header("Tuning")]
    [Range(0f, 1f)] public float calmnessGainPerResolve = 0.33f; // 3 resolves -> calm

    public void AddCalmness(float amount = -1f)
    {
        if (calmnessController == null) return;

        float add = (amount < 0f) ? calmnessGainPerResolve : amount;
        calmnessController.calmness = Mathf.Clamp01(calmnessController.calmness + add);
    }
}
