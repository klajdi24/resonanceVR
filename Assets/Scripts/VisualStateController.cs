using UnityEngine;

public class VisualStateController : MonoBehaviour
{
    [Header("Reference")]
    public CalmnessController calmnessController;

    [Header("Particles")]
    public ParticleSystem noiseParticles;
    public Gradient particleColorOverCalmness;

    [Header("Lighting")]
    public Light mainLight;
    public Color lightColorOverload = new Color(0.9f, 0.95f, 1f);
    public Color lightColorCalm = new Color(0.95f, 0.9f, 0.8f);
    public float lightIntensityOverload = 1.2f;
    public float lightIntensityCalm = 0.7f;

    [Header("Fog")]
    public bool controlFog = true;
    public Color fogColorOverload = new Color(0.7f, 0.8f, 0.9f);
    public Color fogColorCalm = new Color(0.85f, 0.9f, 0.85f);
    public float fogDensityOverload = 0.02f;
    public float fogDensityCalm = 0.005f;

    [Header("Accent Materials (optional)")]
    public Renderer[] accentRenderers;
    [ColorUsage(true, true)] public Color emissiveOverload = Color.white;
    [ColorUsage(true, true)] public Color emissiveCalm = Color.white;
    public string emissiveProperty = "_EmissionColor";

    void Reset()
    {
        // Useful defaults for the gradient if you forget to set it
        particleColorOverCalmness = new Gradient();
        particleColorOverCalmness.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1f, 0.3f, 0.3f), 0f),   // red-ish overload
                new GradientColorKey(new Color(0.2f, 0.9f, 0.8f), 1f)  // teal calm
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );
    }

    void Update()
    {
        if (calmnessController == null) return;
        float c = Mathf.Clamp01(calmnessController.calmness);

        // 1) Particles color
        if (noiseParticles != null)
        {
            var main = noiseParticles.main;
            Color col = particleColorOverCalmness.Evaluate(c);
            main.startColor = col;
        }

        // 2) Lighting
        if (mainLight != null)
        {
            mainLight.color = Color.Lerp(lightColorOverload, lightColorCalm, c);
            mainLight.intensity = Mathf.Lerp(lightIntensityOverload, lightIntensityCalm, c);
        }

        // 3) Fog
        if (controlFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = Color.Lerp(fogColorOverload, fogColorCalm, c);
            RenderSettings.fogDensity = Mathf.Lerp(fogDensityOverload, fogDensityCalm, c);
        }

        // 4) Emissive accents
        if (accentRenderers != null)
        {
            Color e = Color.Lerp(emissiveOverload, emissiveCalm, c);
            for (int i = 0; i < accentRenderers.Length; i++)
            {
                var r = accentRenderers[i];
                if (r == null) continue;
                foreach (var mat in r.materials)
                {
                    if (mat.HasProperty(emissiveProperty))
                    {
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor(emissiveProperty, e);
                    }
                }
            }
        }
    }
}

