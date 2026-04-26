using UnityEngine;

public class FullCalmnessSceneSwap : MonoBehaviour
{
    [Header("Calmness")]
    public CalmnessController calmnessController;
    [Range(0f, 1f)] public float triggerAt = 0.99f;

    [Header("Hide These At Full Calmness")]
    public GameObject[] objectsToHide;

    [Header("Show These At Full Calmness")]
    public GameObject[] objectsToShow;

    [Header("Optional")]
    public bool hideShownObjectsOnStart = true;

    private bool done = false;

    void Awake()
    {
        if (calmnessController == null)
            calmnessController = FindFirstObjectByType<CalmnessController>();

        if (hideShownObjectsOnStart)
        {
            foreach (GameObject obj in objectsToShow)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (done || calmnessController == null) return;

        if (calmnessController.calmness >= triggerAt)
        {
            done = true;
            DoSwap();
        }
    }

    private void DoSwap()
    {
        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        foreach (GameObject obj in objectsToShow)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }
}
