using System.Collections;
using UnityEngine;

public class MushroomPowerUpBlock : BaseBlock
{
    [SerializeField] public Animator animator;
    [Header("PowerUp Settings")]
    [SerializeField] private PowerUpReleaser powerUpReleaser;

    protected override void Awake()
    {
        base.Awake();

        // Configure this block type
        canReleasePowerUp = true;
        staysBouncy = false;
        hasAnimation = true;

        if (powerUpReleaser == null)
        {
            Debug.LogWarning("PowerUpReleaser not assigned in inspector. Attempting to find it on the same GameObject.");
        }
    }

    protected override void Start()
    {
        base.Start();
        if (animator)
        {
            animator.SetBool("isActive", true);
        }
    }

    protected override void PlayAnimation()
    {
        if (animator)
        {
            Debug.Log("QuestionMarkBlock: Playing animation");
            animator.SetBool("isActive", false);
        }
    }

    protected override void ResetAnimation()
    {
        if (animator)
        {
            Debug.Log("QuestionMarkBlock: Stopping animation");
            animator.SetBool("isActive", true);
        }
    }

    protected override void ReleasePowerUp()
    {
        if (powerUpReleaser != null)
        {
            Debug.Log("Releasing powerup from " + gameObject.name);
            powerUpReleaser.ReleasePowerUp();
            Debug.Log("Powerup released, setting canReleasePowerUp to false");
        }
        else
        {
            Debug.LogWarning("PowerUpReleaser not found on " + gameObject.name);
        }
    }

    protected override void ResetBlock()
    {
        base.ResetBlock();

        if (powerUpReleaser != null)
        {
            powerUpReleaser.ResetPowerUp();
        }
    }
}