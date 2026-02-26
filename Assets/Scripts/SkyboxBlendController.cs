using UnityEngine;

public class SkyboxBlendController : MonoBehaviour
{
    [Header("Reference")]
    public CalmnessController calmnessController;

    [Header("Skybox Material (Custom/SkyboxBlend)")]
    public Material blendedSkyboxMaterial;

    [Header("How calmness maps to sky blend")]
    [Range(0f, 1f)] public float startBlendAtCalmness = 0.15f;
    [Range(0f, 1f)] public float fullBlendAtCalmness = 0.85f;

    [Header("Smoothing")]
    public float smoothTime = 0.35f;

    [Header("Galaxy Animation")]
    [Range(-5f, 5f)] public float galaxyRotationSpeed = 0.25f;
    [Range(0f, 1f)] public float twinkleStrength = 0.25f;
    [Range(0f, 10f)] public float twinkleSpeed = 1.5f;

    private float blendVelocity;
    private float currentBlend;

    private static readonly int BlendID = Shader.PropertyToID("_Blend");
    private static readonly int GalaxyRotSpeedID = Shader.PropertyToID("_GalaxyRotationSpeed");
    private static readonly int TwinkleStrengthID = Shader.PropertyToID("_TwinkleStrength");
    private static readonly int TwinkleSpeedID = Shader.PropertyToID("_TwinkleSpeed");

    private void Awake()
    {
        if (calmnessController == null)
            calmnessController = FindFirstObjectByType<CalmnessController>();

        if (blendedSkyboxMaterial == null)
            blendedSkyboxMaterial = RenderSettings.skybox;
    }

    private void Start()
    {
        Apply(0f);
    }

    private void Update()
    {
        if (calmnessController == null || blendedSkyboxMaterial == null) return;

        float c = Mathf.Clamp01(calmnessController.calmness);

        float targetBlend = Mathf.InverseLerp(startBlendAtCalmness, fullBlendAtCalmness, c);
        targetBlend = Mathf.Clamp01(targetBlend);

        currentBlend = Mathf.SmoothDamp(currentBlend, targetBlend, ref blendVelocity, Mathf.Max(0.01f, smoothTime));

        Apply(currentBlend);
    }

    private void Apply(float blend01)
    {
        blendedSkyboxMaterial.SetFloat(BlendID, blend01);

        // only animate galaxy when it becomes visible
        float motion = Mathf.SmoothStep(0f, 1f, blend01);
        blendedSkyboxMaterial.SetFloat(GalaxyRotSpeedID, galaxyRotationSpeed * motion);
        blendedSkyboxMaterial.SetFloat(TwinkleStrengthID, twinkleStrength * motion);
        blendedSkyboxMaterial.SetFloat(TwinkleSpeedID, twinkleSpeed);

        DynamicGI.UpdateEnvironment();
    }
}
