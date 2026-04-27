using UnityEngine;

public class EnvironmentTransitionController : MonoBehaviour
{
    [Header("Reference")]
    public CalmnessController calmnessController;

    [Header("Floor Objects")]
    public Renderer concreteFloorRenderer;
    public GameObject waterFloorObject;

    [Header("Concrete Crack Glow (optional)")]
    [ColorUsage(true, true)] public Color crackGlowColor = new Color(0.2f, 0.9f, 1f, 1f);
    public float crackGlowIntensityMax = 2.5f;
    public string emissionPropertyURP = "_EmissionColor";
    public string emissionPropertyBuiltin = "_EmissionColor";

    [Header("Reveal Settings")]
    [Range(0f, 1f)] public float waterStartsAt = 0.25f;
    [Range(0f, 1f)] public float waterFullAt = 0.85f;
    public float waterRaiseAmount = 0.07f;

    [Header("Island (appears near calm)")]
    public Transform islandRoot;
    [Range(0f, 1f)] public float islandStartsAt = 0.55f;
    [Range(0f, 1f)] public float islandFullAt = 1.0f;

    [Header("Fireflies")]
    public ParticleSystem fireflies;
    public float firefliesRateAtOverload = 0f;
    public float firefliesRateAtCalm = 35f;

    [Header("Fish")]
    public FishSchoolController fishSchool;

    [Header("Smoothing")]
    public float smoothTime = 0.6f;

    private float blendVel;
    private float currentWaterBlend;
    private Vector3 waterBasePos;
    private Vector3 islandTargetScale;
    private Material concreteMat;
    private bool concreteIsURP;

    private void Awake()
    {
        if (calmnessController == null)
            calmnessController = FindFirstObjectByType<CalmnessController>();

        if (waterFloorObject != null)
            waterBasePos = waterFloorObject.transform.position;

        if (islandRoot != null)
            islandTargetScale = Vector3.one;

        if (concreteFloorRenderer != null)
        {
            concreteMat = concreteFloorRenderer.material; 
            concreteIsURP = concreteMat != null && concreteMat.HasProperty(emissionPropertyURP);
        }

        
        if (waterFloorObject != null)
            waterFloorObject.SetActive(false);

        if (islandRoot != null)
            islandRoot.localScale = Vector3.zero;

        SetFirefliesRate(0f);

        if (fishSchool != null)
            fishSchool.visibility = 0f;
    }

    private void Update()
    {
        if (calmnessController == null) return;

        float c = Mathf.Clamp01(calmnessController.calmness);

        
        float targetWater = Mathf.InverseLerp(waterStartsAt, waterFullAt, c);
        targetWater = Mathf.Clamp01(targetWater);

        currentWaterBlend = Mathf.SmoothDamp(currentWaterBlend, targetWater, ref blendVel, Mathf.Max(0.01f, smoothTime));

        ApplyConcreteCrackGlow(c);
        ApplyWaterReveal(currentWaterBlend);
        ApplyIsland(c);
        ApplyFireflies(c);
        ApplyFish(c);
    }

    private void ApplyConcreteCrackGlow(float calmness)
    {
        if (concreteMat == null) return;

        
        float glow01 = Mathf.SmoothStep(0f, 1f, calmness);

        float intensity = glow01 * crackGlowIntensityMax;
        Color emissive = crackGlowColor * intensity;

        string prop = emissionPropertyBuiltin;
        if (concreteIsURP) prop = emissionPropertyURP;

        if (concreteMat.HasProperty(prop))
        {
            concreteMat.EnableKeyword("_EMISSION");
            concreteMat.SetColor(prop, emissive);
        }
    }

    private void ApplyWaterReveal(float water01)
    {
        if (waterFloorObject == null) return;

        if (water01 > 0.02f && !waterFloorObject.activeSelf)
            waterFloorObject.SetActive(true);

        
        Vector3 pos = waterBasePos;
        pos.y = waterBasePos.y + (waterRaiseAmount * water01);
        waterFloorObject.transform.position = pos;

        
        
        if (concreteFloorRenderer != null)
        {
            
            concreteFloorRenderer.transform.localPosition = new Vector3(0f, 0f + (0.015f * (1f - water01)), 0f);
        }
    }

    private void ApplyIsland(float calmness)
    {
        if (islandRoot == null) return;

        float t = Mathf.InverseLerp(islandStartsAt, islandFullAt, calmness);
        t = Mathf.Clamp01(t);
        float eased = Mathf.SmoothStep(0f, 1f, t);

        islandRoot.localScale = Vector3.Lerp(Vector3.zero, islandTargetScale, eased);
    }

    private void ApplyFireflies(float calmness)
    {
        float t = Mathf.SmoothStep(0f, 1f, calmness);
        SetFirefliesRate(Mathf.Lerp(firefliesRateAtOverload, firefliesRateAtCalm, t));
    }

    private void SetFirefliesRate(float rate)
    {
        if (fireflies == null) return;
        var em = fireflies.emission;
        em.rateOverTime = rate;
    }

    private void ApplyFish(float calmness)
    {
        if (fishSchool == null) return;

        
        float fish01 = Mathf.InverseLerp(waterStartsAt + 0.1f, 1.0f, calmness);
        fish01 = Mathf.Clamp01(fish01);

        fishSchool.visibility = fish01;
    }
}
