using UnityEngine;

public class RevealObjectAtFullCalmness : MonoBehaviour
{
    [Header("Calmness")]
    public CalmnessController calmnessController;

    [Range(0f, 1f)]
    public float revealAt = 0.99f;

    [Header("Object Reveal")]
    public GameObject targetToEnable;

    [Tooltip("Turn this on if the object should be hidden until full calmness.")]
    public bool hideTargetOnStart = false;

    [Header("Animator Animation")]
    public Animator animatorToPlay;

    [Tooltip("Use this if your Animator has a Trigger parameter.")]
    public string triggerName = "";

    [Tooltip("Use this if you want to play an animation state directly, e.g. Open, Activate, Armature|Action.")]
    public string animationStateName = "";

    [Tooltip("Optional: keep Animator disabled until full calmness so it does not play too early.")]
    public bool disableAnimatorUntilFullCalmness = false;

    [Header("Legacy Animation Component")]
    public Animation legacyAnimationToPlay;

    [Tooltip("Leave empty to play the default legacy animation.")]
    public string legacyAnimationName = "";

    [Header("Sound When Animation Plays")]
    public AudioSource animationAudioSource;
    public AudioClip animationSound;
    [Range(0f, 1f)] public float animationSoundVolume = 1f;

    private bool done;

    void Awake()
    {
        if (calmnessController == null)
            calmnessController = FindFirstObjectByType<CalmnessController>();

        if (hideTargetOnStart && targetToEnable != null)
            targetToEnable.SetActive(false);

        if (disableAnimatorUntilFullCalmness && animatorToPlay != null)
            animatorToPlay.enabled = false;

        if (animationAudioSource == null)
            animationAudioSource = GetComponent<AudioSource>();

        if (animationAudioSource != null)
        {
            animationAudioSource.playOnAwake = false;
            animationAudioSource.spatialBlend = 1f; 
        }
    }

    void Update()
    {
        if (done) return;
        if (calmnessController == null) return;

        if (calmnessController.calmness >= revealAt)
        {
            done = true;
            RevealAndPlay();
        }
    }

    private void RevealAndPlay()
    {
        if (targetToEnable != null)
            targetToEnable.SetActive(true);

        PlayAnimationSound();

        if (animatorToPlay == null && targetToEnable != null)
            animatorToPlay = targetToEnable.GetComponentInChildren<Animator>(true);

        if (animatorToPlay != null)
        {
            animatorToPlay.enabled = true;

            if (!string.IsNullOrEmpty(triggerName))
            {
                animatorToPlay.SetTrigger(triggerName);
            }
            else if (!string.IsNullOrEmpty(animationStateName))
            {
                animatorToPlay.Play(animationStateName, 0, 0f);
            }
            else
            {
                Debug.LogWarning("Animator found, but no triggerName or animationStateName was set.");
            }
        }

        if (legacyAnimationToPlay != null)
        {
            if (!string.IsNullOrEmpty(legacyAnimationName))
                legacyAnimationToPlay.Play(legacyAnimationName);
            else
                legacyAnimationToPlay.Play();
        }
    }

    private void PlayAnimationSound()
    {
        if (animationAudioSource == null || animationSound == null)
            return;

        animationAudioSource.PlayOneShot(animationSound, animationSoundVolume);
    }
}
