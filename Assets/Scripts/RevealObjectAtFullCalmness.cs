using UnityEngine;

public class RevealObjectAtFullCalmness : MonoBehaviour
{
    public CalmnessController calmnessController;
    public GameObject targetToEnable;
    [Range(0f, 1f)] public float revealAt = 1.0f;

    bool done;

    void Awake()
    {
        if (calmnessController == null)
            calmnessController = FindFirstObjectByType<CalmnessController>();
    }

    void Update()
    {
        if (done || calmnessController == null || targetToEnable == null) return;

        if (calmnessController.calmness >= revealAt - 0.0001f)
        {
            targetToEnable.SetActive(true);
            done = true;
        }
    }
}
