using UnityEngine;

public class ButterflySwarmController : MonoBehaviour
{
    [Header("References")]
    public CalmnessController calmnessController;
    public GameObject butterflyPrefab;

    [Header("When to appear")]
    [Range(0f, 1f)] public float startAtCalmness = 0.85f; 
    [Range(0f, 1f)] public float fullAtCalmness = 1.0f;   

    [Header("Swarm")]
    public int count = 10;
    public float radius = 1.6f;
    public float heightMin = 1.0f;
    public float heightMax = 2.0f;

    [Header("Motion")]
    public float orbitSpeed = 0.7f;     
    public float turnSpeed = 6.0f;      
    public float bobAmount = 0.12f;     
    public float driftAmount = 0.35f;   

    [Header("Performance / Behaviour")]
    public bool spawnOnce = true;
    public bool keepSwarmNearThisObject = true; 

    private Transform[] butterflies;
    private Vector3[] baseScales;
    private float[] angles;
    private float[] phase;
    private bool spawned;

    private void Awake()
    {
        if (calmnessController == null)
            calmnessController = FindFirstObjectByType<CalmnessController>();
    }

    private void Update()
    {
        if (calmnessController == null || butterflyPrefab == null) return;

        float c = Mathf.Clamp01(calmnessController.calmness);
        float vis = Mathf.InverseLerp(startAtCalmness, fullAtCalmness, c);
        vis = Mathf.Clamp01(vis);

        
        if (!spawned && (c >= startAtCalmness))
        {
            Spawn();
            spawned = true;
        }

        if (!spawned) return;

        
        

        
        for (int i = 0; i < butterflies.Length; i++)
        {
            if (butterflies[i] == null) continue;

            
            bool show = vis > 0.02f;
            if (butterflies[i].gameObject.activeSelf != show)
                butterflies[i].gameObject.SetActive(show);

            if (!show) continue;

            
            butterflies[i].localScale = baseScales[i] * Mathf.Lerp(0.2f, 1f, vis);

            Fly(i, Time.time);
        }
    }

    private void Spawn()
    {
        butterflies = new Transform[count];
        baseScales = new Vector3[count];
        angles = new float[count];
        phase = new float[count];

        for (int i = 0; i < count; i++)
        {
            float a = Random.Range(0f, 360f);
            angles[i] = a;
            phase[i] = Random.Range(0f, 1000f);

            Vector3 pos = GetTargetPosition(i, Time.time);
            GameObject b = Instantiate(butterflyPrefab, pos, Quaternion.identity, keepSwarmNearThisObject ? transform : null);

            butterflies[i] = b.transform;

            
            float s = Random.Range(0.85f, 1.25f);
            baseScales[i] = butterflies[i].localScale * s;

            
            b.SetActive(false);
        }
    }

    private void Fly(int i, float t)
    {
        Vector3 target = GetTargetPosition(i, t);

        
        Transform b = butterflies[i];
        b.position = Vector3.Lerp(b.position, target, Time.deltaTime * 2.0f);

        
        Vector3 dir = (target - b.position);
        if (dir.sqrMagnitude > 0.00001f)
        {
            Quaternion look = Quaternion.LookRotation(dir.normalized, Vector3.up);
            b.rotation = Quaternion.Slerp(b.rotation, look, Time.deltaTime * turnSpeed);
        }
    }

    private Vector3 GetTargetPosition(int i, float t)
    {
        Vector3 center = transform.position;

        
        angles[i] += orbitSpeed * 35f * Time.deltaTime;

        float rad = angles[i] * Mathf.Deg2Rad;

        
        float driftX = (Mathf.PerlinNoise(phase[i], t * 0.25f) - 0.5f) * 2f * driftAmount;
        float driftZ = (Mathf.PerlinNoise(phase[i] + 33.3f, t * 0.25f) - 0.5f) * 2f * driftAmount;

        
        float baseY = Mathf.Lerp(heightMin, heightMax, Mathf.PerlinNoise(phase[i] + 77.7f, t * 0.12f));
        float bob = Mathf.Sin(t * (1.2f + i * 0.07f)) * bobAmount;

        Vector3 orbit = new Vector3(Mathf.Cos(rad) * radius + driftX, baseY + bob, Mathf.Sin(rad) * radius + driftZ);

        return center + orbit;
    }
}
