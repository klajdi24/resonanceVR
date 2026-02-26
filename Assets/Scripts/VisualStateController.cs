using UnityEngine;

public class VisualStateController : MonoBehaviour
{
    public CalmnessController calmnessController;

    public ParticleSystem noiseParticles;

    public Gradient particleColorOverCalmness;

    public Light mainLight;

    public Color lightColorOverload =
    new Color(0.9f, 0.95f, 1f);

    public Color lightColorCalm =
    new Color(0.95f, 0.9f, 0.8f);

    public float lightIntensityOverload = 1.2f;
    public float lightIntensityCalm = 0.7f;

    public bool controlFog = true;

    public Color fogColorOverload =
    new Color(0.7f, 0.8f, 0.9f);

    public Color fogColorCalm =
    new Color(0.85f, 0.9f, 0.85f);

    public float fogDensityOverload = 0.02f;
    public float fogDensityCalm = 0.005f;

    public Renderer[] accentRenderers;

    public Color emissiveOverload = Color.white;
    public Color emissiveCalm = Color.white;

    public string emissiveProperty = "_EmissionColor";

    void Update()
    {
        if (calmnessController == null) return;

        float c =
        Mathf.Clamp01(
        calmnessController.calmness);

        if (noiseParticles != null)
        {
            var main = noiseParticles.main;

            Color col =
            particleColorOverCalmness.Evaluate(c);

            main.startColor = col;
        }

        if (mainLight != null)
        {
            mainLight.color =
            Color.Lerp(
            lightColorOverload,
            lightColorCalm,
            c);

            mainLight.intensity =
            Mathf.Lerp(
            lightIntensityOverload,
            lightIntensityCalm,
            c);
        }

        if (controlFog)
        {
            RenderSettings.fog = true;

            RenderSettings.fogColor =
            Color.Lerp(
            fogColorOverload,
            fogColorCalm,
            c);

            RenderSettings.fogDensity =
            Mathf.Lerp(
            fogDensityOverload,
            fogDensityCalm,
            c);
        }
    }
}

