using System;
using System.Collections;
using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] public AudioClip coinPickUpSound;
    [SerializeField] public AnimationClip coinFlyAnimation;
    private Animator coinAnimator;
    private SpriteRenderer coinSpriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Debug.Assert(audioSource != null, "AudioSource component missing from CoinBehavior");

        coinSpriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Assert(coinSpriteRenderer != null, "SpriteRenderer component missing from CoinBehavior");
        coinSpriteRenderer.enabled = false;

        coinAnimator = GetComponent<Animator>();
        Debug.Assert(coinAnimator != null, "Animator component missing from CoinBehavior");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayCoinSound()
    {
        Debug.Log("Playing coin sound");
        audioSource.PlayOneShot(coinPickUpSound);
        StartCoroutine(DestroyAfterSound());
    }

    private IEnumerator DestroyAfterSound()
    {
        yield return new WaitForSeconds(coinPickUpSound.length);

        Destroy(gameObject);
    }

    public IEnumerator SpawnAndAnimateCoin(Action onAnimationComplete)
    {
        coinSpriteRenderer.enabled = true;

        coinAnimator.SetTrigger("PlayerHit");
        Debug.Log("Coin spawned and animation triggered");

        yield return new WaitForSeconds(coinFlyAnimation.length);

        coinSpriteRenderer.enabled = false;

        PlayCoinSound();
        onAnimationComplete?.Invoke();
    }

    public void ResetCoinVisual()
    {
        coinSpriteRenderer.enabled = true;
        coinAnimator.ResetTrigger("PlayerHit");
        coinAnimator.Play("Idle", 0);
    }
}
