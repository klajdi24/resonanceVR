using UnityEngine;

public class FishSchoolController : MonoBehaviour
{
    [Header("Fish Prefab")]
    public GameObject fishPrefab;

    [Header("Spawn")]
    public int fishCount = 18;
    public float spawnRadius = 2.0f;
    public float waterY = -0.15f;

    [Header("Swim Motion")]
    public float swimSpeed = 0.7f;
    public float bobAmount = 0.03f;
    public float turnSpeed = 3.0f;

    [Header("Visibility (driven externally)")]
    [Range(0f, 1f)] public float visibility = 0f;

    private Transform[] fish;
    private Vector3[] centers;
    private float[] angles;
    private Renderer[] renderers;

    private void Start()
    {
        if (fishPrefab == null) return;

        fish = new Transform[fishCount];
        centers = new Vector3[fishCount];
        angles = new float[fishCount];
        renderers = new Renderer[fishCount];

        for (int i = 0; i < fishCount; i++)
        {
            Vector2 r = Random.insideUnitCircle * spawnRadius;
            Vector3 center = transform.position + new Vector3(r.x, waterY, r.y);

            centers[i] = center;
            angles[i] = Random.Range(0f, 360f);

            GameObject f = Instantiate(fishPrefab, center, Quaternion.identity, transform);
            fish[i] = f.transform;
            renderers[i] = f.GetComponentInChildren<Renderer>();

            float s = Random.Range(0.8f, 1.3f);
            fish[i].localScale *= s;

            // random tint (optional)
            if (renderers[i] != null && renderers[i].material.HasProperty("_Color"))
            {
                Color c = Color.Lerp(new Color(0.2f, 0.6f, 1f), new Color(0.9f, 0.4f, 1f), Random.value);
                renderers[i].material.color = c;
            }
        }
    }

    private void Update()
    {
        if (fish == null) return;

        // Fade fish in/out by toggling renderer
        bool show = visibility > 0.02f;
        for (int i = 0; i < fishCount; i++)
        {
            if (renderers[i] != null) renderers[i].enabled = show;
        }

        if (!show) return;

        float t = Time.time;

        for (int i = 0; i < fishCount; i++)
        {
            angles[i] += swimSpeed * (20f + i * 0.2f) * Time.deltaTime;

            float rad = angles[i] * Mathf.Deg2Rad;
            float r = spawnRadius * (0.35f + 0.65f * Mathf.PerlinNoise(i * 10.1f, t * 0.08f));

            Vector3 target = centers[i] + new Vector3(Mathf.Cos(rad) * r, 0f, Mathf.Sin(rad) * r);
            target.y = waterY + Mathf.Sin(t * (1.2f + i * 0.07f)) * bobAmount;

            // Move
            fish[i].position = Vector3.Lerp(fish[i].position, target, Time.deltaTime * 1.8f);

            // Look direction
            Vector3 dir = (target - fish[i].position);
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion look = Quaternion.LookRotation(dir.normalized, Vector3.up);
                fish[i].rotation = Quaternion.Slerp(fish[i].rotation, look, Time.deltaTime * turnSpeed);
            }
        }
    }
}