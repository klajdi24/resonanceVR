using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class CalmnessController : MonoBehaviour
{
    [Header("Calmness (0 = overload, 1 = calm)")]
    [Range(0f, 1f)] public float calmness = 0f;

    [Header("Optional manual rise (ONLY if you still want a hold mechanic)")]
    public bool allowHoldToIncrease = false;
    public float riseSpeed = 0.40f;

    [Header("Editor Test (Space key)")]
    public bool allowKeyboardTestInEditor = true;

#if ENABLE_INPUT_SYSTEM
    [Header("XR Input (optional)")]
    [Tooltip("Optional: assign an InputActionReference bound to Right Trigger (Value/Axis).")]
    public InputActionReference holdAction;
#endif

    [Header("Audio Layers")]
    public AudioSource overloadLoop;
    public AudioSource calmLoop;
    public AudioSource breathLoop;

    [Range(0f, 1f)] public float overloadMax = 0.85f;
    [Range(0f, 1f)] public float calmMax = 0.85f;
    [Range(0f, 1f)] public float breathMax = 0.60f;
    [Range(0f, 1f)] public float breathMin = 0.25f;

    [Header("Visual Clutter (Particles)")]
    public ParticleSystem noiseParticles;
    public float particlesAtOverload = 80f;
    public float particlesAtCalm = 5f;

    private bool IsHeld()
    {
#if ENABLE_INPUT_SYSTEM
        
        if (allowKeyboardTestInEditor && Application.isEditor)
        {
            var kb = Keyboard.current;
            return kb != null && kb.spaceKey.isPressed;
        }

        
        if (holdAction != null && holdAction.action != null)
            return holdAction.action.ReadValue<float>() > 0.2f;
#endif
        return false;
    }

#if ENABLE_INPUT_SYSTEM
    private void OnEnable()
    {
        if (holdAction != null && holdAction.action != null)
            holdAction.action.Enable();
    }

    private void OnDisable()
    {
        if (holdAction != null && holdAction.action != null)
            holdAction.action.Disable();
    }
#endif

    private void Update()
    {
        
         if (allowHoldToIncrease && IsHeld())
        {
            calmness += riseSpeed * Time.deltaTime;
            calmness = Mathf.Clamp01(calmness);
        }

        
        if (overloadLoop) overloadLoop.volume = Mathf.Lerp(overloadMax, 0f, calmness);
        if (calmLoop)     calmLoop.volume     = Mathf.Lerp(0f, calmMax, calmness);

        
        if (breathLoop)   breathLoop.volume   = Mathf.Lerp(breathMax, breathMin, calmness);

        
        if (noiseParticles)
        {
            var emission = noiseParticles.emission;
            emission.rateOverTime = Mathf.Lerp(particlesAtOverload, particlesAtCalm, calmness);
        }
    }

    
    public void AddCalmness(float amount)
    {
        calmness = Mathf.Clamp01(calmness + amount);
    }
}


