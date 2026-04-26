using UnityEngine;

public class ResolveEffectOnPlaced : MonoBehaviour
{
    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip soundToPlay;
    public bool playOnResolve = true;
    public bool loopSound = false;

    [Header("Existing Particle System")]
    public ParticleSystem existingParticles;
    public bool clearParticlesBeforePlay = true;

    [Header("Optional Prefab Particles")]
    public ParticleSystem starsPrefab;
    public Transform vfxSpawnPoint;
    public bool parentVFXToThisObject = false;
    public float autoDestroyVFXAfter = 10f;

    private bool hasPlayed = false;

    public void PlayResolveEffect()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        PlaySound();
        PlayExistingParticles();
        SpawnParticlePrefab();
    }

    private void PlaySound()
    {
        if (!playOnResolve) return;
        if (audioSource == null || soundToPlay == null) return;

        audioSource.clip = soundToPlay;
        audioSource.loop = loopSound;
        audioSource.Play();
    }

    private void PlayExistingParticles()
    {
        if (existingParticles == null) return;

        if (clearParticlesBeforePlay)
            existingParticles.Clear(true);

        existingParticles.Play(true);
    }

    private void SpawnParticlePrefab()
    {
        if (starsPrefab == null) return;

        Vector3 pos = vfxSpawnPoint != null ? vfxSpawnPoint.position : transform.position;
        Quaternion rot = Quaternion.identity;

        ParticleSystem spawned = Instantiate(
            starsPrefab,
            pos,
            rot,
            parentVFXToThisObject ? transform : null
        );

        spawned.Play();

        if (autoDestroyVFXAfter > 0f)
            Destroy(spawned.gameObject, autoDestroyVFXAfter);
    }
}
